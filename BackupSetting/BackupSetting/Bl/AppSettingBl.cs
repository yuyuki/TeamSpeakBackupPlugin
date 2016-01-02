using System.Configuration;
using System.Linq;

namespace BackupSetting.Bl
{
    public class AppSettingBl
    {
        #region Member

        private static AppSettingBl _instance;
        private readonly Configuration _config;

        #endregion

        #region Property

        public string BackupFolder
        {
            get { return GetValue(nameof(BackupFolder)); }
            set { SetValue(nameof(BackupFolder), value); }
        }

        public string ProcessName
        {
            get { return GetValue(nameof(ProcessName)); }
            set { SetValue(nameof(ProcessName), value); }
        }

        public string TeamSpeakPath { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        private AppSettingBl()
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        #endregion

        #region  Public Method

        public static AppSettingBl GetInstance()
        {
            return _instance ?? (_instance = new AppSettingBl());
        }

        public void Save()
        {
            _config.Save(ConfigurationSaveMode.Full);
        }

        #endregion

        #region  Private Method

        private string GetValue(string key)
        {
            if (!_config.AppSettings.Settings.AllKeys.Contains(key))
            {
                _config.AppSettings.Settings.Add(key, null);
            }

            return _config.AppSettings.Settings[key].Value;
        }

        private void SetValue(string key, string value)
        {
            if (!_config.AppSettings.Settings.AllKeys.Contains(key))
            {
                _config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                _config.AppSettings.Settings[key].Value = value;
            }
        }

        #endregion
    }
}