using System.ComponentModel;
using System.Xml.Linq;
using Model.Annotations;

namespace Model.Entity.CellsGrid
{
    public class InsightControl : INotifyPropertyChanged
    {
        private const string Element_Id = "Canvas_Element_Idx";
        private const string Element_Code = "Canvas_Element_Code";
        private const string Element_Name = "Canvas_Element_Name";
        private const string ColumnId = "ColumnIndex";
        private const string ColumnSp = "ColumnSpan";
        private const string RowId = "RowIndex";
        private const string RowSp = "RowSpan";
        private const string ControlTy = "ControlType";
        private const string DataSo = "DataSource";
        private const string DataSourceIn = "DataSourceInput";

        private object _dataSourceViewModel;

        public InsightControl(XElement element)
        {
            // input xml example        
            // <GridControl>
            //   <Canvas_Element_Idx>1</Canvas_Element_Idx>
            //   <Canvas_Element_Code>P_05_Title</Canvas_Element_Code>
            //   <Canvas_Element_Name>Insert Title Here</Canvas_Element_Name>
            //   <ControlType>Label</ControlType>
            //   <DataSource></DataSource>
            //   <DataSourceInput></DataSourceInput>
            //   <ColumnIndex>0</ColumnIndex>
            //   <ColumnSpan>10</ColumnSpan>
            //   <RowIndex>0</RowIndex>
            //   <RowSpan>2</RowSpan>
            // </GridControl>

            Id = element.GetValue<string>(Element_Id);
            Code = element.GetValue<string>(Element_Code);
            Name = element.GetValue<string>(Element_Name);
            ControlType = element.GetValue<string>(ControlTy);
            DataSource = element.GetValue<string>(DataSo);
            DataSourceInput = element.GetValue<string>(DataSourceIn);
            LinkTo = element.Element("LinkTo");
            ColumnIndex = element.GetValue<string>(ColumnId);
            ColumnSpan = element.GetValue<string>(ColumnSp);
            RowIndex = element.GetValue<string>(RowId);
            RowSpan = element.GetValue<string>(RowSp);
        }

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ColumnIndex { get; set; }
        public string ColumnSpan { get; set; }
        public string ColumnLastIndex
        {
            get { return (int.Parse(ColumnIndex) + int.Parse(ColumnSpan) - 1).ToString(); }
        }
        public string RowIndex { get; set; }
        public string RowSpan { get; set; }
        public string RowLastIndex
        {
            get { return (int.Parse(RowIndex) + int.Parse(RowSpan) - 1).ToString(); }
        }

        private string _controlType;
        public string ControlType
        {
            get { return _controlType; }
            set
            {
                _controlType = value;
                OnPropertyChanged("ControlType");
            }
        }

        public string DataSource { get; set; }
        public string DataSourceInput { get; set; }
        public XElement LinkTo { get; set; }

        public object DataSourceViewModel
        {
            get { return _dataSourceViewModel; }
            set
            {
                _dataSourceViewModel = value;
                OnPropertyChanged("DataSourceViewModel");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}