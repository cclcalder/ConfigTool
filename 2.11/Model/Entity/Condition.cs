using System;
using System.Xml.Linq;

namespace Model.Entity
{
    public class Condition
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int SortIndex { get; set; }
        public string Name { get; set; }
        public int NumberOfAvailableMeasures { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MeasureName { get; set; }
        public string StatusName { get; set; }
        public string StatusColour { get; set; }

        public Condition ()
        {
            
        }

        public Condition(XElement xml)
        {
            FromXml(xml);
        }

        public void FromXml(XElement node)
        {
            const string dateFormat = "yyyyMMdd";
            const string idElement = "Cond_Idx";
            const string codeElement = "Cond_Code";
            const string sortIndexElement = "Cond_SortIdx";
            const string nameElement = "Cond_Name";
            const string availableMeasuresElement = "NumberOfAvailableMeasures";
            const string startDateElement = "Cond_Start_Date_Idx";
            const string endDateElement = "Cond_End_Date_Idx";
            const string measureNameElement = "PcCustProdMeasure_Name";
            const string statusNameElement = "Cond_Status_Name";
            const string satusColourElement = "Cond_Status_Colour";


            Id = node.GetValue<int>(idElement);
            Code = node.GetValue<string>(codeElement);
            SortIndex = node.GetValue<int>(sortIndexElement);
            Name = node.GetValue<string>(nameElement);
            NumberOfAvailableMeasures = node.GetValue<int>(availableMeasuresElement);
            StartDate = DateTime.ParseExact(node.GetValue<string>(startDateElement), dateFormat, null);
            EndDate = DateTime.ParseExact(node.GetValue<string>(endDateElement), dateFormat, null);
            MeasureName = node.GetValue<string>(measureNameElement);
            StatusName = node.GetValue<string>(statusNameElement);
            StatusColour = node.GetValue<string>(satusColourElement);

        }
    }
}