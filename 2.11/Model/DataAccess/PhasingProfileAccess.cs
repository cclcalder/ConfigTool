using Exceedra.Common;

namespace Model.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Entity;
    using System.Windows;

    public interface IPhasingAccess
    {
        IEnumerable<DayPhasingProfile> GetDayProfiles(IEnumerable<string> promotionIDs);
        IEnumerable<WeekPhasingProfile> GetWeekProfiles(IEnumerable<string> promotionIDs);
        Task<IEnumerable<DayPhasingProfile>> GetDayProfilesAsync(IEnumerable<string> promotionIDs);
        Task<IEnumerable<WeekPhasingProfile>> GetWeekProfilesAsync(IEnumerable<string> promotionIDs);
        Task SaveAsync(PhasingProfile profile);
        Task DeleteAsync(PhasingProfile profile);
    }

    public class PhasingAccess : IPhasingAccess
    {
        public IEnumerable<DayPhasingProfile> GetDayProfiles(IEnumerable<string> promotionIDs)
        {
            return GetProfiles("Daily", StoredProcedure.GetPhasesDaily, x => DayPhasingProfile.FromXml(x, GetPhasingTimeSections), promotionIDs).Cast<DayPhasingProfile>();
        }

        public IEnumerable<WeekPhasingProfile> GetWeekProfiles(IEnumerable<string> promotionIDs)
        {
            return GetProfiles("Weekly", StoredProcedure.GetPhasesWeekly, x => WeekPhasingProfile.FromXml(x, GetPhasingTimeSections), promotionIDs).Cast<WeekPhasingProfile>();
        }

        public Task<IEnumerable<DayPhasingProfile>> GetDayProfilesAsync(IEnumerable<string> promotionIDs)
        {
            return GetProfilesAsync("Daily", StoredProcedure.GetPhasesDaily, x => DayPhasingProfile.FromXml(x, GetPhasingTimeSections), promotionIDs)
                .ContinueWith(t => t.Result.Cast<DayPhasingProfile>());
        }

        public Task<IEnumerable<WeekPhasingProfile>> GetWeekProfilesAsync(IEnumerable<string> promotionIDs)
        {
            return GetProfilesAsync("Weekly", StoredProcedure.GetPhasesWeekly, x => WeekPhasingProfile.FromXml(x, GetPhasingTimeSections), promotionIDs)
                .ContinueWith(t => t.Result.Cast<WeekPhasingProfile>());
        }

        public IEnumerable<decimal> GetPhasingTimeSections(string phaseId)
        {
            //if (phaseId == "-1") return null;
            var argument = new XElement("GetPhasingDetail");
            var userNode = new XElement("UserID");
            userNode.SetValue(User.CurrentUser.ID);
            argument.Add(userNode);
            var phaseIdNode = new XElement("PhaseID");
            phaseIdNode.SetValue(phaseId);
            argument.Add(phaseIdNode);

            var xml = WebServiceProxy.Call(StoredProcedure.GetPhasingDetail, argument);
            return xml
                .Element("TimeSections")
                .MaybeElements("Section")
                .OrderBy(x => int.Parse(x.Element("ID").MaybeValue()))
                .Select(x => decimal.Parse(x.Element("Value").MaybeValue()));
        }

        public IEnumerable<PromoPhasing> GetPromoPhasings(IEnumerable<string> promotionIDs)
        {
            var xml = new XElement("GetPromoPhasing");
            xml.Add(new XElement("UserID", User.CurrentUser.ID));
            var promotions = new XElement("Promotions");
            foreach (var promotionID in promotionIDs)
            {
                promotions.Add(new XElement("ID", promotionID));
            }
            xml.Add(promotions);

            var result = WebServiceProxy.Call(StoredProcedure.GetPromoPhasing, xml);

            return result.Elements("Promo").Select(PromoPhasing.Parse);
        }

        public Task<IEnumerable<PromoPhasing>> GetPromoPhasingsAsync(IEnumerable<string> promotionIDs)
        {
            var ids = promotionIDs.ToArray();
            if (ids.Length == 0) return Task.Factory.FromResult(Enumerable.Empty<PromoPhasing>());
            var xml = new XElement("GetPromoPhasing");
            xml.Add(new XElement("UserID", User.CurrentUser.ID));
            var promotions = new XElement("Promotions");
            foreach (var promotionID in ids)
            {
                promotions.Add(new XElement("ID", promotionID));
            }
            xml.Add(promotions);

            return WebServiceProxy.CallAsync(StoredProcedure.GetPromoPhasing, xml, DisplayErrors.No)
                .ContinueWith(t => t.Result == null ? Enumerable.Empty<PromoPhasing>() : t.Result.Elements("Promo").Select(PromoPhasing.Parse));
        }

        private XElement CreateSaveArgument(PhasingProfile profile)
        {
            /* XML
             * <SavePhasing>
                        <UserID>1</UserID>
                        <PhaseID></PhaseID>
                        <PhaseType>Weekly</PhaseType>
                        <PhaseName>New Phase</PhaseName>
                        <IsNewPhase>1</IsNewPhase>
                        <TimeSections>
                            <Section>
                                <ID>1</ID>
                                <Value>0</Value>
                            </Section>
                            <Section>
                                <ID>2</ID>
                                <Value>.5</Value>
                            </Section>
                            <Section>
                                <ID>3</ID>
                                <Value>.2</Value>
                            </Section>
                            <Section>
                                <ID>4</ID>
                                <Value>.3</Value>
                            </Section>							
                        </TimeSections>
                    </SavePhasing> 
             */

            var xml = new XElement("SavePhasing");
            xml.Add(new XElement("UserID", User.CurrentUser.ID));
            xml.Add(new XElement("PhaseID", profile.ID));
            xml.Add(new XElement("PhaseType", profile.Type));
            xml.Add(new XElement("PhaseName", profile.Name));
            xml.Add(new XElement("IsNewPhase", profile.ID == "0" ? "1" : "0"));

            var timeSections = new XElement("TimeSections");

            for (int i = 0; i < profile.Size; i++)
            {
                timeSections.Add(
                    new XElement("Section",
                        new XElement("ID", i + 1),
                        new XElement("Value", profile.Values[i])
                        )
                    );
            }

            xml.Add(timeSections);

            return xml;
        }

        public Task SaveAsync(PhasingProfile profile)
        {
            var argument = CreateSaveArgument(profile);
            return WebServiceProxy.CallAsync(StoredProcedure.SavePhasing, argument)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.Result == null) return;

                    string mess = t.Result.Element("SuccessMessage").Value;
                    MessageBox.Show(mess, (mess.Contains("Success") ? "Success" : "Error"), MessageBoxButton.OK, (mess.Contains("Success") ? MessageBoxImage.Information : MessageBoxImage.Error));

                    if (t.Result.Element("Error") != null)
                    {
                        throw new ExceedraDataException(t.Result.Element("Error").MaybeValue());
                    }
                });
        }

        public Task DeleteAsync(PhasingProfile profile)
        {
            var xml = new XElement("DeletePhasing");
            xml.Add(new XElement("UserID", User.CurrentUser.ID));
            xml.Add(new XElement("PhaseID", profile.ID));

            return WebServiceProxy.CallAsync(StoredProcedure.DeletePhasing, xml);
        }

        private IEnumerable<PhasingProfile> GetProfiles(string type, string proc, Func<XElement, PhasingProfile> parse, IEnumerable<string> promotionIDs)
        {
            var argument = CreateGetProfilesArgument(type, promotionIDs);

            try
            {
                var nodes = WebServiceProxy.Call(proc, argument, DisplayErrors.No).Elements();
                return nodes.Select(parse);
            }
            catch
            {
                return Enumerable.Empty<PhasingProfile>();
            }

        }

        private static XElement CreateGetProfilesArgument(string type, IEnumerable<string> promotionIDs)
        {
            var argument = new XElement("GetPhases" + type);
            var userNode = new XElement("UserID");
            var promotions = new XElement("Promotions");

            userNode.SetValue(User.CurrentUser.ID);
            argument.Add(userNode);

            foreach (var promotionID in promotionIDs)
            {
                promotions.Add(new XElement("ID", promotionID));
            }
            argument.Add(promotions);

            return argument;
        }

        private static XElement CreateApplyProfileArgument(string weekPhaseID, string dayAPhaseID, string dayBPhaseID, string dayCPhaseID, string postPromoPhaseId, IEnumerable<string> promotions)
        {
            /* <SavePromoPhasing>
                     <UserID>1</UserID>
                     <WeekPhaseID>1</WeekPhaseID>
                     <DayAPhaseID>1</DayAPhaseID>
                     <DayBPhaseID>1</DayBPhaseID>
                     <DayCPhaseID>1</DayCPhaseID>
                     <Promotions>
                            <ID>100</ID>
                            <ID>111</ID>
                            <ID>222</ID>
                     </Promotions>
              <SavePromoPhasing> */
            var argument = new XElement("SavePromoPhasing");
            AddArgument(argument, "UserID", User.CurrentUser.ID);
            AddArgument(argument, "WeekPhaseID", weekPhaseID);
            if (dayAPhaseID != null)
            {
                AddArgument(argument, "DayAPhaseID", dayAPhaseID);
            }
            if (dayBPhaseID != null)
            {
                AddArgument(argument, "DayBPhaseID", dayBPhaseID);
            }
            if (dayCPhaseID != null)
            {
                AddArgument(argument, "DayCPhaseID", dayCPhaseID);
            }
            if (postPromoPhaseId != null)
            {
                AddArgument(argument, "PostPromoPhaseID", postPromoPhaseId);
            }

            var promotionsNode = new XElement("Promotions");
            foreach (var promotion in promotions)
            {
                AddArgument(promotionsNode, "ID", promotion);
            }

            argument.Add(promotionsNode);

            return argument;
        }

        private static void AddArgument(XElement element, XName name, object value)
        {
            var node = new XElement(name);
            node.SetValue(value);
            element.Add(node);
        }

        private Task<IEnumerable<PhasingProfile>> GetProfilesAsync(string type, string proc, Func<XElement, PhasingProfile> parse, IEnumerable<string> promotionIDs)
        {
            var argument = CreateGetProfilesArgument(type, promotionIDs);

            return WebServiceProxy.CallAsync(proc, argument, DisplayErrors.No)
.ContinueWith(t => (t.IsFaulted || t.Result == null)
                       ? Enumerable.Empty<PhasingProfile>()
                       : t.Result.Elements().Select(parse));
        }

        public Task<ValidationResult> ValidatePhasingAsync(IEnumerable<string> promotionIds, string weekProfileId, string leadInProfileId, string fullProfileId, string endingProfileId, string postPromoProfileId)
        {
            var argument = CreateApplyProfileArgument(weekProfileId, leadInProfileId, fullProfileId, endingProfileId, postPromoProfileId,
                                                      promotionIds.ToList());

            return WebServiceProxy.CallAsync(StoredProcedure.ValidatePromoPhasing, argument)
                .ContinueWith(t => t.IsFaulted ? new ValidationResult(ValidationStatus.Error, t.Exception.AggregateMessages()) : GetValidatePhasingMessage(t.Result));
        }

        public Task<string> ApplyPhasingAsync(IEnumerable<string> promotionIds, string weekProfileId, string leadInProfileId, string fullProfileId, string endingProfileId, string postPromoProfileId)
        {
            var argument = CreateApplyProfileArgument(weekProfileId, leadInProfileId, fullProfileId, endingProfileId, postPromoProfileId,
                                                      promotionIds.ToList());

            return WebServiceProxy.CallAsync(StoredProcedure.ApplyPhasing, argument)
                .ContinueWith(t => t.IsFaulted ? t.Exception.AggregateMessages() : GetApplyPhasingMessage(t.Result));
        }

        private static ValidationResult GetValidatePhasingMessage(XElement element)
        {
            element = element.MaybeElements().FirstOrDefault();
            if (element == null) return new ValidationResult(ValidationStatus.Error, string.Empty);
            if (element.Name == "SuccessMessage") return new ValidationResult(ValidationStatus.Success, element.Value);
            if (element.Name == "Warning") return new ValidationResult(ValidationStatus.Warning, element.Value);
            return new ValidationResult(ValidationStatus.Error, element.Value);
        }
        private static string GetApplyPhasingMessage(XElement element)
        {
            return element.Elements().FirstOrDefault().MaybeValue();
        }

        public Task<string> RemovePhasingAsync(IEnumerable<string> promotionIds)
        {
            var argument = CreateRemovePhasingArgument(promotionIds);
            return WebServiceProxy.CallAsync(StoredProcedure.RemovePromoPhasing, argument)
                .ContinueWith(t => t.IsFaulted ? t.Exception.AggregateMessages() : GetRemovePhasingMessage(t.Result));
        }

        private static XElement CreateRemovePhasingArgument(IEnumerable<string> promotionIDs)
        {
            /* <RemovePromoPhasing>
                     <UserID>1</UserID>
                     <Promotions>
                            <ID>100</ID>
                            <ID>111</ID>
                            <ID>222</ID>
                     </Promotions>
              <RemovePromoPhasing> */
            var argument = new XElement("SavePromoPhasing");
            AddArgument(argument, "UserID", User.CurrentUser.ID);

            var promotionsNode = new XElement("Promotions");
            foreach (var id in promotionIDs)
            {
                AddArgument(promotionsNode, "ID", id);
            }

            argument.Add(promotionsNode);

            return argument;
        }

        private static string GetRemovePhasingMessage(XElement element)
        {
            return element.Elements().FirstOrDefault().MaybeValue();
        }
    }
}