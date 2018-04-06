using System;
using System.Data.Common;
using OrionCore.ErrorManagement;

namespace OrionDatabases.Queries
{
    /// <summary>
    /// Base class used to describe a query.
    /// </summary>
    public abstract class OrionQuery
    {
        #region Properties
        public Boolean Executed { get; internal set; }
        public Boolean Failed { get; protected set; }
        public QueryTypes QueryType { get; protected set; }
        public String QueryString { get; protected set; }
        protected OrionDatabase ParentOrionDatabase { get; }
        public Object this[String key]
        {
            get
            {
                if (this.SQLCommand != null && this.SQLCommand.Parameters.Contains(key) == true)
                    return this.SQLCommand.Parameters[key];
                else
                    return null;
            }
            set
            {
                this.SQLCommand.Parameters[key].Value = value;
            }
        }
        #endregion

        #region Constructors
        protected OrionQuery(OrionDatabase parentOrionDatabase)
        {
            this.ParentOrionDatabase = parentOrionDatabase;
        }// OrionQuery()
        #endregion

        #region Abstract and virtual interface
        internal abstract DbCommand SQLCommand { get; }

        public abstract Object Execute();
        #endregion

        #region Public interface
        public void AddParameter(String key, Object value)
        {
            DbParameter xNewParameter;

            if (String.IsNullOrEmpty(key) == false)
            {
                xNewParameter = this.ParentOrionDatabase.CreateParameter(key, value);
                this.SQLCommand.Parameters.Add(xNewParameter);
            }
            else
            {
                this.Failed = true;
                throw new OrionException("Parameter name must be provided;", "SqlQuery=" + this.QueryString);
            }
        }// AddParameter()
        #endregion

        #region Protected interface
        static protected String TrimWhereClause(String sourceWhereClause)
        {
            if (String.IsNullOrEmpty(sourceWhereClause) == false)
            {
                sourceWhereClause = sourceWhereClause.Trim();
                if (sourceWhereClause.Substring(0, 5).ToUpperInvariant() == "WHERE") sourceWhereClause = sourceWhereClause.Substring(6);
            }

            return sourceWhereClause;
        }// TrimWhereClause()
        #endregion
    }
}
