using System;
using OrionCore.ErrorManagement;

namespace OrionFiles
{
    public abstract class OrionFiles : IOrionErrorLogManager
    {
        #region Parent method implementations
        public abstract Boolean LogError(StructOrionErrorLogInfos errorLog);
        #endregion
    }
}
