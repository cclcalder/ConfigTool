using Microsoft.Win32;
using Exceedra.Common.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.UserControls
{
	/// <summary>
	/// Interaction logic for FileEntry.xaml
	/// </summary>
	public partial class FileEntry : UserControl
	{
		public static DependencyProperty TextProperty = DependencyProperty.Register("Path", typeof(string), typeof(FileEntry), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));		

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
		
		public FileEntry()
		{
            InitializeComponent();
		}

		private void BrowseFile(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();						
			dlg.FileName = Path;
			dlg.Multiselect = false;

			bool? result = dlg.ShowDialog();
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
                    Messages.Instance.PutError(string.Format("File can not be found\n {0}", this.Path));
                  
                   // textBox.Dispatcher.BeginInvoke((Action)(() => { textBox.Focus(); }));
                    //this.Path = string.Empty;
                }
                
         
            }
         
        }
	}
}
