using Model.Entity.ROBs;
using ViewModels;

namespace WPF.ViewModels.Shared
{
    public class CommentTypeViewModel: BaseViewModel
    {
        private CommentType _commentType;

        public CommentTypeViewModel(CommentType commentType)
        {
            _commentType = commentType;
        } 

        public string ID
        {
            get { return _commentType.ID; }
            set
            {
                _commentType.ID = value;
                OnPropertyChanged("ID");
            }
        }
         
        public string Name
        {
            get { return _commentType.Name; }
            set
            {
                _commentType.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public bool CanDelete
        {
            get { return _commentType.CanDelete; }
            set
            {
                _commentType.CanDelete = value;
                OnPropertyChanged("CanDelete");
            }
        }

        public bool IsSelected
        {
            get { return _commentType.IsSelected; }
            set
            {
                _commentType.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }
}