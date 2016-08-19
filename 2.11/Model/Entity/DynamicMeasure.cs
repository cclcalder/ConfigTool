using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model
{ 
//      <Measure>
//        <ColumnCode>TotalVol_SI</ColumnCode>
//        <Name>Total Sales In  (cases)</Name>
//        <Value>0.0000</Value>
//        <Display>1</Display>
//        <Format>N0</Format>
//        <Colour>#FFFFFF</Colour>
//        <IsEditable>1</IsEditable>
//        <ColumnSortOrder>7</ColumnSortOrder>
//        <IsCalculated>0</IsCalculated>
//        <CalcFormula />
//        <HasTotal>SUM</HasTotal>
//      </Measure>
  
	[Serializable()]
	public class DynamicMeasure
	{
		public DynamicMeasure()
		{

		}
		public DynamicMeasure(XElement el)
		{
			ID = el.GetValueOrDefault<string>("ColumnCode");
			Name = el.GetValue<string>("Name");
		   
			Display = (el.GetValueOrDefault<string>("Display") == "0" ? false : true);
			Format = el.GetValue<string>("Format");
			Colour = el.GetValue<string>("Colour");
			ColumnSortOrder = el.GetValue<string>("ColumnSortOrder");
			CalcFormula = el.GetValue<string>("CalcFormula");
            HasTotal = el.GetValueOrDefault<string>("HasColumnTotal").ToLower();
			Value = this.InvariantFormatValue(el.GetValue<string>("Value"));

			try
			{
				RawValue = el.GetValueOrDefault<decimal>("Value");
			}
			catch
			{
				RawValue = 0;
			}

			IsReadOnly = el.GetValueOrDefault<string>("IsEditable") == "0";
            IsRowCalculated = el.GetValueOrDefault<string>("IncludedInRowTotal") == "0";



//            XElement demo = XElement.Parse(@" <RemoteCalc>
//                                      <TargetGrid>Promotion</TargetGrid>
//                                      <TargetFieldID>9</TargetFieldID>
//                                      <Action>MUL</Action>
//                                      <Products>
//                                        <Product>
//                                          <ID>15125</ID>
//                                          <Factor>20</Factor>
//                                        </Product>
//                                        <Product>
//                                          <ID>15138</ID>
//                                          <Factor>50</Factor>
//                                        </Product>
//                                        <Product>
//                                          <ID>15171</ID>
//                                          <Factor>100</Factor>
//                                        </Product>
//                                        <Product>
//                                          <ID>15185</ID>
//                                          <Factor>25</Factor>
//                                        </Product>
//                                      </Products>
//                                    </RemoteCalc>");
			RemoteData =  new RemoteCalc(el.Element("RemoteCalc")); //  new RemoteCalc(demo);//

		}

		/// <summary>
		/// Gets or sets the Id of this PromotionMeasure.
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// Gets or sets the Name of this PromotionMeasure.
		/// </summary>
		public string Name { get; set; }

		private string _value;
		private decimal _rawvalue;

		/// <summary>
		/// Gets or sets the Value of this PromotionMeasure.
		/// </summary>
		public decimal RawValue
		{
			get { return _rawvalue; }
			set
			{

				_rawvalue = value;

			}
		}

		public string Value
		{
			get { return _value; }
			set
			{
				_value = FormatValue(value);
			}
		}

		private string FormatValue(string value)
		{
			decimal d;
			string returnValue = value;
			if (decimal.TryParse(value.Replace("%", ""), NumberStyles.Any, CultureInfo.CurrentCulture, out d))
			{
				returnValue = FormatValue(d);
			}
			return returnValue;
		}

		private string FormatValue(decimal value)
		{
			var result = (value).ToString(Format, CultureInfo.CurrentCulture.NumberFormat);
			return result;
		}

		private string InvariantFormatValue(string value)
		{
			decimal d;
			string returnValue = value;
			if (decimal.TryParse(value.Replace("%", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
			{
				returnValue = FormatValue(d);
			}
			return returnValue;
		}
		/// <summary>
		/// Gets or sets the Format of this PromotionMeasure.
		/// </summary>
		public string Format { get; set; }

		public bool IsReadOnly { get; set; }
		public bool IsRowCalculated { get; set; }

		public bool Display { get; set; }

		public string Colour { get; set; }

		public string ColumnSortOrder { get; set; }

		public string CalcFormula { get; set; }

		public string HasTotal { get; set; }
		public RemoteCalc RemoteData { get; set; }

	}

	public class RemoteCalc
	{
    // <RemoteCalc>
    //    <TargetGrid>Promotion</TargetGrid>
    //    <TargetFieldID>9</TargetFieldID>
    //    <Action>MUL</Action>
    //    <Products>
    //    <Product>
    //        <ID>15125</ID>
    //        <Factor>20</Factor>
    //    </Product>
    //    <Product>
    //        <ID>15138</ID>
    //        <Factor>50</Factor>
    //    </Product>
    //    <Product>
    //        <ID>15171</ID>
    //        <Factor>100</Factor>
    //    </Product>
    //    <Product>
    //        <ID>15185</ID>
    //        <Factor>25</Factor>
    //    </Product>
    //    </Products>
    //</RemoteCalc>

		private XElement xElement;

		public RemoteCalc(XElement el)
		{
            ProductFactors = new Dictionary<int, decimal>();
			TargetGrid = el.GetValueOrDefault<string>("TargetGrid");
            TargetFieldID = el.GetValueOrDefault<int>("TargetFieldID");
			Action = el.GetValueOrDefault<string>("Action");
             
			var products = el.Element("Products");

			if (products != null)
			{
				foreach (var product in products.Elements())
				{
					var ID = Convert.ToInt32(product.Element("ID").Value);
					var Factor = Convert.ToDecimal(product.Element("Factor").Value);


					ProductFactors.Add(ID, Factor);
				}
			}

		}


		public string TargetGrid { get; set; }
        public int TargetFieldID { get; set; }
		public string Action { get; set; }

		public Dictionary<int, decimal> ProductFactors { get; set; }

		public static decimal RunAction(decimal num1, decimal num2, string action)
		{

			decimal res = 0;
			switch (action.ToLower())
			{
				case "add":
					res = num1 + num2;
					break;
				case "sub":
					res = num1 - num2;
					break;
				case "mul":
					res = num1 * num2;
					break;
				case "div":
					res = num1 / num2;
					break;
			}

			return Math.Round(res, 2);

		}

	}
}
