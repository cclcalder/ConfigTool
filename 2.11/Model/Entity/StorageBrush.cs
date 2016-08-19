using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;


namespace Model.Entity
{
    public class StatusBrushStorage
    {

        public StatusBrushStorage()
        { }
        public StatusBrushStorage(List<ScheduleStatuses> statuses)
        {
            if (ProgrammeBrushes == null)
            {
                ProgrammeBrushes = new Dictionary<string, Brush>();
                foreach (var s in statuses)
                {
                      foreach (var child in s.Statuses)
                      {
                          var k = s.Name.Replace(" ", "").ToUpper() +
                                  child.Name.Replace(" ", "").Replace("(", "_").Replace(")", "_").ToUpper();

                        if (!ProgrammeBrushes.ContainsKey(k))
                        {
                            ProgrammeBrushes.Add(k, (SolidColorBrush)new BrushConverter().ConvertFrom(child.Colour));
                        } 
                      }
                }
            }
        }

        private static Dictionary<string, Brush> _programmeBrushes;
        public Dictionary<string, Brush> ProgrammeBrushes
        {

            set
            {
                _programmeBrushes = value;
            }
            get
            {
                return _programmeBrushes;
             
            }

        }
    }
}
