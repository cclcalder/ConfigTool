using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Model;
using WPF.ViewModels.Shared;

namespace WPF.ViewModels
{
    public class StagedProductViewModel
    {
        private readonly StagedProduct _model;
        private ObservableCollection<GridValuesViewModel<StagedMeasure>> _bindableValues;

        public StagedProductViewModel(StagedProduct model)
        {
            _model = model;

            if (_model != null)
            {
                Measures = _model.Measures;
            }

            Compute();
        }

        internal void Compute()
        {
            _bindableValues = new ObservableCollection<GridValuesViewModel<StagedMeasure>>();

            foreach (var measure in MeasureNames)
            {
                var valuesForMeasure = Measures.Where(arg => arg.MeasureName == measure);
                _bindableValues.Add(new GridValuesViewModel<StagedMeasure>(measure, new ObservableCollection<StagedMeasure>(valuesForMeasure)));
            }
        }

        public string Id { get { return _model.ProductId; } }

        public string Name { get { return _model.ProductName; } }

        public List<StagedMeasure> Measures { get; set; }

        public List<string> Stages
        {
            get { return Measures.Select(arg => arg.StageName).Distinct().ToList(); }
        }

        public List<string> MeasureNames
        {
            get { return Measures.Select(arg => arg.MeasureName).Distinct().ToList(); }
        }

        public ObservableCollection<GridValuesViewModel<StagedMeasure>> BindableValues
        {
            get
            {
                return _bindableValues;
            }
        }
    }
}
