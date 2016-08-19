using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Model;
using Model.Entity.ROBs;
using ViewHelper;
using ViewModels;
using WPF.ViewModels.Shared;
using Coder.UI.WPF;
using System.ComponentModel;
using Exceedra.Common.Utilities;
using Model.Entity.Generic;
using Exceedra.MultiSelectCombo.ViewModel;

namespace WPF.UserControls
{
    public class CommentsViewModel : BaseViewModel
    {
        private ObservableCollection<CommentViewModel> _comments;
        private ActionCommand _deleteCommentCommand;

        public CommentsViewModel()
        {
            _comments = new ObservableCollection<CommentViewModel>();
        }

        private bool _filterEnabled;

        public bool FilterEnabled
        {
            get { return _filterEnabled; }
            set
            {
                _filterEnabled = value;
                PropertyChanged.Raise(this, "FilterEnabled");
            }
        }

        public bool CommentIsNotEmpty(object obj)
        {

            return true;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CommentViewModel> Comments
        {
            get { return _comments; }
            set
            {
                Set(ref _comments, value, "Comments");
            }
        }

        public ObservableCollection<CommentTypeViewModel> CommentTypes
        {
            get { return _commentTypes; }
            set { Set(ref _commentTypes, value, "CommentTypes"); }
        }

        public MultiSelectViewModel FilterCommentTypes
        {
            get { return _filtercommentTypes; }
            set { Set(ref _filtercommentTypes, value, "FilterCommentTypes"); }
        }


        private CommentViewModel _currentComment;
        public CommentViewModel CurrentComment
        {
            get { return _currentComment; }
            set { Set(ref _currentComment, value, "CurrentComment"); }
        }

        private CommentViewModel _selectedComment;
        public CommentViewModel SelectedComment
        {
            get { return _selectedComment; }
            set { Set(ref _selectedComment, value, "SelectedComment"); }
        }

        private string _newCommentText;
        public string NewCommentText
        {
            get { return _newCommentText; }
            set
            {
                Set(ref _newCommentText, value, "NewCommentText");
                PropertyChanged.Raise(this, "CommentIsNotEmpty");
            }
        }

        private CommentTypeViewModel _selectedCommentType;
        private ObservableCollection<CommentTypeViewModel> _commentTypes;
        private MultiSelectViewModel _filtercommentTypes = new MultiSelectViewModel();
        public CommentTypeViewModel SelectedCommentType
        {
            get { return _selectedCommentType; }
            set
            {
                Set(ref _selectedCommentType, value, "SelectedCommentType");
                PropertyChanged.Raise(this, "CommentIsNotEmpty");
            }
        }

        public ICommand AddCommentCommand
        {
            get { return new ViewCommand(CanAddComment, AddComment); }
        }

        public ICommand DeleteCommentCommand
        {
            get { return new ViewCommand(CommentIsNotEmpty, DeleteComment); }
        }

        #region AddCommentCommand
        private bool CanAddComment(object o)
        {
            return !string.IsNullOrWhiteSpace(NewCommentText) && !DisableAdd;
        }

        private void AddComment(object o)
        {

            if ((!string.IsNullOrEmpty(NewCommentText) && SelectedCommentType != null))
            {
                try
                {
                    _currentComment = new CommentViewModel(new Model.Entity.ROBs.Comment
                    {
                        ID = "0",
                        TimeStamp = DateTime.Now,

                        UserName = User.CurrentUser.DisplayName,
                        CommentType = SelectedCommentType.ID,
                        Value = NewCommentText
                    });
                    _currentComment.CanDelete = CommentTypes.FirstOrDefault(x => x.ID == SelectedCommentType.ID).CanDelete;
                    OnAddCommentExecuteMethod();
                }
                catch (Exception ex)
                {
                    MessageBoxShow(ex.Message);
                }
            }
            else
            {
                MessageBoxShow("Cannot save comment either type is not selected or comment text has not been added");
            }


            //call parent comment
        }

        public event EventHandler OnExecuteAddMethod;
        protected virtual void OnAddCommentExecuteMethod()
        {
            if (OnExecuteAddMethod != null)
            {
                OnExecuteAddMethod(this, EventArgs.Empty);

                Comments.Add(CurrentComment);
                NewCommentText = string.Empty;
            }
        }

        private void DeleteComment(object obj)
        {
            try
            {
                OnDeleteCommentExecuteMethod();
            }
            catch (Exception ex)
            {
                MessageBoxShow(ex.Message);
            }
        }

        public event EventHandler OnExecuteDeleteMethod;
        private void OnDeleteCommentExecuteMethod()
        {
            if (OnExecuteDeleteMethod != null)
            {
                OnExecuteDeleteMethod(this, EventArgs.Empty);

                if (Comments.Contains(CurrentComment))
                    Comments.Remove(CurrentComment);
            }
        }

        public bool DisableAdd { get; set; }

        #endregion

    }
}