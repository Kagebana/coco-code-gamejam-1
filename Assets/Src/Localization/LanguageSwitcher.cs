using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Src.Localization
{
    public class LanguageSwitcher : MonoBehaviour
    {
        public void SetLanguage(Locale locale)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
    }
}