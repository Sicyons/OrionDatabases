using System;
using System.Data;
using System.Data.Common;
using OrionCore.ErrorManagement;

namespace OrionDatabases.Queries
{
    public class OrionExecuteQuery : OrionQuery
    {
        #region Fields
        private DbCommand m_xCommand;
        #endregion

        #region Properties
        public Int32 ChangedRows { get; private set; }
        #endregion

        #region Constructors
        internal OrionExecuteQuery(OrionDatabase parentXDatabase, String queryString)
            : base(parentXDatabase)
        {
            String[] strQueryElements;

            strQueryElements = queryString.Split(' ');
            if (strQueryElements[0].ToUpperInvariant() != "SELECT")
            {
                this.QueryString = queryString;

                this.QueryType = QueryTypes.Execute;

                this.m_xCommand = this.ParentOrionDatabase.CreateCommand(this.QueryString);
            }
            else
                throw new OrionException("The SQL query should not be a SELECT one;");
        }// OrionExecuteQuery()
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
            if (this.ParentOrionDatabase.ConnectionState == ConnectionState.Closed) this.ParentOrionDatabase.Connect();

            try
            {
                this.ChangedRows = this.m_xCommand.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                throw new OrionException("Can't execute DbCommand.", ex, "Source Request=" + this.QueryString);
            }

            this.Executed = true;

            if (this.ParentOrionDatabase.PersistentConnection == false && this.ParentOrionDatabase.TransactionState != TransactionStates.Started) this.ParentOrionDatabase.Disconnect();

            return this.ChangedRows;
        }// Execute()
        #endregion
    }
}