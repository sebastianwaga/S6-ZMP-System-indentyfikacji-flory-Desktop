using System;
using System.Linq;
using System.Windows;

namespace VirtualHerbarium.AdminPanel.Helpers
{
    public static class LanguageManager
    {
        public static void SetLanguage(string lang)
        {
            var app = Application.Current;

            var langDict = app.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null &&
                                     d.Source.OriginalString.Contains("Strings"));

            if (langDict == null)
                return;

            switch (lang)
            {
                case "en":
                    langDict.Source = new Uri("Localization/Strings.en.xaml", UriKind.Relative);
                    break;

                default:
                    langDict.Source = new Uri("Localization/Strings.pl.xaml", UriKind.Relative);
                    break;
            }
        }
    }
}
