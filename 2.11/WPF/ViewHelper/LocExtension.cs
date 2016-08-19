using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace WPF
{
    public class LocExtension : MarkupExtension
    {
        #region Private Members

        private string _key;
        private string _fallback { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="fallback"></param>
        public LocExtension(string key, string fallback)
        {
            _key = key;
            _fallback = fallback;
        }

        public LocExtension(string key)
        {
            _key = key;
            _fallback = "No Fallback";
        }

        #endregion

        [ConstructorArgument("key")]
        public string Key
        {
            get { return _key; }
            set { _key = value;}
        }

        [ConstructorArgument("key")]
        public string Fallback
        {
            get { return _fallback; }
            set { _fallback = value; }
        }

        /// <summary>
        /// See <see cref="MarkupExtension.ProvideValue" />
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
                  {
                      Source = new TranslationData(_key, _fallback)
                  };
            return binding.ProvideValue(serviceProvider);
        }
    }

}
