using System;
using System.Diagnostics;
using Windows.Security.Credentials;
using Windows.Storage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BGGApp.Helpers
{
    public abstract class VaultManager
    {
        public  const string ResourceName = "BGGAPP";

        public static void Initialize()
        {
            VaultManager.LoadPasswordVault();
        }

        private static Windows.Security.Credentials.PasswordVault vault;
        private static PasswordCredential credential;
        
        [DebuggerHidden]
        private static void LoadPasswordVault()
        {
            // any call to the password vault will load the vault 
            vault = new Windows.Security.Credentials.PasswordVault();
            try
            {
                IReadOnlyList<PasswordCredential> credentials = vault.FindAllByResource(ResourceName);
                credential = credentials.FirstOrDefault();
                if (string.IsNullOrEmpty(credential.Password))
                    credential.RetrievePassword();
                _Password = credential.Password;
                _Username = credential.UserName;
            }
            catch (Exception)
            {
               // If the credentials is empty the PasswordVault will throw an unspecific Exception (WEIRD...)
            }
        }

        private static string _Password;
        public static string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (value != Password)
                {
                    _Password = value;
                    Save();
                }
            }
        }
        private static string _Username;
        public static string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                if (value != Username)
                {
                    _Username = value;
                    Save();
                }
            }
        }

        private static void Save()
        {
            if(credential != null)
                vault.Remove(credential);

            credential = new PasswordCredential();

            if (!string.IsNullOrEmpty(Username))
            {
                credential.UserName = Username;
            }
            else
            {
                credential.UserName = "BoardGameGeek"; // Default user
            }

            if (!string.IsNullOrEmpty(Password))
            {
                credential.Password = Password;
            }
            else
            {
                credential.Password = "tobeimplemented";
            }
            credential.Resource = ResourceName;

            vault.Add(credential);
        }
    } 

    class AppSettings
    {

        private static AppSettings _Singleton;
        public static AppSettings Singleton
        {
            get
            {
                if (_Singleton == null)
                    _Singleton = new AppSettings();

                return _Singleton;
            }
        }

        // Our isolated storage settings
        //IsolatedStorageSettings isolatedStore;
        ApplicationDataContainer localSettings;
        // The isolated storage key names of our settings
        

        // The default value of our settings
        const string UserNameDefault = "BoardGameGeek";
        const string UserPasswordDefault = "";
        
        const string VAULT_RESOURCE = "[BGGApp] Credentials";
        

        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        private AppSettings()
        {
            try
            {
                localSettings = ApplicationData.Current.LocalSettings;
                VaultManager.Initialize(); 
            }
            catch (Exception)
            {
                //Todo Elmah (?)
            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            //if (localSettings.Contains(Key))
            if (localSettings.Values.ContainsKey(Key))
            {
                // If the value has changed
                if (localSettings.Values[Key] != value)
                {
                    // Store the new value
                    localSettings.Values[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                localSettings.Values.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (localSettings.Values.ContainsKey(Key))
            {
                value = (T)localSettings.Values[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }

            return value;
        }

        public string UserNameSetting
        {
            get
            {
                return VaultManager.Username;
            }
            set
            {
                VaultManager.Username = value;
            }
        }


        
        public string UserPasswordSetting
        {
            get
            {
                return VaultManager.Password;
            }
            set
            {
                VaultManager.Password = value;
            }
        }

    }
}




