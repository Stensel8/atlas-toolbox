using System;
using System.IO;
using AtlasToolbox.Stores;
using AtlasToolbox.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AtlasToolbox.Services.ConfigurationServices
{
    public class LockScreenConfigurationService : IConfigurationService
    {
        private const string ATLAS_STORE_KEY_NAME = @"HKLM\SOFTWARE\AtlasOS\Services\LockScreen";
        private const string STATE_VALUE_NAME = "state";

        private const string PERSONALIZATION_KEY_NAME = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\Personalization";

        private const string NO_LOCK_SCREEN_VALUE_NAME = "NoLockScreen";
        private const string NO_CHANGING_LOCK_SCREEN_VALUE_NAME = "NoChangingLockScreen";

        private static readonly string LockScreenScriptDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            @"AtlasDesktop\4. Interface Tweaks\Lock Screen"
        );

        private readonly ConfigurationStore _lockScreenConfigurationStore;

        public LockScreenConfigurationService(
            [FromKeyedServices("LockScreen")] ConfigurationStore lockScreenConfigurationStore)
        {
            _lockScreenConfigurationStore = lockScreenConfigurationStore;
        }

        public void Disable()
        {
            RegistryHelper.SetValue(PERSONALIZATION_KEY_NAME, NO_LOCK_SCREEN_VALUE_NAME, 1, Microsoft.Win32.RegistryValueKind.DWord);
            RegistryHelper.SetValue(PERSONALIZATION_KEY_NAME, NO_CHANGING_LOCK_SCREEN_VALUE_NAME, 1, Microsoft.Win32.RegistryValueKind.DWord);
            RegistryHelper.SetValue(ATLAS_STORE_KEY_NAME, STATE_VALUE_NAME, 0);
            RegistryHelper.SetValue(ATLAS_STORE_KEY_NAME, "path", Path.Combine(LockScreenScriptDir, "Hide Lock Screen.ps1"));

            _lockScreenConfigurationStore.CurrentSetting = IsEnabled();
        }

        public void Enable()
        {
            RegistryHelper.DeleteValue(PERSONALIZATION_KEY_NAME, NO_LOCK_SCREEN_VALUE_NAME);
            RegistryHelper.DeleteValue(PERSONALIZATION_KEY_NAME, NO_CHANGING_LOCK_SCREEN_VALUE_NAME);
            RegistryHelper.SetValue(ATLAS_STORE_KEY_NAME, STATE_VALUE_NAME, 1);
            RegistryHelper.SetValue(ATLAS_STORE_KEY_NAME, "path", Path.Combine(LockScreenScriptDir, "Show Lock Screen (default).ps1"));

            _lockScreenConfigurationStore.CurrentSetting = IsEnabled();
        }

        public bool IsEnabled()
        {
            return RegistryHelper.IsMatch(ATLAS_STORE_KEY_NAME, STATE_VALUE_NAME, 1);
        }
    }
}
