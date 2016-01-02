using System.IO;
using System.Windows.Automation;
using BackupSetting.Interface;
using BackupSetting.Properties;

namespace BackupSetting.Bl
{
    internal class ImportIdentitiesBl : DialogIdentitiesBl
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ImportIdentitiesBl(ILogEngine logEngine) : base(logEngine)
        {
            ResourceDialogBoxTitle = Resources.ImportIdentity;
            ResourceDialogBoxButtonConfirm = Resources.Open;
            ResourceDialogBoxFileEdit = Resources.AutomationFileNameImportId;
            ResourceButtonStartProcess = Resources.AutomationImport;
        }

        #endregion

        #region  Protected Method

        protected override void ConfirmDialogInternal(string fileName)
        {
            // nothing to do
        }

        protected override void Process(AutomationElement root, AutomationElement dialogBox)
        {
            foreach (var file in Directory.GetFiles(AppSettingBl.BackupFolder, "*.ini"))
            {
                ClickOnStartProcessButton(dialogBox);
                ConfirmDialog(root, Path.GetFileNameWithoutExtension(file));
            }

            //Close
            var elBt = dialogBox.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, Resources.AutomationOk));
            ClickOnButton(elBt);
        }

        #endregion
    }
}