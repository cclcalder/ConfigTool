using System;
using System.Globalization;
using System.Xml.Linq;

namespace Model.Entity
{


    public class ScenarioData
    {
        public ScenarioData() { }

        public ScenarioData(XElement el)
        {
            ID = el.GetValue<string>("Scen_Idx");
            Name = el.GetValue<string>("Scen_Name");
            IsSelected = el.GetValue<string>("IsSelected") == "1";

            if (el.Element("Start_Day_Idx") != null)
            {
                var startDate = DateTime.ParseExact(el.GetValue<string>("Start_Day_Idx"), "yyyyMMdd", CultureInfo.InvariantCulture);
                StartDate = startDate;
            }
            if (el.Element("End_Day_Idx") != null)
            {
                var endDate = DateTime.ParseExact(el.GetValue<string>("End_Day_Idx"), "yyyyMMdd", CultureInfo.InvariantCulture);
                EndDate = endDate;
            }
            if (el.Element("Scen_Type_Name") != null)
            {
                ItemType = el.GetValue<string>("Scen_Type_Name");
            }
            if (el.Element("Scen_Status_Name") != null)
            {
                StatusName = el.GetValue<string>("Scen_Status_Name");
            }
            if (el.Element("Scen_Status_Name") != null)
            {
                StatusColour = el.GetValue<string>("Scen_Status_Colour");
            }
            if (el.Element("NumberOfCustomers") != null)
            {
                NumberOfCustomers = el.GetValue<int>("NumberOfCustomers");
            }
            if (el.Element("NumberOfPromotions") != null)
            {
                NumberOfPromotions = el.GetValue<int>("NumberOfPromotions");
            }
            if (el.Element("NumberOfFundings") != null)
            {
                NumberOfFundings = el.GetValue<int>("NumberOfFundings");
            }
            if (el.Element("NumberOfProducts") != null)
            {
                NumberOfProducts = el.GetValue<int>("NumberOfProducts");
            }
            if (el.Element("Export") != null)
            {
                Export = el.GetValue<string>("Export");
            }

            if (el.Element("Created") != null)
            {
                var createdDate = DateTime.ParseExact(el.GetValue<string>("Created"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                CreatedDate = createdDate;
            }

            if (el.Element("Author") != null)
            {
                Author = el.GetValue<string>("Author");
            }
        }

        public bool IsSelected { get; set; }

        public int NumberOfProducts { get; set; }

        public string Export { get; set; }
        
        public string Delete { get; set; }
        public string Close { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedDateDisplay { 
            get 
            {
                return CreatedDate.ToString("dd/MM/yyyy");
            }
        }

        public string Author { get; set; }

        #region ID
        /// <summary>
        /// Gets or sets the Id of this PromotionData.
        /// </summary>
        public string ID { get; set; }
        #endregion

        #region Name
        /// <summary>
        /// Gets or sets the Name of this PromotionData.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Number ofCustomers
        /// <summary>
        /// Gets or sets the CustomerName of this PromotionData.
        /// </summary>
        public int NumberOfCustomers { get; set; }
        #endregion

        public int NumberOfPromotions { get; set; }
        
        public int NumberOfFundings { get; set; }

        #region StatusName
        /// <summary>
        /// Gets or sets the StatusName of this PromotionData.
        /// </summary>
        public string StatusName { get; set; }
        public string StatusColour { get; set; }
        #endregion


        #region ItemType
        public string ItemType { get; set; }
        #endregion
    }
}
