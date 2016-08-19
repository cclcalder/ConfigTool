using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Exceedra.Common;

namespace Exceedra.Controls
{
	/// <summary>
	/// Interaction logic for FileEntry.xaml
	/// </summary>
	public partial class FileEntry : UserControl
	{
		public static  DependencyProperty TextProperty = DependencyProperty.Register("Path", typeof(string), typeof(FileEntry), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ReadOnlyProperty = DependencyProperty.Register("IsReadOnlyMode", typeof(bool), typeof(FileEntry), null);		

		public string Path { 
            get { 
                return GetValue(TextProperty) as string; }

            set
            {
                SetValue(TextProperty, value);
                tp.BorderBrush = new SolidColorBrush(Colors.Gray);
                if (string.IsNullOrEmpty(this.Path.Trim()) == false)
                {

                    if (this.Path.Contains("//") || this.Path.Contains("\\"))
                    {
                        //  Messages.Instance.PutInfo(string.Format("File is not on a local disk please make sure the file exists \n {0}", this.Path));
                       
                        return;
                    }
                    else if (!File.Exists(this.Path))
                    {
                        tp.ToolTip = (string.Format("File can not be found\n {0}", this.Path));
                        tp.BorderBrush = new SolidColorBrush(Colors.Red);
                        // textBox.Dispatcher.BeginInvoke((Action)(() => { textBox.Focus(); }));
                        //this.Path = string.Empty;
                    }
                }
            } 
        }

	    public bool IsReadOnlyMode
	    {
            get { return (bool) GetValue(ReadOnlyProperty) ; }
            set { SetValue(ReadOnlyProperty, value); }
	    }

	    public bool IsEnabled
	    {
	        get
	        {
                //if (IsReadOnlyMode == true)
                //{
                //    return false;
                //}
                //return true;
	            return false;
	        }
	    }

		public FileEntry()
		{
			InitializeComponent();
		}

		private void BrowseFile(object sender, RoutedEventArgs e)
		{
            OpenFileDialog dlg = new OpenFileDialog();

		    if (!String.IsNullOrEmpty(Path))
		    {
		        var filename = System.IO.Path.GetFileName(Path);
		        var directory = System.IO.Path.GetDirectoryName(Path);
		       
		        dlg.FileName = filename;
		        dlg.InitialDirectory = directory;
		    }

		    dlg.Multiselect = false;


            bool? result;
            try
            {
                result = dlg.ShowDialog();
            }
            catch (Win32Exception)
            {
                dlg = new OpenFileDialog();
                result = dlg.ShowDialog();
            }

		    if (result.Value)
		    {
		        Path = dlg.FileName;
		        BindingExpression be = GetBindingExpression(TextProperty);
		        if (be != null)
		            be.UpdateSource();
		    }
		    
		}

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            this.Path = textBox.Text;
            BindingExpression be = GetBindingExpression(TextProperty);
            if (be != null)
                be.UpdateSource();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (this.Path != null)
                if (string.IsNullOrEmpty(this.Path.Trim()) == false)
                {
                    // allows web/fileserver based files to be linked to cliams
                    if (this.Path.Contains("//") || this.Path.Contains("\\"))
                    {
                      //  Messages.Instance.PutInfo(string.Format("File is not on a local disk please make sure the file exists \n {0}", this.Path));
                  
                        return;
                    }
                    else if (!File.Exists(this.Path)) 
                    {
                      Common.Mvvm.Messages.Instance.PutError(string.Format("File can not be found\n {0}", this.Path));
                  
                       // textBox.Dispatcher.BeginInvoke((Action)(() => { textBox.Focus(); }));
                        //this.Path = string.Empty;
                    }
                
         
                }
         
        }

        public void OpenScanLocation(string scanLocation)
        {
            if(scanLocation != null)
                if (File.Exists(scanLocation) || scanLocation.Contains("//"))
                {
                    Process.Start(scanLocation);
                }
                else
                {
                    MessageBoxShow(string.Format("File can not be found\n {0}", scanLocation), "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
        }

        protected static void MessageBoxShow(string message, string title = null,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information)
        {
            switch (image)
            {
                case MessageBoxImage.Error:
                    Common.Mvvm.Messages.Instance.PutError(message);
                    break;
                case MessageBoxImage.Warning:
                    Common.Mvvm.Messages.Instance.PutWarning(message);
                    break;
                default:
                    Common.Mvvm.Messages.Instance.PutInfo(message);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenScanLocation(Path);
        }
	}
}
