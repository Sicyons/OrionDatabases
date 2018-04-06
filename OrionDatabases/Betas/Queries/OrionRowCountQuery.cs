using System;
using System.Data;
using System.Text;
using System.Data.Common;
using OrionCore.ErrorManagement;

namespace OrionDatabases.Queries
{
    public class OrionRowCountQuery : OrionQuery
    {
        #region Fields
        private DbDataAdapter xDataAdapter;
        #endregion

        #region Properties
        internal override DbCommand SQLCommand
        {
            get
            {
                return this.xDataAdapter.SelectCommand;
            }
        }
        #endregion

        #region Constructors
        internal OrionRowCountQuery(OrionDatabase parentXDatabase, String tableName, String whereClause = null)
            : base(parentXDatabase)
        {
            StringBuilder strQueryString;

            this.QueryType = QueryTypes.RowCount;

            strQueryString = new StringBuilder("SELECT COUNT(*) FROM " + tableName);

            if (String.IsNullOrWhiteSpace(whereClause) == false)
            {
                whereClause = OrionQuery.TrimWhereClause(whereClause);
                strQueryString.Append(" WHERE " + whereClause);
            }
            this.QueryString = strQueryString.ToString();

            if (this.xDataAdapter == null) this.xDataAdapter = this.ParentOrionDatabase.CreateDataAdapter(this.QueryString);
            this.xDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;
            this.xDataAdapter.SelectCommand.Transaction = this.ParentOrionDatabase.Transaction;
        }// OrionRowCountQuery
        #endregion

        #region Parent interface implementation
        public override Object Execute()
        {
            Int64 lRowCount;
            DataTable xResultTable;
            Exception xException;

            lRowCount = 0L;
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
                    this.Executed = true;
                else
                {
                    this.Failed = true;

                    throw new OrionException("Can't execute DbDataAdapter.Fill(DataTable);", xException, "SqlQuery=" + this.QueryString);
                }

                if (xResultTable.Rows.Count > 0 && xResultTable.Rows[0][0] is DBNull == false)
                    if (Int64.TryParse(xResultTable.Rows[0][0].ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out lRowCount) == false)
                        lRowCount = 0L;
            }

            return lRowCount;
        }// Execute()
        #endregion
    }
}
