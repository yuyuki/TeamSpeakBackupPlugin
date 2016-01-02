using BackupSetting.Interface;

namespace BackupSetting.Bl
{
    internal sealed class AutomationTeamSpeakBl
    {
        #region Member

        private readonly ILogEngine _logEngine;

        #endregion

        #region Constructor

        public AutomationTeamSpeakBl(ILogEngine logEngine)
        {
            _logEngine = logEngine;
        }

        #endregion

        #region  Public Method

        public void ExportIdentities()
        {
            var p = new  ExportIdentitiesBl(_logEngine);
            p.StartProcess();
        }

        public void ImportIdentities()
        {
            var p = new ImportIdentitiesBl(_logEngine);
            p.StartProcess();
        }

        #endregion
    }
}