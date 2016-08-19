using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using AzureProvider;
using Exceedra.Annotations;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Model.Entity.Diagnostics;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.Messages;
using Microsoft.Win32;
using Telerik.Windows.Cloud.Controls.Upload;

namespace Exceedra.Controls.Storage.Azure
{
    /// <summary>
    /// Interaction logic for UploadEntry.xaml
    /// </summary>
    public partial class UploadEntry : UserControl, INotifyPropertyChanged
    {
        //private RecordViewModel _filesRvm;
        //public RecordViewModel FilesRVM
        //{
        //    get
        //    {
        //        return _filesRvm;
        //    }
        //    set
        //    {
        //        if (_filesRvm != value)
        //        {
        //            _filesRvm = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

 

        //public string Screen
        //{
        //    get { return (string)GetValue(ScreenProperty); }
        //    set
        //    {
        //        SetValue(ScreenProperty, value);
        //    }
        //}

        private static AzureUploadProvider _provider;
        private static string _container;
        private static string _screen;
        private static string _idx;
        public void Load(string screen, string idx, StorageData settings)
        {
            _screen = screen;
            _idx = idx;
            if (settings != null)
            {
                  _provider = new AzureUploadProvider(settings.Account, settings.Key)
                {
                    UploadFileSettings = settings
                };
                _container = settings.Container;
                cloudUpload1.Provider = _provider;
            }
            root.IsEnabled = (settings != null);

            LoadFiles(_idx, _screen);
            cloudUpload1.CreateOpenFileDialog = () =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "All files (*.*)|*.*|" +
                             "PDF files (*.pdf)|*.pdf|" +
                             "Excel files 2003-2007 (*.xls)|*.xls|" +
                             "Excel files >2010 (*.xlsx)|*.xlsx|" +
                             "Word files 2003-2007 (*.doc)|*.doc|" +
                             "Word files >2010 (*.docx)|*.docx|" +
                             "Jpeg files (*.jpg)|*.jpg|" +
                             "Bitmap files (*.bmp)|*.bnp|" +
                             "Text files (*.txt)|*.txt";

                          
            
                return ofd;
            };


        }

        private void LoadFiles(string idx, string screen)
        {

            var results = new Model.DataAccess.Storage.StorageAccess().GetFiles(idx, screen).ContinueWith(
                t =>
                {
                    #region xml
                    //            var t = XElement.Parse(@" <Results>
                    //	<RowsLimitedAt>500</RowsLimitedAt>
                    //	<RowsAvailable>8</RowsAvailable>
                    //	<RootItem>
                    //		<Item_Idx>2</Item_Idx>
                    //		<Item_Type>ROBGrid</Item_Type>
                    //		<Item_RowSortOrder>1</Item_RowSortOrder>
                    //		<Attributes>
                    //			<Attribute>
                    //				<ColumnCode>ROB_Name</ColumnCode>
                    //				<HeaderText>From hard coded XML</HeaderText>
                    //				<Value>Static data from XML</Value>
                    //				<ExternalData>img_052610-geeky-tees-3_thumb555.jpg</ExternalData>
                    //				<Format />
                    //				<ForeColour />
                    //				<BorderColour />
                    //				<IsDisplayed>1</IsDisplayed>
                    //				<IsEditable>0</IsEditable>
                    //				<ControlType>ExternalHyperlink</ControlType>
                    //				<TotalsAggregationMethod>NONE</TotalsAggregationMethod>
                    //				<ColumnSortOrder>0</ColumnSortOrder>
                    //			</Attribute>
                    //		</Attributes>
                    //	</RootItem>

                    //</Results>");
                    #endregion
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        filesGrid.ItemDataSource = RecordViewModel.LoadWithData(t.Result)
                        ));
                });
        }
         

        public UploadEntry()
        {
           
            InitializeComponent();
            root.IsEnabled = false;

            filesGrid.HyperLinkHandler = HyperLinkHandler;

        }

        private void HyperLinkHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            Record obj = ((FrameworkElement)sender).DataContext as Record;
            
            var idxColumn = obj.Properties.FirstOrDefault(t => t.ControlType.ToLower() == "externalhyperlink");

            if (idxColumn != null && !idxColumn.Value.IsEmpty())
            {
               
                //get file for user
                DownloadFileFromBlob(idxColumn.Value);
            }
            else
            {
                CustomMessageBox.ShowOK("No file found in data", "Information", "Ok");
            }
        }

        public void DownloadFileFromBlob(string fileName)
        {
            var account = _provider.Account;
           var blobClient = account.CreateCloudBlobClient();
           var container = blobClient.GetContainerReference(_container);
           var blob = container.GetBlobReference(fileName);
            MemoryStream memStream = new MemoryStream();
            blob.DownloadToStream(memStream);

            var dlg = new SaveFileDialog();
            dlg.FileName = fileName; 

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                using (FileStream file = File.Create(dlg.FileName))
                {
                    memStream.WriteTo(file);
                    //open the saved file using default program
                    Process.Start(dlg.FileName);

                    //we have a view, send to DB to increment count
                    var access = new Model.DataAccess.Storage.StorageAccess();
                    access.UpdateFileView(fileName);


                }
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloudUpload1_OnItemStateChanged(object sender, CloudUploadFileStateChangedEventArgs e)
        {
            var state = e.Item.State;
            var path = System.IO.Path.GetFileName(_provider.FixedFilename);
            

            if (state == CloudUploadFileState.Uploaded)
            {

                var access = new Model.DataAccess.Storage.StorageAccess();
                var x = access.SaveFile(_screen, path, _idx, _provider.FixedFileSize).ContinueWith(t=>
                {
                    LoadFiles(_idx, _screen);
                    var mess = t.Result.Element("Message").MaybeValue();
            
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        CustomMessageBox.Show(mess, "Upload notification")
                       ));
                    
                    
                });
              
                //
            }
             
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            filesGrid.Visibility = Visibility.Collapsed;
            root.Visibility = Visibility.Visible;
        }
        private void MenuItem2_OnClick(object sender, RoutedEventArgs e)
        {
            filesGrid.Visibility = Visibility.Visible;
            root.Visibility = Visibility.Collapsed;
        }
    }
}
