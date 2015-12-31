using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using BackupSetting.Interface;
using BackupSetting.Properties;

namespace BackupSetting.Bl
{
    internal sealed class AutomationTeamSpeakBl
    {
        #region Member

        private readonly string _appPath;
        private readonly ILogEngine _logEngine;
        private enum Direction
        {
            Export,
            Import
        }

        #endregion

        #region Constructor

        public AutomationTeamSpeakBl(ILogEngine logEngine, string appPath)
        {
            _appPath = appPath;
            _logEngine = logEngine;
        }

        #endregion

        #region  Public Method

        public void ExportIdentities()
        {
            Automation(Direction.Export);
        }

        public void RestartTeamSpeak()
        {
            var process = Process.GetProcessesByName(Settings.Default.ProcessName);
            if (process.Any())
            {
                foreach (var p in process)
                {
                    p.Kill();
                }
            }
            Process.Start(Path.Combine(_appPath, $"{Settings.Default.ProcessName}.exe"));

            Thread.Sleep(1000);
        }

        #endregion

        #region  Private Method

        private void Automation(Direction direction)
        {
            var root = FocusOnTeamSpeak();
            DisplayIdentitiesDialogBox(direction, root);
        }

        private void ClickOnButton(AutomationElement elBt)
        {
            try
            {
                var invoke = elBt?.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                invoke?.Invoke();
            }
            catch (Exception ex)
            {
                _logEngine.WriteLog(ex.Message);
            }
        }

        private void ConfirmDialog(AutomationElement root, Direction direction, string profileName)
        {
            var fileName = GetFileName(profileName);
            if (direction == Direction.Export && File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // Load dialogbox in memory
            root.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));

            root = root?.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, Resource.ExportIdentity));
            EnterDirectoryName(root, fileName);
            var elBt = root?.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, Resource.Save));
            ClickOnButton(elBt);
        }

        private void DisplayIdentitiesDialogBox(Direction direction, AutomationElement root)
        {
            var dialogBox = root?.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, Settings.Default.AutomationIdentities));
            var lst = dialogBox?.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List));
            if (lst != null)
            {
                var items = lst.FindAll(TreeScope.Children, Condition.TrueCondition)
                    ?.Cast<AutomationElement>();
                AutomationElement elBt;
                foreach (var profile in items)
                {
                    ClickOnButton(profile);

                    var btName = direction == Direction.Export ? Settings.Default.AutomationExport : Settings.Default.AutomationImport;
                    elBt = dialogBox.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, btName));
                    if (elBt == null)
                    {
                        _logEngine.WriteLog($"Button {btName} not detected");
                    }
                    else
                    {
                        ClickOnButton(elBt);
                        SkipWarning(dialogBox);
                        ConfirmDialog(root, direction, profile.Current.Name);
                    }
                }

                //Close
                elBt = dialogBox?.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, Resource.AutomationOk));
                ClickOnButton(elBt);
            }
        }

        private void EnterDirectoryName(AutomationElement root, string profileName)
        {
            var el = root?.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, Resource.AutomationFileNameId));
            var valuePattern = el?.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            valuePattern?.SetValue(profileName);
        }

        private static string GetFileName(string profileName)
        {
            var fileName = Path.Combine(Settings.Default.BackupFolder, $"{profileName}.ini");
            return fileName;
        }

        private AutomationElement FocusOnTeamSpeak()
        {
            AutomationElement root = null;
            for (var i = 0; i < 3 || root == null; i++)
            {
                root = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                                                               new PropertyCondition(
                                                                   AutomationElement.NameProperty,
                                                                   Settings.Default.AutomationName));
                if (root == null)
                {
                    _logEngine.WriteLog("Impossible to detect Team Speak 3. We try to launch it.");
                    RestartTeamSpeak();
                }
            }

            try
            {
                // Focus on app
                root.SetFocus();

                // Wait Focus
                Thread.Sleep(500);

                // Display Identities dialogbox
                SendKeys.SendWait(Settings.Default.AutomationIdentitiesShortcut);
            }
            catch (Exception ex)
            {
                _logEngine.WriteLog(ex.Message);
            }
            return root;
        }

        /// <summary>
        ///     Skip warning message
        /// </summary>
        /// <param name="el"></param>
        private void SkipWarning(AutomationElement el)
        {
            var warning = el.FindFirst(TreeScope.Subtree,
                                       new PropertyCondition(AutomationElement.NameProperty, Settings.Default.AutomationWarning));
            if (warning != null)
            {
                var elBts = warning.FindAll(TreeScope.Subtree, Condition.TrueCondition);
                var elBt = elBts.Cast<AutomationElement>()
                    .FirstOrDefault(f => f.Current.Name.Contains(Settings.Default.AutomationWarningYes));

                ClickOnButton(elBt);
            }
        }

        #endregion
    }
}