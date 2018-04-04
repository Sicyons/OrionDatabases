using System;
using System.Data;
using System.Data.Common;
using OrionCore.ErrorManagement;

namespace OrionDatabases.Queries
{
    public class OrionSelectQuery : OrionQuery
    {
        #region Fields
        private DbDataAdapter xDataAdapter;
        #endregion

        #region Constructors
        internal OrionSelectQuery(OrionDatabase parentXDatabase, String queryString)
            : base(parentXDatabase)
        {
            String[] strQueryElements;

            strQueryElements = queryString.Split(' ');
            if (strQueryElements.Length == 1)
                // Whole table content loading.
                this.QueryString = "SELECT * FROM " + strQueryElements[0].Trim();
            else
            {
                if (queryString.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase) == true)
                    this.QueryString = queryString;
                else
                    throw new OrionException("The SQL query is not a valid SELECT one;");
            }

            this.QueryType = QueryTypes.Select;

            if (this.xDataAdapter == null) this.xDataAdapter = this.ParentOrionDatabase.CreateDataAdapter(this.QueryString);
            ////this.xDataAdapter.MissingSchemaAction = this.ParentXDatabase.GetTableSchemaWithKey == true ? MissingSchemaAction.AddWithKey : MissingSchemaAction.Add;
            this.xDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;
            this.xDataAdapter.SelectCommand.Transaction = this.ParentOrionDatabase.Transaction;
        }// OrionSelectQuery()
        #endregion

        #region Parent interface implementation
        public override Object Execute()
        {
            DataTable xResultTable;
            Exception xException;

            xException = null;

            using (xResultTable = new DataTable())
            {
                xResultTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

                if (this.ParentOrionDatabase.ConnectionState == ConnectionState.Closed) this.ParentOrionDatabase.Connect();

                try
                {
                    this.xDataAdapter.Fill(xResultTable);
                }
                catch (InvalidOperationException ex)
                {
                    xException = ex;
                }
                catch (DbException ex)
                {
                    xException = ex;
                }
                catch (DataException ex)
                {
                    xException = ex;
                }

                if (this.ParentOrionDatabase.PersistentConnection == false && this.ParentOrionDatabase.TransactionState != TransactionStates.Started)
                    this.ParentOrionDatabase.Disconnect();

                if (xException == null)
                {
                    this.Executed = true;

                    return xResultTable;
                }
                else
                {
                    this.Failed = true;

                    throw new OrionException("Can't execute DbDataAdapter.Fill(DataTable);", xException, "SqlQuery=" + this.QueryString);
                }
            }
        }// Execute()
        #endregion
    }
}