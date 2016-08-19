using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WPF
{
    public interface ITranslationProvider
    {
        /// <summary>
        /// Translates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        object Translate(string key, string fallback);

        /// <summary>
        /// Gets the available languages.
        /// </summary>
        /// <value>The available languages.</value>
        ObservableCollection<CultureInfo> Languages { get; set; }




        void LoadLanguages();
    }
}
