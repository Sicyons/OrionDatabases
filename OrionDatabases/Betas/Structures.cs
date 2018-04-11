using System;

namespace OrionDatabases
{
    #region Structures
    public struct LogConfigurationInfos
    {
        #region Properties
        public Boolean IsInitialized { get; private set; }
        public String Comment1FieldName { get; private set; }
        public String Comment2FieldName { get; private set; }
        public String DateFieldName { get; private set; }
        public String MessageFieldName { get; private set; }
        public String SourceApplicationFieldName { get; private set; }
        public String TableName { get; private set; }
        public String TypeLogFieldName { get; private set; }
        #endregion

        #region Constructors
        internal LogConfigurationInfos(String tableName, String dateFieldName, String sourceApplicationFieldName, String typeLogFieldName, String messageFieldName, String comment1FieldName, String comment2FieldName)
        {
            this.IsInitialized = true;
            this.TableName = tableName;
            this.DateFieldName = dateFieldName;
            this.MessageFieldName = messageFieldName;
            this.SourceApplicationFieldName = sourceApplicationFieldName;
            this.TypeLogFieldName = typeLogFieldName;
            this.Comment1FieldName = comment1FieldName;
            this.Comment2FieldName = comment2FieldName;
        }// LogConfigurationInfos()
        #endregion
    }
    #endregion
}
