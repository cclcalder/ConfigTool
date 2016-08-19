using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPF
{
    public class LanguageChangedEventManager : WeakEventManager
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="listener"></param>
        public static void AddListener(TranslationManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="listener"></param>
        public static void RemoveListener(TranslationManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            DeliverEvent(sender, e);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        protected override void StartListening(object source)
        {
            var manager = (TranslationManager)source;
            manager.LanguageChanged += OnLanguageChanged;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        protected override void StopListening(Object source)
        {
            var manager = (TranslationManager)source;
            manager.LanguageChanged -= OnLanguageChanged;
        }

        private static LanguageChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(LanguageChangedEventManager);
                var manager = (LanguageChangedEventManager)GetCurrentManager(managerType);
                if (manager == null)
                {
                    manager = new LanguageChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }

    }

}
