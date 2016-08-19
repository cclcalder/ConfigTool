using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for version.xaml
    /// </summary>
    public partial class version : UserControl
    {
        public version()
        {
            InitializeComponent();

            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;

            lversion.Content = string.Format("Version {0}",ver.ToString());


        }
    }
}
