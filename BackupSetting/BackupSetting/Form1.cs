using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BackupSetting.Bl;
using BackupSetting.Interface;
using BackupSetting.Properties;

namespace BackupSetting
{
    public partial class Form1 : Form, ILogEngine
    {
        #region Member

        private readonly AutomationTeamSpeakBl _automationBL;
        private readonly string _configPath;
        private readonly AppSettingBl _appSettingBl;

        #endregion

        #region Constructor

        public Form1()
        {
            var args = Environment.GetCommandLineArgs();
            var appPath = FormatPath(args.Skip(1).FirstOrDefault());
            _configPath = FormatPath(args.Skip(2).FirstOrDefault());

            _automationBL = new AutomationTeamSpeakBl(this, appPath);
            _appSettingBl = AppSettingBl.GetInstance();

            InitializeComponent();
            WriteLog($"configPath : {_configPath}");
            WriteLog($"appPath : {appPath}");
            RestoreSetting();
        }

        #endregion

        #region Implementation of ILogEngine

        #region Property

        public void WriteLog(string message)
        {
            lstLog.Items.Insert(0, message);
        }

        #endregion

        #endregion

        #region  Private Method

        private void ActivateButton()
        {
            btBackup.Enabled = btRestore.Enabled = Directory.Exists(_appSettingBl.BackupFolder);
        }


        private void btBackup_Click(object sender, EventArgs e)
        {
            CopyTo(_configPath, _appSettingBl.BackupFolder);
            _automationBL.ExportIdentities();
        }

        private void btBrowseBackupFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtDestinationFolder.Text))
            {
                txtDestinationFolder.Text = CreateDirectory(txtDestinationFolder.Text);
            }

            folderBrowserDialog.SelectedPath = txtDestinationFolder.Text;
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.Description = Resources.BrowseBackupFolder_Description;
            if (DialogResult.OK == folderBrowserDialog.ShowDialog())
            {
                txtDestinationFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btRestore_Click(object sender, EventArgs e)
        {
            CopyTo(_appSettingBl.BackupFolder, _configPath);
            _automationBL.RestartTeamSpeak();
        }

        private void btSaveConfig_Click(object sender, EventArgs e)
        {
            SaveSetting();
        }

        private void CopyTo(string src, string dest)
        {
            try
            {
                const string db = "settings.db";
                src = Path.Combine(src, db);
                dest = Path.Combine(dest, db);
                File.Copy(src, dest, true);
                WriteLog($"the file « {db} » has been copied to « {dest} »");
            }
            catch (Exception ex)
            {
                WriteLog($"Error : {ex.Message}");
            }
        }

        private string CreateDirectory(string path)
        {
            var result = string.Empty;
            try
            {
                var dirInfo = Directory.CreateDirectory(path);
                result = dirInfo.FullName;
            }
            catch (Exception ex)
            {
                WriteLog($"Error : {ex.Message}");
            }

            return result;
        }

        private static string FormatPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                path = (path + @"\").Replace("/", @"\")
                    .Replace(@"\\", @"\");
            }

            return path;
        }

        private void RestoreSetting()
        {
            txtDestinationFolder.Text = _appSettingBl.BackupFolder;
            txtProcessName.Text = _appSettingBl.ProcessName;
            ActivateButton();
        }

        private void SaveSetting()
        {
            _appSettingBl.BackupFolder = CreateDirectory(txtDestinationFolder.Text);
            _appSettingBl.ProcessName = txtProcessName.Text;
            _appSettingBl.Save();

            txtDestinationFolder.Text = _appSettingBl.BackupFolder;

            ActivateButton();
        }

        #endregion
    }
}