using Model.Entity.ROBs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModels;

namespace WPF.ViewModels.Shared
{
    public class CommentViewModel : BaseViewModel
    {
        private readonly Comment _comment;

        public CommentViewModel()
        { }
        public CommentViewModel(Comment comment)
        {
            _comment = comment;
        }

        public string Header
        {
            get { return FormatHeader(); }
        }

        public string Value
        {
            get { return _comment.Value; }
            set
            {
                _comment.Value = value;
                OnPropertyChanged("Value");
            }
        }

        public DateTime TimeStamp
        {
            get { return _comment.TimeStamp; }
            set
            {
                _comment.TimeStamp = value;
                OnPropertyChanged("Value");
            }
        }

        public string UserName
        {
            get { return _comment.UserName; }
            set
            {
                _comment.UserName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string CommentType
        {
            get { return _comment.CommentType; }
            set
            {
                _comment.CommentType = value;
                OnPropertyChanged("CommentType");
            }
        }

        public bool CanDelete
        {
            get { return _comment.CanDelete; }
            set
            {
                _comment.CanDelete = value;
                OnPropertyChanged("CanDelete");
            }
        }

        public string ID
        {
            get { return _comment.ID; }
            set
            {
                _comment.ID = value;
                OnPropertyChanged("ID");
            }
        }

        private string FormatHeader()
        {
            string time;
            if (_comment.TimeStamp.Date == DateTime.Today)
            {
                time = _comment.TimeStamp.ToLongTimeString();
            }
            else
            {
                time = string.Format("{0} at {1}", _comment.TimeStamp.ToShortDateString(),
                                     _comment.TimeStamp.ToShortTimeString());
            }

            return string.Format("{0} by {1}", time, _comment.UserName);
        }

    }

    public static class CommentExtentions
    {
        public static Comment ToEntity(this CommentViewModel commentViewModel)
        {
            return new Comment
                {
                    CommentType = commentViewModel.CommentType,
                    ID = commentViewModel.ID,
                    TimeStamp = commentViewModel.TimeStamp,
                    UserName = commentViewModel.UserName,
                    Value = commentViewModel.Value,
                    CanDelete = commentViewModel.CanDelete
                };
        }
    }
}
