using System;
using System.Data;
using System.Text;
using System.Data.Common;
using OrionCore.ErrorManagement;

namespace OrionDatabases.Queries
{
    public class OrionInsertQuery : OrionQuery
    {
        #region Fields
        private DbCommand xNewIdCommand;
        #endregion

        #region Properties
        public Int32 ChangedRows { get; private set; }
        public String IdFieldName { get; private set; }
        public String TableName { get; private set; }
        #endregion

        #region Constructors
        internal OrionInsertQuery(OrionDatabase parentXDatabase, String tableName, params String[] fieldNames)
        : base(parentXDatabase)
        {
            StringBuilder strQueryString, strColumns, strValues;

            this.TableName = tableName;

            strQueryString = new StringBuilder("INSERT INTO " + tableName);
            strColumns = new StringBuilder("(");
            strValues = new StringBuilder("VALUES (");

            foreach (String strFieldNameTemp in fieldNames)
            {
                strColumns.Append(strFieldNameTemp + ",");
                //if (this.ParentOrionDatabase is XDatabaseOdbc == false)
                strValues.Append(this.ParentOrionDatabase.ParameterCharacter + strFieldNameTemp + ",");
                //else
                //strValues.Append(this.ParentOrionDatabase.ParameterCharacter + ",");
            }

            strQueryString.Append(" " + strColumns.ToString().Trim(',') + ")");
            strQueryString.Append(" " + strValues.ToString().Trim(',') + ")");
            this.QueryString = strQueryString.ToString();

            this.QueryType = QueryTypes.Insert;

            this.SQLCommand = this.ParentOrionDatabase.CreateCommand(this.QueryString);

            foreach (String strFieldNameTemp in fieldNames)
                this.AddParameter(strFieldNameTemp, null);
        }// OrionInsertQuery()
        internal OrionInsertQuery(OrionDatabase parentXDatabase, String tableName, DataRow sourceRow, params String[] exclusions)
            : base(parentXDatabase)
        {
            Boolean bExcluded;
            StringBuilder strQueryString, strColumns, strValues;

            strQueryString = new StringBuilder("INSERT INTO " + tableName);
            strColumns = new StringBuilder("(");
            strValues = new StringBuilder("VALUES (");

            if (sourceRow != null)
            {
                foreach (DataColumn xColumnTemp in sourceRow.Table.Columns)
                {
                    bExcluded = false;
                    if (exclusions != null)
                        foreach (String strExclusionTemp in exclusions)
                            if (strExclusionTemp == xColumnTemp.ColumnName)
                            {
                                bExcluded = true;
                                break;
                            }

                    if (bExcluded == false)
                    {
                        strColumns.Append(xColumnTemp.ColumnName + ",");
                        strValues.Append(this.ParentOrionDatabase.ParameterCharacter + xColumnTemp.ColumnName + ",");
                    }
                }

                strQueryString.Append(" " + strColumns.ToString().Trim(',') + ")");
                strQueryString.Append(" " + strValues.ToString().Trim(',') + ")");
                this.QueryString = strQueryString.ToString();

                this.QueryType = QueryTypes.Insert;

                this.SQLCommand = this.ParentOrionDatabase.CreateCommand(this.QueryString);
                this.SQLCommand.Transaction = this.ParentOrionDatabase.Transaction;

                foreach (DataColumn xColumnTemp in sourceRow.Table.Columns)
                {
                    bExcluded = false;
                    foreach (String strExclusionTemp in exclusions)
                        if (strExclusionTemp == xColumnTemp.ColumnName)
                        {
                            bExcluded = true;
                            break;
                        }

                    if (bExcluded == false) this.AddParameter(xColumnTemp.ColumnName, sourceRow[xColumnTemp.ColumnName]);
                }
            }
        }// OrionInsertQuery()
        #endregion

        #region Parent interface implementation
        internal override DbCommand SQLCommand { get; }

        public override Object Execute()
        {
            return this.Execute(null);
        }// Execute()
        public Object Execute(String idFieldName)
        {
            Object objNewId;
            Exception xException;

            objNewId = null;
            xException = null;

            if (this.ParentOrionDatabase.ConnectionState == ConnectionState.Closed) this.ParentOrionDatabase.Connect();

            try
            {
                this.ChangedRows = this.SQLCommand.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                xException = ex;
            }

            if (xException == null && String.IsNullOrEmpty(idFieldName) == false)
            {
                this.xNewIdCommand = this.ParentOrionDatabase.CreateCommandGetLastValue(this.TableName, idFieldName);
                objNewId = this.xNewIdCommand.ExecuteScalar();
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

            if (String.IsNullOrEmpty(idFieldName) == true)
                return this.ChangedRows;
            else
                return objNewId;
        }// Execute()
        #endregion
    }
}
