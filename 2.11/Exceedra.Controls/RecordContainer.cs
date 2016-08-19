using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Exceedra.Controls.ViewModels;
using Exceedra.DynamicGrid.Models;

namespace Exceedra
{
    public abstract class RecordContainer : Base
    {
        #region static

        public static T DeserializeFromXml<T>(string xml)
        {
            T result;
            XmlSerializer ser = new XmlSerializer(typeof (T));
            using (TextReader tr = new StringReader(xml))
            {
                result = (T) ser.Deserialize(tr);
            }

            return result;
        }

        public static void SerializeToXml<T>(T obj, string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            ser.Serialize(fileStream, obj);
            fileStream.Close();
        }

        public static XmlWriter fileStream { get; set; }

        #endregion

        #region fields

        private bool _hasChanged;
        private ObservableCollection<RecordBase> _recordsBase;

        #endregion

        #region properties

        public bool HasChanged
        {
            get { return _hasChanged; }
            set
            {
                _hasChanged = value;

                NotifyPropertyChanged(this, vm => vm.HasChanged);
            }
        }

        public ObservableCollection<RecordBase> RecordsBase
        {
            get
            {
                if (_recordsBase == null) _recordsBase = new ObservableCollection<RecordBase>();
                return _recordsBase;
            }
            set
            {
                _recordsBase = value;
                NotifyPropertyChanged(this, vm => vm.RecordsBase);
            }
        }

        #endregion

        #region public methods

        public virtual void AddRecord(RecordBase newRecord)
        {
            RecordsBase.Add(newRecord);
            NotifyPropertyChanged(this, vm => vm.RecordsBase);
        }

        /// <summary>
        /// Checks if all REQUIRED properties in all records have value(s)
        /// </summary>
        /// <returns>
        /// TRUE if all properties in all records with IsRequired == 1 have value(s)
        /// FALSE if at least one property in any record with IsRequired == 1 doesn't have any value
        /// </returns>
        public abstract bool AreRecordsFulfilled();

        #endregion
    }
}
