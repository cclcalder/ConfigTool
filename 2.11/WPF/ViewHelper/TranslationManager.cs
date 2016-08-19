using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Threading;
using Model.Language;
using System.Windows.Markup;
using System.Reflection;

namespace WPF
{
    public class TranslationManager
    {
        private static TranslationManager _translationManager;

        public event EventHandler LanguageChanged;
         
        public CultureInfo CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                //if (value != Thread.CurrentThread.CurrentUICulture)
                //{
                    
                    Thread.CurrentThread.CurrentUICulture = value;
                    Thread.CurrentThread.CurrentCulture = value;
                    App.LoadLanguage(value.IetfLanguageTag);
                    
                    OnLanguageChanged();
  
               // }
            }
        }


        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                if (TranslationProvider != null)
                {
                    return TranslationProvider.Languages;
                }
                return Enumerable.Empty<CultureInfo>();
            }
        }

        public static TranslationManager Instance
        {
            get
            {
                if (_translationManager == null)
                    _translationManager = new TranslationManager();
                return _translationManager;
            }
        }

        public ITranslationProvider TranslationProvider { get; set; }

        private void OnLanguageChanged()
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(this, EventArgs.Empty);
            }
        }

        public object Translate(string key, string fallback)
        {
            if (TranslationProvider != null)
            {
                object translatedValue = TranslationProvider.Translate(key, fallback);
                if (translatedValue != null)
                {
                    return translatedValue;
                }
            }
            return string.Format("!{0}!", key);
        }

     
    }
}
