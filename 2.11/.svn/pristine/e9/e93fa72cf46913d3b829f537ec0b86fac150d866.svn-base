using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace WPF
{
    public class TranslationData : IWeakEventListener,
                      INotifyPropertyChanged, IDisposable
    {
        private string _key;
        private string _fallback;

        public TranslationData(string key, string fallback)
        {
            _key = key;
            _fallback = fallback;
            LanguageChangedEventManager.AddListener(
                      TranslationManager.Instance, this);
        }

        ~TranslationData()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LanguageChangedEventManager.RemoveListener(
                          TranslationManager.Instance, this);
            }
        }


        public object Value
        {
            get
            {
                return TranslationManager.Instance.Translate(_key, _fallback);
            }
        }

        public bool ReceiveWeakEvent(Type managerType,
                                object sender, EventArgs e)
        {
            if (managerType == typeof(LanguageChangedEventManager))
            {
                OnLanguageChanged(sender, e);
                return true;
            }
            return false;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
 
}
