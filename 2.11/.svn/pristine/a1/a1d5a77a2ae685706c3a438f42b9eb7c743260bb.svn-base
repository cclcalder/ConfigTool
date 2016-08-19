using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using ViewHelper;

namespace WPF.ViewModels
{
    class StagedPromotionViewModel
    {
        private readonly StagedPromotion _model;
        private readonly List<StagedProductViewModel> _products;
        private readonly ICommand _saveCommand;
        private readonly ICommand _cancelCommand;
        private readonly PromotionAccess _dataAccess;
        private readonly Window _window;

        public StagedPromotionViewModel(PromotionAccess dataAccess, StagedPromotion model, Window window)
        {
            _window = window;
            _model = model;
            _dataAccess = dataAccess;

            if (_model != null)
            {
                _products = _model.Products.Select(arg => new StagedProductViewModel(arg)).ToList();
            }
            else
            {
                _products = new List<StagedProductViewModel>();
            }

            _cancelCommand = new ActionCommand(Close);
        }

        public List<StagedProductViewModel> Products
        {
            get { return _products; }
        }

        public ICommand SaveCommand
        {
            get { return new ViewCommand(CanSave, SaveStagedPromotion); }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        private bool CanSave(object o)
        {
            return _model != null && _model.IsEditable;
        }

        private void SaveStagedPromotion(object o)
        {
            try
            {
                _dataAccess.ValidatePromotionVolumesDaily(_model);
                _dataAccess.SaveStagedPromotion(_model);
                Close();
            }
            catch (ExceedraDataException error)
            {
                CustomMessageBox.Show(error.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close()
        {
            if (_window != null)
            {
                _window.Close();
            }
        }
    }
}
