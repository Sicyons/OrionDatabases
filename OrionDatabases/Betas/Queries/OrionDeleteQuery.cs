using System;
using System.Text;
using System.Data;
using System.Data.Common;
using OrionCore.ErrorManagement;

namespace OrionDatabases.Queries
{
    public class OrionDeleteQuery : OrionQuery
    {
        #region Fields
        private DbCommand m_xCommand;
        #endregion

        #region Properties
        public Int32 ChangedRows { get; private set; }
        #endregion

        #region Constructors
        internal OrionDeleteQuery(OrionDatabase parentXDatabase, String tableName, String whereClause)
            : base(parentXDatabase)
        {
            StringBuilder strQueryString;


            strQueryString = new StringBuilder("DELETE FROM " + tableName);

            if (String.IsNullOrWhiteSpace(whereClause) == false)
            {
                whereClause = OrionQuery.TrimWhereClause(whereClause);
                strQueryString.Append(" WHERE " + whereClause);
            }
            this.QueryString = strQueryString.ToString();

            this.QueryType = QueryTypes.Delete;

            this.m_xCommand = this.ParentOrionDatabase.CreateCommand(this.QueryString);
        }// OrionDeleteQuery()
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
                this.Failed = true;
                xException = ex;
            }

            if (this.ParentOrionDatabase.PersistentConnection == false && this.ParentOrionDatabase.TransactionState != TransactionStates.Started) this.ParentOrionDatabase.Disconnect();

            if (xException == null)
            {
                this.Executed = true;

                return this.ChangedRows;
            }
            else
                throw new OrionException("Can't execute DbCommand.", xException, "Source Request=" + this.QueryString);
        }// Execute()
        #endregion
    }
}
