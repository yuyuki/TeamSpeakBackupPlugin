using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using BackupSetting.Interface;
using BackupSetting.Properties;

namespace BackupSetting.Bl
{
    internal abstract class DialogIdentitiesBl
    {
        #region Member

        protected readonly AppSettingBl AppSettingBl;

        protected ILogEngine LogEngine;
        protected string ResourceButtonStartProcess;
        protected string ResourceDialogBoxButtonConfirm;
        protected string ResourceDialogBoxFileEdit;
        protected string ResourceDialogBoxTitle;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected DialogIdentitiesBl(ILogEngine logEngine)
        {
            LogEngine = logEngine;
            AppSettingBl = AppSettingBl.GetInstance();
        }

        #endregion

        #region  Public Method

        public void StartProcess()
        {
            var root = FocusOnTeamSpeak();
            LogEngine.WriteLog("Display the identitiy dialogBox");
            DisplayIdentitiesDialogBox(root);
        }

        #endregion

        #region  Protected Method

        protected void ClickOnButton(AutomationElement elBt)
        {
            try
            {
                var invoke = elBt?.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                invoke?.Invoke();
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog($"Click on button {elBt?.Current.Name} with error : {ex.Message}");
            }
        }

        /// <summary>
        ///     Click on Import/Export button
        /// </summary>
        /// <param name="dialogBox"></param>
        protected void ClickOnStartProcessButton(AutomationElement dialogBox)
        {
            var elBt = dialogBox.FindFirst(TreeScope.Children,
                                           new PropertyCondition(AutomationElement.NameProperty,
                                                                 ResourceButtonStartProcess));

            ClickOnButton(elBt);
        }

        protected void ConfirmDialog(AutomationElement root, string profileName)
        {
            var fileName = GetFileName(profileName);

            LogEngine.WriteLog($"{ResourceButtonStartProcess} {fileName}");

            ConfirmDialogInternal(fileName);

            // Load dialogbox and all subtree in memory
            root.FindAll(TreeScope.Subtree,
                         new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));


            root = root.FindFirst(TreeScope.Subtree,
                                  new PropertyCondition(AutomationElement.NameProperty, ResourceDialogBoxTitle));

            EnterDirectoryName(root, fileName);
            var elBt = root?.FindFirst(TreeScope.Children,
                                       new PropertyCondition(AutomationElement.NameProperty,
                                                             ResourceDialogBoxButtonConfirm));
            ClickOnButton(elBt);
        }

        protected abstract void ConfirmDialogInternal(string fileName);

        protected void EnterDirectoryName(AutomationElement root, string profileName)
        {
            // Export
            var el = root?.FindFirst(
                TreeScope.Subtree,
                new PropertyCondition(AutomationElement.AutomationIdProperty,
                                      ResourceDialogBoxFileEdit));

            var valuePattern = el?.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            valuePattern?.SetValue(profileName);
        }


        protected abstract void Process(AutomationElement root, AutomationElement dialogBox);

        #endregion

        #region  Private Method

        private void DisplayIdentitiesDialogBox(AutomationElement root)
        {
            var dialogBox = root?.FindFirst(
                TreeScope.Subtree,
                new PropertyCondition(AutomationElement.NameProperty,
                                      Resources.AutomationIdentities));
            if (dialogBox == null)
            {
                LogEngine.WriteLog("The DialogBox Identities is not detected.");
                return;
            }
            Process(root, dialogBox);
        }

        private AutomationElement FocusOnTeamSpeak()
        {
            LogEngine.WriteLog("Detection of TeamSpeak");
            AutomationElement root = null;
            for (var i = 0; i < 3 || root == null; i++)
            {
                root = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                                                               new PropertyCondition(
                                                                   AutomationElement.NameProperty,
                                                                   Resources.AutomationName));
                if (root == null)
                {
                    LogEngine.WriteLog("Impossible to detect Team Speak 3. We try to launch it.");
                    RestartTeamSpeak();
                }
            }

            try
            {
                // Focus on app
                LogEngine.WriteLog("Focus on TeamSpeak");
                root.SetFocus();

                // Wait Focus
                Thread.Sleep(500);

                // Display Identities dialogbox
                LogEngine.WriteLog("Send key for displaying Identities dialog");
                SendKeys.SendWait(Resources.AutomationIdentitiesShortcut);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ex.Message);
            }
            return root;
        }

        private string GetFileName(string profileName)
        {
            var fileName = Path.Combine(AppSettingBl.BackupFolder, $"{profileName}.ini");
            return fileName;
        }

        private void RestartTeamSpeak()
        {
            var process = System.Diagnostics.Process.GetProcessesByName(AppSettingBl.ProcessName);
            if (process.Any())
            {
                foreach (var p in process)
                {
                    p.Kill();
                    LogEngine.WriteLog($"Kill the process {p.ProcessName} with Pid {p.Id}");
                }
            }

            LogEngine.WriteLog("Start a new instance of TeamSpeak");
            System.Diagnostics.Process.Start(
                Path.Combine(AppSettingBl.TeamSpeakPath, $"{AppSettingBl.ProcessName}.exe"));

            Thread.Sleep(1000);
        }

        #endregion
    }
}