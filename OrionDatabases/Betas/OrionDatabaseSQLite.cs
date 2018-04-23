using System;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;
using OrionCore.ErrorManagement;
using OrionDatabases.Queries;

namespace OrionDatabases
{
    /// <summary>Class used to connect to SQLite databases and manage their datas.</summary>
    /// <remarks>Inherits from <see cref="OrionDatabase" /> base class.</remarks>
    public class OrionDatabaseSQLite : OrionDatabase
    {
        #region Properties
        /// <summary>
        /// ADO .Net provider dll name.
        /// </summary>
        /// <value>Type : <see cref="String"/>
        public static String RequiredDll
        {
            get
            {
                return "System.Data.SQLite.dll";
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OrionDatabaseSQLite" /> class without any access restrictions.
        /// </summary>
        /// <remarks>A non secured access will be used to connect to the given SQLite database.
        /// <br />No data will be loaded at this point.
        /// <br /><br />If <b>databaseFilePath</b> is <i>null</i> or <i>String.Empty</i>, invalid or indicates a missing file, a <see cref="OrionException" /> is thrown. Its <b>Data</b> property contains a <i>databaseFilePath</i> entry with provided <b>databaseFilePath</b> as value.<br />This exception is also thrown if <i>System.Data.SQLite.dll</i> is missing.
        /// </remarks>
        /// <param name="databaseFilePath">Path to the SQLite database file.</param>
        /// <exception cref="OrionException"><b>databaseFilePath</b> is <i>null</i>, invalid or indicates a missing file, or <i>System.Data.SQLite.dll</i> is missing.</exception>
        /// <seealso cref="OrionException" />
        public OrionDatabaseSQLite(String databaseFilePath)
        {
            // Check dll
            OrionDatabase.CheckMissingDll(OrionDatabaseSQLite.RequiredDll);

            this.Initialize(databaseFilePath, null);
        }// OrionDatabaseSQLite()
         /// <summary>
         /// Initializes a new instance of the <see cref="OrionDatabaseSQLite" /> class with authentication.
         /// </summary>
         /// <remarks>A password is used to connect to the SQLite database.
         /// <br />No data will be loaded at this point.
         /// <br /><br />If <b>databaseFilePath</b> is <i>null</i> or <i>String.Empty</i>, invalid or indicates a missing file, a <see cref="OrionException" /> is thrown. Its <b>Data</b> property contains a <i>databaseFilePath</i> entry with provided <b>databaseFilePath</b> as value.<br />This exception is also thrown if <i>System.Data.SQLite.dll</i> is missing.
         /// </remarks>
         /// <param name="databaseFilePath">Path to the SQLite database file.</param>
         /// <param name="password">Password of the user as specified in the SQLite database.</param>
         /// <exception cref="OrionException"><b>databaseFilePath</b> is not valid or indicates a missing file, or <i>System.Data.SQLite.dll</i> is missing.</exception>
         /// <seealso cref="OrionException" />
        public OrionDatabaseSQLite(String databaseFilePath, String password)
        {
            // Check dll
            OrionDatabase.CheckMissingDll(OrionDatabaseSQLite.RequiredDll);

            this.Initialize(databaseFilePath, password);
        }// OrionDatabaseSQLite()
        #endregion

        #region Initializations
        private void Initialize(String databaseFilePath, String password)
        {
            this.ParameterCharacter = '@';

            //** Check if the database file path is valid and indicates an existing file **
            OrionDatabaseSQLite.CheckPathValidity(databaseFilePath, true);

            this.ConnectionStringBuilder = new SQLiteConnectionStringBuilder();

            base.Initialize();

            this.ConnectionStringBuilder["Data Source"] = databaseFilePath;
            this.ConnectionStringBuilder["Version"] = "3";
            this.ConnectionStringBuilder["FailIfMissing"] = Boolean.FalseString;

            // Add password if needed.
            if (String.IsNullOrEmpty(password) == false) this.ConnectionStringBuilder["Password"] = password;

            // Create the ADO.Net connection
            this.Connection = new SQLiteConnection(this.ConnectionStringBuilder.ConnectionString, true);
        }// Initialize()
        #endregion

        #region Base abstract class implementation
        /// <exclude />
        internal override DbCommand CreateCommand(String sqlQuery)
        {
            SQLiteCommand xCommand;

            try
            {
                xCommand = new SQLiteCommand(sqlQuery, (SQLiteConnection)this.Connection);
            }
            catch (DbException ex)
            {
                throw new OrionException("Can't create SQLiteCommand;", ex, "SqlQuery=" + sqlQuery);
            }

            xCommand.Transaction = (SQLiteTransaction)this.Transaction;

            return xCommand;
        }// CreateCommand()
         /// <exclude />
        internal override DbCommand CreateCommandGetLastValue(String tableName, String fieldName)
        {
            String strQuery;
            SQLiteCommand xCommand;

            strQuery = "select " + fieldName + " from " + tableName + " where rowid = (select max(rowid) from " + tableName + ")";

            try
            {
                xCommand = new SQLiteCommand(strQuery, (SQLiteConnection)this.Connection);
            }
            catch (DbException ex)
            {
                throw new OrionException("Can't create SQLiteCommand;", ex, "SqlQuery=" + strQuery);
            }

            xCommand.Transaction = (SQLiteTransaction)this.Transaction;

            return xCommand;
        }// CreateCommandGetLastValue()
        /// <exclude />
        internal override DbParameter CreateParameter(String key, Object value)
        {
            return new SQLiteParameter(key, value);
        }// CreateParameter();
        /// <exclude />
        internal override DbDataAdapter CreateDataAdapter(String sqlRequest)
        {
            try
            {
                return new SQLiteDataAdapter(sqlRequest, (SQLiteConnection)this.Connection);
            }
            catch (DbException ex)
            {
                throw new OrionException("Can't create SQLite DataAdapter;", ex);
            }
        }// CreateDataAdapter()
        #endregion

        #region Public interface
        /// <summary>
        /// Creates a new SQLite database file if missing.
        /// </summary>
        /// <param name="databaseFilePath">Path of the new SQLite database file to create.</param>
        /// <exception cref="OrionException">File already exists or database can't be created. <see cref="Exception.Data"/> dictionnary contains <i>DatabaseFilePath</i> key with provided file path as value.</exception>
        /// <seealso cref="OrionException" />
        static public void CreateDatabaseFile(String databaseFilePath)
        {
            OrionDatabaseSQLite.CreateDatabaseFile(databaseFilePath, null);
        }// CreateDatabaseFile()
        /// <summary>
        /// Creates a new SQLite database file if missing.
        /// </summary>
        /// <param name="databaseFilePath">Path of the new SQLite database file to create.</param>
        /// <param name="password">Password to set on database.</param>
        /// <exception cref="OrionException">File already exists or database can't be created. <see cref="Exception.Data"/> dictionnary contains <i>DatabaseFilePath</i> key with provided file path as value.</exception>
        /// <seealso cref="OrionException" />
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "N/A")]
        static public void CreateDatabaseFile(String databaseFilePath, String password)
        {
            OrionDatabaseSQLite xBase;

            xBase = null;

            // Check if the database file path is not null, not an empty string, and valid.
            OrionDatabaseSQLite.CheckPathValidity(databaseFilePath);

            //** Check if the database file path indicates an existing file **
            if (File.Exists(databaseFilePath) == false)
            {
                try
                {
                    SQLiteConnection.CreateFile(databaseFilePath);
                }
                catch (Exception ex)
                {
                    throw new OrionException("Database can't be created.", ex, "DatabaseFilePath=" + databaseFilePath);
                }

                try
                {
                    xBase = new OrionDatabaseSQLite(databaseFilePath);
                }
                catch (OrionException ex)
                {
                    throw new OrionException("Can't create XDatabaseSQLite object;", ex, "DatabaseFilePath=" + databaseFilePath);
                }

                if (String.IsNullOrWhiteSpace(password) == false)
                    try
                    {
                        xBase.SetPassword(password, false);
                    }
                    catch (Exception ex)
                    {
                        throw new OrionException("Password can't be set.", ex, "DatabaseFilePath=" + databaseFilePath);
                    }
            }
            else
                throw new OrionException("The specified SQLite database file already exists.", "DatabaseFilePath=" + databaseFilePath);
        }// CreateDatabaseFile()

        public void SetPassword(String password, Boolean persistentConnection)
        {
            Byte[] byPasswordBytes;

            if (this.Connection != null)
            {
                this.Connect();

                if (String.IsNullOrWhiteSpace(password) == false)
                {
                    byPasswordBytes = new byte[password.Length * sizeof(char)];
                    byPasswordBytes = System.Text.Encoding.Default.GetBytes(password);
                    ((SQLiteConnection)this.Connection).ChangePassword(byPasswordBytes);
                }
                else
                    ((SQLiteConnection)Connection).ChangePassword(String.Empty);

                if (persistentConnection == false) this.Disconnect();
            }
        }// SetPassword()
        #endregion

        #region Utility procedures
        static private void CheckPathValidity(String databaseFilePath, Boolean mustExist = false)
        {
            String strDirectoryPath;

            if (String.IsNullOrWhiteSpace(databaseFilePath) == true)
                throw new OrionException("The SQLite database file path can't be null or an empty string.", "DatabaseFilePath=" + databaseFilePath);
            else
            {
                try
                {
                    strDirectoryPath = Path.GetDirectoryName(databaseFilePath);
                }
                catch (Exception ex)
                {
                    throw new OrionException("The database file path is invalid.", ex, "DatabaseFilePath=" + databaseFilePath);
                }

                if (Directory.Exists(strDirectoryPath) == false) throw new OrionException("The database directory path can't be found.", "DatabaseFilePath=" + databaseFilePath);
                if (mustExist == true && File.Exists(databaseFilePath) == false) throw new OrionException("The specified SQLite database file is missing.", "DatabaseFilePath=" + databaseFilePath);
            }
        }// CheckPathValidity()
        #endregion
    }
}
