using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Exceedra.Controls.DynamicGrid.Models
{
    [Serializable]
    public class HeaderRow
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public int Count {
            get {
                return Operations.Count(); 
            } 
        }

        public List<HeaderOperation> Operations { get; set; }
        

    }
    [Serializable]
    public class HeaderOperation
    {
        // 2015 03 24
        //<HeaderRow>
        //    <HeaderOperation>
        //    <Label>Magic Calc</Label>
        //    <Type>Calculation</Type>
        //    <Calculation>PromoVol_SO = BaseVol_SO * (1 + INPUT / 100); PromoVol_SI = BaseVol_SI + PromoVol_SO – BaseVol_SO</Calculation>
        //    </HeaderOperation>
        //</HeaderRow>


        public string Type { get; set; }
        public string ProRatingColumnCode { get; set; }
        public string Calculation { get; set; }
        public string ParentColumnCode { get; set; }
        //public string ShowSearchableTextBox { get; set; }

        public string Key
        {
            get
            {
                return Type + "," + ParentColumnCode + "," + ProRatingColumnCode + "," + Calculation;
            }
        }

        public ObservableCollection<string> HelperCollection { get; set; }

        //public string Visible
        //{
        //    get;
        //    set;
        //}

        private Visibility _visibility;
        public Visibility Visibility
        {
            get { return (Type == "" ? Visibility.Collapsed : Visibility.Visible); }
            //{
            //    //   return (Visible == "System.Windows.Visibility.Hidden" || Visible == null ? Visibility.Hidden : Visibility.Visible);          
            //}
            set { _visibility = value; }

        }

        public string OperationLabel { get; set; }

    }
}
