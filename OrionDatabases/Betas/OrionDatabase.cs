using System;
using System.IO;
using System.Data;
using System.Text;
using System.Threading;
using System.Data.Common;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OrionCore.ErrorManagement;
using OrionCore.LogManagement;
using OrionDatabases.Queries;
using OrionCore;

namespace OrionDatabases
{
    /// <summary>Base class for database connections and data management derived classes. This class is abstract.</summary>
    /// <remarks >For example, <b>OrionDatabase</b> is the abstract base class of <see cref="OrionDatabaseSQLite" /> or <see cref="OrionDatabasePostgreSql"/>, which manages communication with SQLite or PostgreSql databases.
    /// <br />Any opened connection is automatically closed before disposing <see cref="OrionDatabase" /> object.</remarks>
    /// <seealso cref="OrionDatabaseSQLite" />
    /// <seealso cref = "OrionDatabasePostgreSql" />
    public abstract class OrionDatabase : IOrionLogManager, IDisposable
    {
        #region Fields
        private Boolean bDisposed;
        private ObservableCollection<OrionQuery> xQueries;
        #endregion

        #region Properties
        /// <summary>
        /// Enables persistent connection to the database.
        /// </summary>
        /// <remarks>If value is <i>true</i>, opened connection to the database is not closed after each SQL request. So the proper way to close it is to call the <see cref="Disconnect" /> method. If value is <i>false</i>, connection is automatically closed after each method call which execute a SQL request. It is the default <see cref="OrionDatabase" /> behavior.
        /// <br />Any opened connection is automatically closed before disposing <see cref="OrionDatabase" /> object.</remarks>
        /// <value>Type : <see cref="Boolean"/>
        /// <br/>Persistence of the connection. Default value is <i>false</i>.</value>
        /// <see cref="Disconnect" />
        public Boolean PersistentConnection { get; set; }
        /// <summary>
        /// Gets the character used to specify parameter names in Sql queries.
        /// </summary>
        /// <value>Type : <see cref="Char"/>
        /// <br/>The parameter character.</value>
        public Char ParameterCharacter { get; set; }
        /// <summary>
        /// Gets a value that describes the current state of the connection.
        /// </summary>
        /// <value>Type : <see cref="ConnectionState"/>
        /// <br/>The current state of the connection.</value>
        /// <seealso cref="ConnectionState"/>
        public ConnectionState ConnectionState
        {
            get
            {
                return this.Connection != null ? this.Connection.State : ConnectionState.Closed;
            }
        }
        /// <summary>
        /// Gets current transaction state.
        /// </summary>
        /// <value>Type : <see cref="TransactionStates"/>
        /// <br/>State of the current connection. Default value is <i>None</i>.</value>
        public TransactionStates TransactionState { get; private set; }
        public LogConfigurationInfos LogInfoConfig { get; private set; }
        public LogConfigurationInfos LogErrorConfig { get; private set; }
        /// <summary>
        /// Connection string used to establish connection with the target database.
        /// </summary>
        /// <value>Type : <see cref="String"/>
        /// <br/>The connection string used to establish the connection.</value>
        public String ConnectionString
        {
            get
            {
                return this.ConnectionStringBuilder.ConnectionString;
            }
        }
        public DbConnection Connection { get; set; }
        /// <summary>
        /// Gets last created <see cref="OrionQuery"/> object if existing.
        /// </summary>
        /// <value>Type : <see cref="OrionQuery"/>
        /// <br/>The current query object.</value>
        /// <seealso cref="OrionQuery"/> 
        /// <seealso cref="OrionDeleteQuery"/> 
        /// <seealso cref="OrionExecuteQuery"/> 
        /// <seealso cref="OrionInsertQuery"/> 
        /// <seealso cref="OrionSelectQuery"/>
        /// <seealso cref="OrionUpdateQuery"/> 
        public OrionQuery CurrentQuery
        {
            get
            {
                return this.xQueries != null && this.xQueries.Count > 0 ? this.xQueries[this.xQueries.Count - 1] : null;
            }
        }

        /// <exclude />
        internal DbTransaction Transaction { get; set; }
        /// <exclude />
        internal DbConnectionStringBuilder ConnectionStringBuilder { get; set; }
        #endregion

        #region Constructors
        protected OrionDatabase()
        {
        }// OrionDatabase()
        #endregion
        #region Destructors
        /// <exclude />
        ~OrionDatabase()
        {
            this.Dispose(false);
        }// ~OrionDatabase()
        #endregion

        #region Interface implementations
        /// <summary>
        /// Releases all resources used by this <see cref="OrionDatabase" /> object.
        /// </summary>
        /// <remarks>Calling <b>Dispose</b> allows the resources used by this <see cref="OrionDatabase" /> object to be reallocated for other purposes.
        /// <br />If a transaction has been started and is not committed yet, <see cref="RollbackTransaction()"/> method is called before disposing the <see cref="OrionDatabase" /> object</remarks>
        /// <seealso cref="OrionDatabase" />
        /// <seealso cref="RollbackTransaction()"/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }// Dispose()
        public virtual Boolean SaveLog(OrionLogInfos logInfos)
        {
            List<String> strFieldNames;
            OrionInsertQuery xQuery;
            LogConfigurationInfos xLogInfos;

            xLogInfos = new LogConfigurationInfos();

            switch (logInfos.EventType)
            {
                case OrionCore.EventManagement.EventTypes.Information:
                    xLogInfos = this.LogInfoConfig;

                    if (xLogInfos.IsInitialized == false)
                        throw new OrionException("OrionDatatable information log have to be initialized;");

                    break;
                case OrionCore.EventManagement.EventTypes.Warning:
                case OrionCore.EventManagement.EventTypes.Error:
                case OrionCore.EventManagement.EventTypes.CriticalError:
                    xLogInfos = this.LogErrorConfig;

                    if (xLogInfos.IsInitialized == false)
                        throw new OrionException("OrionDatatable error log have to be initialized;");
                    break;
                default:
                    xLogInfos = new LogConfigurationInfos();
                    break;
            }

            strFieldNames = new List<String>(new String[] { xLogInfos.DateFieldName, xLogInfos.SourceApplicationFieldName, xLogInfos.TypeLogFieldName, xLogInfos.MessageFieldName });
            if (String.IsNullOrEmpty(xLogInfos.Comment1FieldName) == false) strFieldNames.Add(xLogInfos.Comment1FieldName);
            if (String.IsNullOrEmpty(xLogInfos.Comment2FieldName) == false) strFieldNames.Add(xLogInfos.Comment2FieldName);

            xQuery = this.PrepareQueryInsert(xLogInfos.TableName, strFieldNames.ToArray());
            xQuery.AddParameter(this.ParameterCharacter + xLogInfos.DateFieldName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture));
            xQuery.AddParameter(this.ParameterCharacter + xLogInfos.SourceApplicationFieldName, logInfos.SourceApplicationName);
            xQuery.AddParameter(this.ParameterCharacter + xLogInfos.TypeLogFieldName, logInfos.EventType.ToString());
            xQuery.AddParameter(this.ParameterCharacter + xLogInfos.MessageFieldName, logInfos.LogMessage.ToString());
            if (String.IsNullOrEmpty(xLogInfos.Comment1FieldName) == false) xQuery.AddParameter(this.ParameterCharacter + xLogInfos.Comment1FieldName, logInfos.Comment1.ToString());
            if (String.IsNullOrEmpty(xLogInfos.Comment2FieldName) == false) xQuery.AddParameter(this.ParameterCharacter + xLogInfos.Comment2FieldName, logInfos.Comment2.ToString());
            xQuery.Execute();

            return true;
        }// SaveLog()
        #endregion

        #region Virtual and abstract interfaces
        /// <exclude />
        internal abstract DbCommand CreateCommand(String xSqlRequest);
        /// <exclude />
        internal abstract DbDataAdapter CreateDataAdapter(String sqlRequest);
        /// <exclude />
        internal abstract DbParameter CreateParameter(String key, Object value);
        #endregion

        #region Protected interface
        static protected void CheckMissingDll(String dllFileName)
        {
            String strDllFilePath;

            if (String.IsNullOrWhiteSpace(dllFileName) == false)
            {
                strDllFilePath = Path.Combine(OrionDeploymentInfos.DataFolder, dllFileName);
                if (File.Exists(strDllFilePath) == false) throw new OrionException("Missing dll [" + dllFileName + "];", "DllFilePath=" + strDllFilePath);
            }
        }// CheckMissingDll()

        protected void Initialize()
        {
            this.xQueries = new ObservableCollection<OrionQuery>();
        }//  Initialize()
        #endregion

        #region Public interface
        /// <summary>
        /// Commits the database pending transaction.
        /// </summary>
        /// <exception cref="OrionException">There is no active transaction, or the transaction has already been committed or rolled back or the connection is broken.</exception>
        /// <seealso cref="OrionException" />
        public void CommitTransaction()
        {
            if (this.Transaction != null)
            {
                try
                {
                    this.Transaction.Commit();
                }
                catch (DbException ex)
                {
                    throw new OrionException("Error while trying to commit transaction;", ex);
                }

                this.TransactionState = TransactionStates.Committed;

                this.Transaction.Dispose();
                this.Transaction = null;
            }
            else
                throw new OrionException("No transaction has been started.");

            this.xQueries.Clear();

            if (this.Connection.State != ConnectionState.Closed && this.PersistentConnection == false) this.Disconnect();
        }// CommitTransaction()
        /// <summary>
        /// Establishes a connection to the source database.
        /// </summary>
        /// <remarks>Three attempts will be made before throwing an exception if connection can't be established, with a delay of one second between each of them.
        /// <br />If connection is on <i>Broken</i> state, the <see cref="OrionDatabase" /> will try to properly close it, wait for one second and then establish a new connection.</remarks>
        /// <exception cref="OrionException">No connection has been initialized.</exception>
        /// <seealso cref="OrionException" />
        public void Connect()
        {
            Int32 iAttemptCounter;

            if (this.Connection != null)
            {
                //** If connection state is Broken, try to disconnect **
                if (this.Connection.State == ConnectionState.Broken || this.Connection.State == ConnectionState.Connecting)
                {
                    this.Disconnect();

                    Thread.Sleep(1000);
                }

                //** Try to connect to the database (3 attempts) **
                iAttemptCounter = 0;
                while (this.Connection.State != ConnectionState.Open && iAttemptCounter < 3)
                {
                    try
                    {
                        this.Connection.Open();
                        break;
                    }
                    catch (DbException ex)
                    {
                        iAttemptCounter++;

                        Thread.Sleep(1000);

                        if (this.Connection.State != ConnectionState.Open && iAttemptCounter == 2)
                            throw new OrionException("Error while connecting to database (3 attempts)", ex, "ConnectionString=" + this.ConnectionStringBuilder.ToString());
                    }
                }
            }
            else
                throw new OrionException("Connection has not been correctly initialized.");
        }// Connect()
        /// <summary>
        /// Disconnects from the source database.
        /// </summary>
        /// <remarks>Three attempts will be made before throwing an exception if connection can't be disconnected, with a delay of one second between each of them.
        /// <br />If a transaction is started and has not been committed, <see cref="RollbackTransaction()" /> method is called before disconnecting the <see cref="OrionDatabase" /> object.
        /// <br />If <see cref="PersistentConnection" /> property value is <i>true</i>, it is automatically set back to <i>false</i>.</remarks>
        /// <exception cref="OrionException">No connection has been initialized.</exception>
        /// <seealso cref="OrionException" />
        public void Disconnect()
        {
            Int32 iAttemptCounter;

            if (this.Connection != null)
            {
                this.PersistentConnection = false;

                //** Try to disconnect from the database (3 attempts) **
                iAttemptCounter = 0;
                while (this.Connection.State != ConnectionState.Closed && iAttemptCounter < 3)
                {
                    try
                    {
                        if (this.Transaction != null) this.RollbackTransaction();
                        this.Connection.Close();

                        break;
                    }
                    catch (DbException ex)
                    {
                        iAttemptCounter++;

                        Thread.Sleep(1000);

                        if (this.Connection.State != ConnectionState.Closed && iAttemptCounter == 3) throw new OrionException("Error while disconnecting from database (3 attempts)", ex);
                    }
                }
            }
            else
                throw new OrionException("No connection has been initialized");
        }// Disconnect()
        public void InitLogs(String tableName = "T_Logs", String dateFieldName = "CreationDate", String sourceApplicationFieldName = "SourceApplication", String logTypeFieldName = "Type", String messageFieldName = "Message", String comment1FieldName = null, String comment2FieldName = null, Boolean createMissingTable = false)
        {
            this.InitInformationLogs(tableName, dateFieldName, sourceApplicationFieldName, logTypeFieldName, messageFieldName, comment1FieldName, comment2FieldName, createMissingTable);
            this.InitErrorLogs(tableName, dateFieldName, sourceApplicationFieldName, logTypeFieldName, messageFieldName, comment1FieldName, comment2FieldName, false);
        }// InitLogs()
        public void InitInformationLogs(String tableName = "T_Logs", String dateFieldName = "CreationDate", String sourceApplicationFieldName = "SourceApplication", String logTypeFieldName = "Type", String messageFieldName = "Message", String comment1FieldName = null, String comment2FieldName = null, Boolean createMissingTable = false)
        {
            this.LogInfoConfig = new LogConfigurationInfos(tableName, dateFieldName, sourceApplicationFieldName, logTypeFieldName, messageFieldName, comment1FieldName, comment2FieldName);

            if (createMissingTable == true) this.CreateLogTable(this.LogInfoConfig);
        }// InitInformationLogs()
        public void InitErrorLogs(String tableName = "T_Logs", String dateFieldName = "CreationDate", String sourceApplicationFieldName = "SourceApplication", String logTypeFieldName = "Type", String messageFieldName = "Message", String comment1FieldName = null, String comment2FieldName = null, Boolean createMissingTable = false)
        {
            this.LogErrorConfig = new LogConfigurationInfos(tableName, dateFieldName, sourceApplicationFieldName, logTypeFieldName, messageFieldName, comment1FieldName, comment2FieldName);

            if (createMissingTable == true) this.CreateLogTable(this.LogErrorConfig);
        }// InitErrorLogs
        public OrionDeleteQuery PrepareQueryDelete(String tableName)
        {
            return this.PrepareQueryDelete(tableName, null);
        }// PrepareQueryDelete()
        public OrionDeleteQuery PrepareQueryDelete(String tableName, String whereClause)
        {
            Boolean bReplacePreviousQuery;
            OrionDeleteQuery xNewQuery;

            xNewQuery = null;

            if (String.IsNullOrEmpty(tableName) == false)
            {
                bReplacePreviousQuery = true;

                if (this.xQueries.Count != 0)
                {
                    if (this.CurrentQuery.Executed == false && this.CurrentQuery.Failed == false)
                        throw new OrionException("Previous query has not been executed.");
                    else if (this.TransactionState == TransactionStates.Started)
                        bReplacePreviousQuery = false;
                }

                xNewQuery = new OrionDeleteQuery(this, tableName, whereClause);
                if (xNewQuery != null)
                {
                    if (bReplacePreviousQuery == true) this.xQueries.Clear();
                    this.xQueries.Add(xNewQuery);
                }
            }
            else
                throw new OrionException("Delete query needs a table name.");

            return xNewQuery;
        }// PrepareQueryDelete()
        public OrionExecuteQuery PrepareQueryExecute(String queryString)
        {
            Boolean bReplacePreviousQuery;
            OrionExecuteQuery xNewQuery;

            if (String.IsNullOrEmpty(queryString) == false)
            {
                bReplacePreviousQuery = true;

                if (this.xQueries.Count != 0)
                {
                    if (this.CurrentQuery.Executed == false && this.CurrentQuery.Failed == false)
                        throw new OrionException("Previous query has not been executed.");
                    else if (this.TransactionState == TransactionStates.Started)
                        bReplacePreviousQuery = false;
                }

                xNewQuery = new OrionExecuteQuery(this, queryString);
                if (xNewQuery != null)
                {
                    if (bReplacePreviousQuery == true) this.xQueries.Clear();
                    this.xQueries.Add(xNewQuery);
                }

                return xNewQuery;
            }
            else
                throw new OrionException("Query needs a query string to be initialized.");
        }// PrepareQueryExecute()
        public OrionRowCountQuery PrepareQueryRowCount(String tableName, String whereClause = null)
        {
            Boolean bReplacePreviousQuery;
            OrionRowCountQuery xNewQuery;

            xNewQuery = null;

            if (String.IsNullOrEmpty(tableName) == false)
            {
                bReplacePreviousQuery = true;

                if (this.xQueries.Count != 0)
                {
                    if (this.CurrentQuery.Executed == false && this.CurrentQuery.Failed == false)
                        throw new OrionException("Previous query has not been executed.");
                    else if (this.TransactionState == TransactionStates.Started)
                        bReplacePreviousQuery = false;
                }

                xNewQuery = new OrionRowCountQuery(this, tableName, whereClause);
                if (xNewQuery != null)
                {
                    if (bReplacePreviousQuery == true) this.xQueries.Clear();
                    this.xQueries.Add(xNewQuery);
                }
            }
            else
                throw new OrionException("RowCount query needs a table name.");

            return xNewQuery;
        }// PrepareQueryRowCount()
        public OrionInsertQuery PrepareQueryInsert(String tableName, params String[] fieldNames)
        {
            Boolean bReplacePreviousQuery;
            OrionInsertQuery xNewQuery;

            xNewQuery = null;

            if (String.IsNullOrEmpty(tableName) == false)
            {
                if (fieldNames != null && fieldNames.Length > 0)
                {
                    bReplacePreviousQuery = true;

                    if (this.xQueries.Count != 0)
                    {
                        if (this.CurrentQuery.Executed == false && this.CurrentQuery.Failed == false)
                            throw new OrionException("Previous query has not been executed.");
                        else if (this.TransactionState == TransactionStates.Started)
                            bReplacePreviousQuery = false;
                    }

                    xNewQuery = new OrionInsertQuery(this, tableName, fieldNames);
                    if (xNewQuery != null)
                    {
                        if (bReplacePreviousQuery == true) this.xQueries.Clear();
                        this.xQueries.Add(xNewQuery);
                    }
                }
                else
                    throw new OrionException("Insert query needs field names.");
            }
            else
                throw new OrionException("Insert query needs a table name.");

            return xNewQuery;
        }// PrepareQueryInsert()
        public OrionInsertQuery PrepareQueryInsert(String tableName, DataRow sourceRow, params String[] exclusions)
        {
            Boolean bReplacePreviousQuery;
            OrionInsertQuery xNewQuery;

            xNewQuery = null;

            if (String.IsNullOrEmpty(tableName) == false)
            {
                if (sourceRow != null)
                {
                    bReplacePreviousQuery = true;

                    if (this.xQueries.Count != 0)
                    {
                        if (this.CurrentQuery.Executed == false && this.CurrentQuery.Failed == false)
                            throw new OrionException("Previous query has not been executed.");
                        else if (this.TransactionState == TransactionStates.Started)
                            bReplacePreviousQuery = false;
                    }

                    xNewQuery = new OrionInsertQuery(this, tableName, sourceRow, exclusions);
                    if (xNewQuery != null)
                    {
                        if (bReplacePreviousQuery == true) this.xQueries.Clear();
                        this.xQueries.Add(xNewQuery);
                    }
                }
                else
                    throw new OrionException("Insert query needs a source DataRow.");
            }
            else
                throw new OrionException("Insert query needs a table name.");

            return xNewQuery;
        }// PrepareQueryInsert() 
        public OrionSelectQuery PrepareQuerySelect(String queryString)
        {
            Boolean bReplacePreviousQuery;
            OrionSelectQuery xNewQuery;

            if (String.IsNullOrEmpty(queryString) == false)
            {
                bReplacePreviousQuery = true;

                if (this.xQueries.Count != 0)
                {
                    if (this.CurrentQuery.Executed == false && this.CurrentQuery.Failed == false)
                        throw new OrionException("Previous query has not been executed.");
                    else if (this.TransactionState == TransactionStates.Started)
                        bReplacePreviousQuery = false;
                }

                xNewQuery = new OrionSelectQuery(this, queryString);
                if (xNewQuery != null)
                {
                    if (bReplacePreviousQuery == true) this.xQueries.Clear();
                    this.xQueries.Add(xNewQuery);
                }

                return xNewQuery;
            }
            else
                throw new OrionException("Query needs a query string to be initialized.");
        }// PrepareQuerySelect()
        public OrionUpdateQuery PrepareQueryUpdate(String tableName, String whereClause, params String[] fieldNames)
        {
            Boolean bReplacePreviousQuery;
            OrionUpdateQuery xNewQuery;

            xNewQuery = null;

            if (String.IsNullOrEmpty(tableName) == false)
            {
                if (fieldNames != null && fieldNames.Length > 0)
                {
                    bReplacePreviousQuery = true;

                    if (this.xQueries.Count != 0)
                    {
                        if (this.CurrentQuery.Executed == false && this.CurrentQuery.Failed == false)
                            throw new OrionException("Previous query has not been executed.");
                        else if (this.TransactionState == TransactionStates.Started)
                            bReplacePreviousQuery = false;
                    }

                    xNewQuery = new OrionUpdateQuery(this, tableName, whereClause, fieldNames);
                    if (xNewQuery != null)
                    {
                        if (bReplacePreviousQuery == true) this.xQueries.Clear();
                        this.xQueries.Add(xNewQuery);
                    }
                }
                else
                    throw new OrionException("Update query needs field names.");
            }
            else
                throw new OrionException("Update query needs a table name.");

            return xNewQuery;
        }// PrepareQueryUpdate()
         /// <summary>
         /// Creates and starts a new transaction.
         /// </summary>
         /// <remarks>Every following SQL request will be automatically enroled in this transaction. The whole transaction can be validated and executed by using the <see cref="CommitTransaction()" /> method.
         /// <br />If a transaction is started and has not been committed, <see cref="RollbackTransaction()" /> method is called before disconnecting or disposing the <see cref="OrionDatabase" /> object.</remarks>
         /// <exception cref="OrionException">A transaction has already been started.</exception>
         /// <seealso cref="OrionException" />
         /// <seealso cref="CommitTransaction()" />
         /// <seealso cref="RollbackTransaction()" />
        public void StartTransaction()
        {
            if (this.TransactionState != TransactionStates.Started)
                if (this.xQueries.Count == 0 || this.CurrentQuery.Executed == true || this.CurrentQuery.Failed == true)
                {
                    if (this.Connection.State != ConnectionState.Open) this.Connect();

                    try
                    {
                        this.Transaction = this.Connection.BeginTransaction();
                    }
                    catch (DbException ex)
                    {
                        throw new OrionException("DbConnection.BeginTransaction() can't be executed;", ex);
                    }
                    this.TransactionState = TransactionStates.Started;

                    this.xQueries.Clear();
                }
                else
                    throw new OrionException("No transaction can be started if a query has already been initialized.");
            else
                throw new OrionException("A transaction has always been started.");
        }// StartTransaction()
        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        /// <remarks>All pending operations are cancelled. If <see cref="PersistentConnection" /> property value is <i>false</i>, connection will be closed after the rollback processus.</remarks>
        /// <seealso cref="PersistentConnection" />
        /// <exception cref="OrionException">Transaction has not been rollbacked.</exception>
        /// <seealso cref="OrionException" />
        public void RollbackTransaction()
        {
            if (this.Transaction != null)
            {
                try
                {
                    this.Transaction.Rollback();
                }
                catch (DbException ex)
                {
                    throw new OrionException("Error while trying to rollback transaction;", ex);
                }

                this.TransactionState = TransactionStates.Rollbacked;

                this.Transaction.Dispose();
                this.Transaction = null;
            }
            else
                throw new OrionException("No transaction has been started;");

            this.xQueries.Clear();

            if (this.Connection.State != ConnectionState.Closed && this.PersistentConnection == false) this.Disconnect();
        }// RollbackTransaction()
        /// <summary>
        /// Search for table in database.
        /// </summary>
        /// <param name="tableName">Name of the table to find.</param>
        /// <returns><i>True</i> if table exists.</returns>
        public Boolean TableExits(String tableName)
        {
            DataTable xResults;
            OrionSelectQuery xQuery;

            try
            {
                xQuery = this.PrepareQuerySelect("SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "'");
                xResults = (DataTable)xQuery.Execute();
            }
            catch (OrionException ex)
            {
                throw new OrionException("Can't read sqlite_master table in order to search for '" + tableName + "' table;", ex);
            }

            return xResults != null && xResults.Rows.Count > 0 ? true : false;
        }// TableExits()
        #endregion

        #region Utility procedures
        private void CreateLogTable(LogConfigurationInfos logConfig)
        {
            StringBuilder strQuery;
            OrionExecuteQuery xQuery;

            if (this.TableExits(logConfig.TableName) == false)
            {
                strQuery = new StringBuilder("CREATE TABLE " + logConfig.TableName + " (");
                strQuery.Append(logConfig.DateFieldName + " text NOT NULL,");
                strQuery.Append(logConfig.SourceApplicationFieldName + " text NOT NULL,");
                strQuery.Append(logConfig.TypeLogFieldName + " text,");
                strQuery.Append(logConfig.MessageFieldName + " text");
                if (String.IsNullOrEmpty(logConfig.Comment1FieldName) == false) strQuery.Append("," + logConfig.Comment1FieldName + " text");
                if (String.IsNullOrEmpty(logConfig.Comment2FieldName) == false) strQuery.Append("," + logConfig.Comment2FieldName + " text");
                strQuery.Append(")");

                xQuery = this.PrepareQueryExecute(strQuery.ToString());
                xQuery.Execute();
            }
        }// CreateLogTable()
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="OrionDatabase"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">Type: <see cref="Boolean" />
        /// <br /><i>true</i> to release both managed and unmanaged resources; <i>false</i> to release only unmanaged resources.</param>
        /// <remarks>If a transaction has been started and is not commited yet, <see cref="RollbackTransaction()"/> method is called before disposing the <see cref="OrionDatabase" /> object</remarks>
        /// <seealso cref="RollbackTransaction()"/>
        private void Dispose(Boolean disposing)
        {
            if (!this.bDisposed)
            {
                //** Release managed resources **
                if (disposing)
                {
                    if (this.Connection != null && this.Connection.State != ConnectionState.Closed) this.Disconnect();
                    if (this.Transaction != null) this.RollbackTransaction();
                }

                this.bDisposed = true;
            }
        }// Dispose()
        #endregion
    }
}
