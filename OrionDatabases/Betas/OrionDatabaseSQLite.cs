using System;
using System.IO;
using System.Data.SQLite;
using OrionCore.ErrorManagement;

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
            OrionDatabaseSQLite.CheckPathValidity(databaseFilePath);

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

        #region Utility procedures
        static private void CheckPathValidity(String databaseFilePath)
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
                if (File.Exists(databaseFilePath) == false) throw new OrionException("The specified SQLite database file is missing.", "DatabaseFilePath=" + databaseFilePath);
            }
        }// CheckPathValidity()
        #endregion
    }
}
