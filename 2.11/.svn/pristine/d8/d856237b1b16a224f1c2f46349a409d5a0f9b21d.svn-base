using System;
using System.Xml.Linq;
using Exceedra.Common.Utilities;
using Exceedra.Controls.Helpers;
using Model.Utilities;
using Telerik.Pivot.Core;
using Telerik.Pivot.Core.Aggregates;

namespace Exceedra.Pivot.ViewModels
{
    public class Property
    {
        public Property(XElement xElement)
        {
            if (xElement.Attribute("PivotType") != null)
                PivotType = (PivotType)Enum.Parse(typeof (PivotType), xElement.Attribute("PivotType").Value);

            Type = xElement.Attribute("Type") != null ? Converter.StringToType(xElement.Attribute("Type").Value) : typeof(object);
            Code = xml.FixNullInline(xElement.Attribute("Code"));
            
            // if no name specified it will be the same as the code
            Name = xml.FixNullInline(xElement.Attribute("Name"));
            if (string.IsNullOrEmpty(Name)) Name = Code;

            Format = xml.FixNullInline(xElement.Attribute("Format"));
            PivotBoxSortOrder = xml.FixInt(xElement.Attribute("PivotBoxSortOrder"));

            AggregationType = AggregateFunctions.Sum;
            if (xElement.Attribute("AggregationType") != null)
                AggregationType = Converter.StringToAggregateFunctionType(xElement.Attribute("AggregationType").Value);

            GridSorting = SortOrder.None;
            if (xElement.Attribute("GridSorting") != null)
                GridSorting = Converter.StringToSortOrder(xElement.Attribute("GridSorting").Value);

            if (xElement.Attribute("Equation") != null)
                Equation = xElement.Attribute("Equation").Value;

            PropertyXml = xElement;
        }

        public PivotType PivotType { get; set; }
        public Type Type { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }
        public string Format { get; set; }
        public int PivotBoxSortOrder { get; set; }

        public AggregateFunction AggregationType { get; set; }
        public SortOrder GridSorting { get; set; }

        /// <summary>
        /// Used only for calculated fields. (Check PivotCalculatedField.cs)
        /// </summary>
        public string Equation { get; set; }

        /// <summary>
        /// Could be useful
        /// </summary>
        public XElement PropertyXml { get; set; }
    }

    public enum PivotType
    {
        Filter,
        Column,
        Row,
        Value,
        CalculatedField
    }
}