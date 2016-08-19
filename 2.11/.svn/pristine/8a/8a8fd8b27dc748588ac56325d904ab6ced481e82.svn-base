using Model.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WPF
{
    public class TranslationProvider : ITranslationProvider
    {
 
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ResxTranslationProvider"/> class.
        /// </summary>
        /// <param name="baseName">Name of the base.</param>
        /// <param name="assembly">The assembly.</param>
        public TranslationProvider(Assembly assembly)
        {
           
        }

        #endregion

        #region ITranslationProvider Members

        /// <summary>
        /// See <see cref="ITranslationProvider.Translate" />
        /// </summary>
        public object Translate(string key, string fallback)
        {
            if (App.ShowKeys)
            {
                return key;
            }

            if (App.CurrentLang == null)
            {
                return fallback;
            }

            var ret = App.CurrentLang.AppLabels.FirstOrDefault(r => r.Code == key);
            var name = ret != null ? ret.Name : null;

            if (name == null)
            {
                App.LogError(String.Format("Missing label: {0}", key));

                //todo: save translated label in DB and update currentlang entries
                //var f = new Model.Translate.Translator().Translate(fallback, App.CurrentLang.LanguageCode);
                
            }

            return (name ?? fallback);
        }


        #endregion

        #region ITranslationProvider Members

        /// <summary>
        /// See <see cref="ITranslationProvider.AvailableLanguages" />
        /// </summary>
        private ObservableCollection<CultureInfo> _languages;
        public ObservableCollection<CultureInfo> Languages
        {
            get
            {
                if (_languages == null)
                {
                    LoadLanguages();
                }
                return _languages;
                //var d = new List<CultureInfo>();

                //d.Add(new CultureInfo("en-GB"));
                //d.Add(new CultureInfo("en-US"));
                //d.Add(new CultureInfo("es-ES"));
                //d.Add(new CultureInfo("fr-BE"));
                //d.Add(new CultureInfo("fr-FR"));
                //d.Add(new CultureInfo("it-IT")); 

                //return d;
            }
            set {
                _languages = value;                
            }

        #endregion
        }


        public void LoadLanguages()
        {
            _languages = new ObservableCollection<CultureInfo>(LoginAccess.GetLanguages());
        }
    }
}


 