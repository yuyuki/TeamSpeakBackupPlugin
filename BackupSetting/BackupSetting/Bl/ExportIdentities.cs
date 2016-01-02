using System.IO;
using System.Linq;
using System.Windows.Automation;
using BackupSetting.Interface;
using BackupSetting.Properties;

namespace BackupSetting.Bl
{
    internal class ExportIdentitiesBl : DialogIdentitiesBl
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ExportIdentitiesBl(ILogEngine logEngine) : base(logEngine)
        {
            ResourceDialogBoxTitle = Resources.ExportIdentity;
            ResourceDialogBoxButtonConfirm = Resources.Save;
            ResourceDialogBoxFileEdit = Resources.AutomationFileNameExportId;
            ResourceButtonStartProcess = Resources.AutomationExport;
        }

        #endregion

        #region  Protected Method

        protected override void ConfirmDialogInternal(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        protected override void Process(AutomationElement root, AutomationElement dialogBox)
        {
            var lst = dialogBox?.FindFirst(TreeScope.Subtree,
                                           new PropertyCondition(
                                               AutomationElement.ControlTypeProperty,
                                               ControlType.List));
            if (lst != null)
            {
                var items = lst.FindAll(TreeScope.Children, Condition.TrueCondition)
                    ?.Cast<AutomationElement>();

                if (items != null)
                {
                    foreach (var profile in items)
                    {
                        ClickOnButton(profile);
                        ClickOnStartProcessButton(dialogBox);
                        SkipWarning(dialogBox);
                        ConfirmDialog(root, profile.Current.Name);
                    }
                    
                }
                //Close
                var elBt = dialogBox.FindFirst(TreeScope.Subtree, new PropertyCondition(
                                                                      AutomationElement.NameProperty,
                                                                      Resources.AutomationOk));
                ClickOnButton(elBt);
            }
        }

        #endregion

        #region  Private Method

        /// <summary>
        ///     Skip warning message
        /// </summary>
        /// <param name="el"></param>
        private void SkipWarning(AutomationElement el)
        {
            var warning = el.FindFirst(TreeScope.Subtree,
                                       new PropertyCondition(AutomationElement.NameProperty, Resources.AutomationWarning));
            if (warning != null)
            {
                var elBts = warning.FindAll(TreeScope.Subtree, Condition.TrueCondition);
                var elBt = elBts.Cast<AutomationElement>()
                    .FirstOrDefault(f => f.Current.Name.Contains(Resources.AutomationWarningYes));

                ClickOnButton(elBt);
            }
        }

        #endregion
    }
}