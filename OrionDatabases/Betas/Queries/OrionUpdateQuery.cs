using System;
using System.Data;
using System.Text;
using System.Data.Common;
using OrionCore.ErrorManagement;

namespace OrionDatabases.Queries
{
    public class OrionUpdateQuery : OrionQuery
    {
        #region Fields
        private DbCommand m_xCommand;
        #endregion

        #region Properties
        public Int32 ChangedRows { get; private set; }
        #endregion

        #region Constructors
        internal OrionUpdateQuery(OrionDatabase parentOrionDatabase, String tableName, String whereClause, params String[] fieldNames)
            : base(parentOrionDatabase)
        {
            StringBuilder strQueryString, strSet;

            strQueryString = new StringBuilder("UPDATE " + tableName);
            strSet = new StringBuilder(" SET ");

            foreach (String strFieldNameTemp in fieldNames)
            {
                strSet.Append(strFieldNameTemp + "=");
                strSet.Append(this.ParentOrionDatabase.ParameterCharacter + strFieldNameTemp + ",");
            }

            strQueryString.Append(strSet.ToString().Trim(','));
            if (String.IsNullOrWhiteSpace(whereClause) == false)
            {
                whereClause = OrionQuery.TrimWhereClause(whereClause);
                strQueryString.Append(" WHERE " + whereClause);
            }
            this.QueryString = strQueryString.ToString();

            this.QueryType = QueryTypes.Update;

            this.m_xCommand = this.ParentOrionDatabase.CreateCommand(this.QueryString);

            foreach (String strFieldNameTemp in fieldNames)
                this.AddParameter(strFieldNameTemp, null);
        }// OrionUpdateQuery()
        #endregion

        #region Parent interface implementation
        internal override DbCommand SQLCommand
        {
            get
            {
                return this.m_xCommand;
            }
        }

        public override Object Execute()
        {
            Exception xException;

            xException = null;

            if (this.ParentOrionDatabase.ConnectionState == ConnectionState.Closed) this.ParentOrionDatabase.Connect();

            try
            {
                this.ChangedRows = this.m_xCommand.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                xException = ex;
            }

            if (xException == null)
            {
                this.Executed = true;

                if (this.ParentOrionDatabase.PersistentConnection == false && this.ParentOrionDatabase.TransactionState != TransactionStates.Started) this.ParentOrionDatabase.Disconnect();
            }
            else
            {
                this.Failed = true;
                this.ParentOrionDatabase.Disconnect();

                throw new OrionException("Can't execute DbCommand.ExecuteNonQuery();", xException, "SqlQuery=" + this.QueryString);
            }

            return this.ChangedRows;
        }// Execute()
        #endregion
    }
}