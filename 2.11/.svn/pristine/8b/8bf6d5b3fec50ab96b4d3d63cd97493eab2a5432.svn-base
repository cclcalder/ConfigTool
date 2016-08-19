using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Exceedra.Controls.DynamicRow.Models;

namespace Exceedra.Controls.Converters
{
  public  class DataitemTemplateSelector : DataTemplateSelector
    {

      public DataTemplate t_MultiSelectDropdown{ get; set; }
      public DataTemplate t_Dropdown { get; set; }
      public DataTemplate t_Checkbox { get; set; }
      public DataTemplate t_Textbox { get; set; }
      public DataTemplate t_Label { get; set; }

        public override DataTemplate SelectTemplate(object item,
          DependencyObject container)
        {
            var rec = (RowProperty)item; 
            
            switch (rec.ControlType)
            {
                case "MultiSelectDropdown":
                    return t_MultiSelectDropdown;
                    
                case "Dropdown":
                    return t_Dropdown;
                  
                case "Checkbox":
                    return t_Checkbox;
                    
                case "Textbox":
                    return t_Textbox;
                   
                default:

                    return t_Label;
            }
            
        }

    }
}
