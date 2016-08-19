using System;
using System.Windows.Data;
using System.Windows.Markup;
using Model.Entity;

namespace WPF
{
    public class ConfigExtension : MarkupExtension
    {
        public ConfigExtension(string key)
        {
            _key = key;
        }

        private string _key;

        [ConstructorArgument("key")]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
            {
                Source = new FeatureConfigData(_key)
            };
            return binding.ProvideValue(serviceProvider);
        }
    }

    public class FeatureConfigData
    {
        public FeatureConfigData(string key)
        {
            _key = key;
        }

        private readonly string _key;

        public object Value
        {
            get
            {
                return ClientConfiguration.IsFeatureVisible(_key);
            }
        }
    }
}