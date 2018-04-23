using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrionCore.ErrorManagement;
using OrionCore.EventManagement;
using OrionCore.LogManagement;
using OrionDatabases.Queries;
using System.Data.SQLite;
using OrionDatabases;
using OrionCore;

namespace OrionDatabasesTests
{
    [TestClass]
    public class OrionDatabaseSQLiteTests
    {
        #region Fields
        internal const String strPASSWORD = "Poseidon";
        internal const String strSOURCENOPASSWORDBASEFILENAME = "Datas_No_Password.dtx";
        internal const String strSOURCEPASSWORDBASEFILENAME = "Datas_Password.dtx";

        private const String strTESTBASEFILENAME = "Datas.dtx";

        private static String strSourceDirectoryPath, strTestsDirectoryPath, strTestsMiscellaneousDirectoryPath;
        private static String strTestsNoPasswordDirectoryPath, strTestsPasswordDirectoryPath;
        #endregion

        #region Initializations
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            String strTargetDirectoryPath;
            String[] strDirectoryNames;
            Exception xException;

            xException = null;
            OrionDatabaseSQLiteTests.strTestsDirectoryPath = Path.Combine(OrionDeploymentInfos.DataFolder, "OrionDatabaseSQLiteTests");
            OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsDirectoryPath, "Miscellaneous");
            OrionDatabaseSQLiteTests.strSourceDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsDirectoryPath, "Source");
            OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsDirectoryPath, "No Password");
            OrionDatabaseSQLiteTests.strTestsPasswordDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsDirectoryPath, "Password");

            Assert.IsTrue(Directory.Exists(OrionDatabaseSQLiteTests.strSourceDirectoryPath));

            try
            {
                strDirectoryNames = new String[] { "Missing Dll", "Set Password", "Create Database Existing File" };
                if (Directory.Exists(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath) == false) Directory.CreateDirectory(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath);
                foreach (String strDirectoryNameTemp in strDirectoryNames)
                {
                    strTargetDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, strDirectoryNameTemp);
                    if (Directory.Exists(strTargetDirectoryPath) == false) Directory.CreateDirectory(strTargetDirectoryPath);
                }

                // No Password.
                OrionDatabaseSQLiteTests.InitializeDirectories(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, new String[] { "Missing Database", "Create Database", "Initialize Database", "Connect Database" });

                // No Password Logs.
                OrionDatabaseSQLiteTests.InitializeDirectories(Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs"), new String[] { "Initialization Default Values", "Initialization Default Values Create Table", "Initialization User Values Separate Tables", "Initialization User Values Create Separate Tables", "No Initialization Exception", "ReportEvent Information Default Table Ok", "ReportEvent Warning User Table Ok", "ReportEvent User Tables Ok" });

                // No Password DELETE queries.
                OrionDatabaseSQLiteTests.InitializeDirectories(Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Delete Queries"), new String[] { "No Table", "Unknown Table", "Whole Table", "Row With Parameters" });

                // No Password INSERT queries.
                OrionDatabaseSQLiteTests.InitializeDirectories(Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries"), new String[] { "No Table", "Unknown Table", "No Field Names", "No Row", "Whole Row", "Row By Parameters", "Row By Parameters Get New Id Ok" });

                // No Password SELECT queries.
                OrionDatabaseSQLiteTests.InitializeDirectories(Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Select Queries"), new String[] { "No Query", "Bad Query", "Ok", "With Parameters Ok", "With Missing Parameter", "With Unnamed Parameter" });

                // No Password RowCounter queries.
                OrionDatabaseSQLiteTests.InitializeDirectories(Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "RowCount Queries"), new String[] { "No Table", "Unknown Table", "OK", "With Parameters" });

                // No Password UPDATE queries.
                OrionDatabaseSQLiteTests.InitializeDirectories(Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Update Queries"), new String[] { "No Table", "No Field Names", "Unknown Table", "Row" });

                // No Password EXECUTE queries.
                OrionDatabaseSQLiteTests.InitializeDirectories(Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Execute Queries"), new String[] { "No Query", "Bad Query", "OK" });

                //** No Password Transactions. **
                strTargetDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Transactions");
                if (Directory.Exists(strTargetDirectoryPath) == false) Directory.CreateDirectory(strTargetDirectoryPath);
                strTargetDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Transactions", "OK");
                if (Directory.Exists(strTargetDirectoryPath) == false) Directory.CreateDirectory(strTargetDirectoryPath);

                // Password.
                OrionDatabaseSQLiteTests.InitializeDirectories(OrionDatabaseSQLiteTests.strTestsPasswordDirectoryPath, new String[] { "Missing Database", "Create Database", "Initialize Database", "Connect Database" });
            }
            catch (Exception ex)
            {
                xException = ex;
            }

            Assert.IsNull(xException, "Class Initialize() method failed;");
        }// Initialize()
        [TestInitialize]
        public void InitializeTest()
        {
            String strRequiredDllFilePath, strSourceDllFilePath;
            Exception xException;

            xException = null;

            strRequiredDllFilePath = Path.Combine(OrionDeploymentInfos.DataFolder, OrionDatabaseSQLite.RequiredDll);
            strSourceDllFilePath = Path.Combine(OrionDatabaseSQLiteTests.strSourceDirectoryPath, OrionDatabaseSQLite.RequiredDll);

            try
            {
                if (File.Exists(strRequiredDllFilePath) == false) File.Copy(strSourceDllFilePath, strRequiredDllFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }

            Assert.IsNull(xException, "Test Initialize() method failed;");
        }// InitializeTest()
        #endregion

        #region Test methods
        #region Miscellaneous
        [TestMethod, TestCategory("OrionDatabaseSQLite"), Ignore]
        public void Miscellaneous_MissingSQLiteDll_OrionException()
        {
            String strRequiredDllFilePath, strSourceBaseFilePath, strTargetBaseFilePath;
            Exception xException;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strSourceBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strSourceDirectoryPath, OrionDatabaseSQLiteTests.strSOURCENOPASSWORDBASEFILENAME);
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, "Missing Dll", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            strRequiredDllFilePath = Path.Combine(OrionDeploymentInfos.DataFolder, OrionDatabaseSQLite.RequiredDll);
            xException = null;
            xOrionException = null;

            try
            {
                if (File.Exists(strTargetBaseFilePath) == true) File.Delete(strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException, "Can't delete existing test database file;");

            try
            {
                File.Copy(strSourceBaseFilePath, strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException, "Can't copy source database file;");

            try
            {
                if (File.Exists(strRequiredDllFilePath) == true) File.Delete(strRequiredDllFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }

            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Missing dll [" + OrionDatabaseSQLite.RequiredDll + "];");
            Assert.AreEqual(xOrionException.Data.Contains("DllFilePath"), true);
            Assert.AreEqual(xOrionException.Data["DllFilePath"], strRequiredDllFilePath);
        }// Miscellaneous_MissingSQLiteDll_OrionException()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void Miscellaneous_SetPassword_OkDisconnected()
        {
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, "Set Password", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xResults = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_Directors").Execute();
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreNotEqual(xResults.Rows.Count, 0);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.SetPassword(OrionDatabaseSQLiteTests.strPASSWORD, false);
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xResults = null;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_Directors").Execute();
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.IsInstanceOfType(xOrionException.InnerException, typeof(SQLiteException));
            Assert.AreEqual(xOrionException.InnerException.Message, "file is not a database\r\nfile is not a database");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            if (xOrionException != null && xBase != null) xBase.Dispose();

            try
            {
                xOrionException = null;
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath, OrionDatabaseSQLiteTests.strPASSWORD);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xOrionException = null;
                xResults = null;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_Directors").Execute();
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xResults.Rows.Count, 16);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            if (xBase != null) xBase.Dispose();
        }// Miscellaneous_SetPassword_OkDisconnected()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_TableExists_Ok()
        {
            Boolean bResult;
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            bResult = false;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, "TableExists", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                bResult = xBase.TableExits("TEST");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsFalse(bResult);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xBase.PersistentConnection = false;
                bResult = xBase.TableExits("T_OrionDatabases_Tests");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bResult);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);
        }// NoPassword_TableExists_Ok()
        #endregion

        #region Logs
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_Initialization_Default_Values()
        {
            Boolean bTableExists;
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            bTableExists = false;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "Initialization Default Values", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.InitLogs();
            Assert.IsTrue(xBase.LogInfoConfig.IsInitialized);
            Assert.AreEqual(xBase.LogInfoConfig.TableName, "T_Logs");
            Assert.AreEqual(xBase.LogInfoConfig.DateFieldName, "CreationDate");
            Assert.AreEqual(xBase.LogInfoConfig.SourceApplicationFieldName, "SourceApplication");
            Assert.AreEqual(xBase.LogInfoConfig.MessageFieldName, "Message");
            Assert.IsNull(xBase.LogInfoConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogInfoConfig.Comment2FieldName);
            Assert.AreEqual(xBase.LogInfoConfig.TypeLogFieldName, "Type");

            Assert.IsTrue(xBase.LogErrorConfig.IsInitialized);
            Assert.AreEqual(xBase.LogErrorConfig.TableName, "T_Logs");
            Assert.AreEqual(xBase.LogErrorConfig.DateFieldName, "CreationDate");
            Assert.AreEqual(xBase.LogErrorConfig.SourceApplicationFieldName, "SourceApplication");
            Assert.AreEqual(xBase.LogErrorConfig.MessageFieldName, "Message");
            Assert.IsNull(xBase.LogErrorConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogErrorConfig.Comment2FieldName);
            Assert.AreEqual(xBase.LogErrorConfig.TypeLogFieldName, "Type");

            try
            {
                bTableExists = xBase.TableExits("T_Logs");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsFalse(bTableExists);

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_Initialization_Default_Values()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_Initialization_Default_Values_Create_Table()
        {
            Boolean bTableExists;
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            bTableExists = false;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "Initialization Default Values Create Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.InitLogs(createMissingTable: true);
            Assert.IsTrue(xBase.LogInfoConfig.IsInitialized);
            Assert.AreEqual(xBase.LogInfoConfig.TableName, "T_Logs");
            Assert.AreEqual(xBase.LogInfoConfig.DateFieldName, "CreationDate");
            Assert.AreEqual(xBase.LogInfoConfig.SourceApplicationFieldName, "SourceApplication");
            Assert.AreEqual(xBase.LogInfoConfig.TypeLogFieldName, "Type");
            Assert.AreEqual(xBase.LogInfoConfig.MessageFieldName, "Message");
            Assert.IsNull(xBase.LogInfoConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogInfoConfig.Comment2FieldName);

            Assert.IsTrue(xBase.LogErrorConfig.IsInitialized);
            Assert.AreEqual(xBase.LogErrorConfig.TableName, xBase.LogInfoConfig.TableName);
            Assert.AreEqual(xBase.LogErrorConfig.DateFieldName, xBase.LogInfoConfig.DateFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.SourceApplicationFieldName, xBase.LogInfoConfig.SourceApplicationFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.TypeLogFieldName, xBase.LogInfoConfig.TypeLogFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.MessageFieldName, xBase.LogInfoConfig.MessageFieldName);
            Assert.IsNull(xBase.LogErrorConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogErrorConfig.Comment2FieldName);

            try
            {
                bTableExists = xBase.TableExits("T_Logs");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bTableExists);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xBase.PersistentConnection = false;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM " + xBase.LogInfoConfig.TableName).Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.DateFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.SourceApplicationFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.TypeLogFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.MessageFieldName));
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_Initialization_Default_Values_Create_Table()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_Initialization_User_Values_Separate_Tables()
        {
            Boolean bTableExists;
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            bTableExists = false;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "Initialization Default Values Separate Tables", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            xBase.InitInformationLogs(tableName: "T_Logs_Infos", dateFieldName: "CreationDateInfos", sourceApplicationFieldName: "SourceApplicationInfos", logTypeFieldName: "TypeInfos", messageFieldName: "MessageInfos");
            xBase.InitErrorLogs(tableName: "T_Logs_Errors", dateFieldName: "CreationDateErrors", sourceApplicationFieldName: "SourceApplicationErrors", logTypeFieldName: "TypeErrors", messageFieldName: "MessageErrors");

            Assert.IsTrue(xBase.LogInfoConfig.IsInitialized);
            Assert.AreEqual(xBase.LogInfoConfig.TableName, "T_Logs_Infos");
            Assert.AreEqual(xBase.LogInfoConfig.DateFieldName, "CreationDateInfos");
            Assert.AreEqual(xBase.LogInfoConfig.SourceApplicationFieldName, "SourceApplicationInfos");
            Assert.AreEqual(xBase.LogInfoConfig.TypeLogFieldName, "TypeInfos");
            Assert.AreEqual(xBase.LogInfoConfig.MessageFieldName, "MessageInfos");
            Assert.IsNull(xBase.LogInfoConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogInfoConfig.Comment2FieldName);

            Assert.IsTrue(xBase.LogErrorConfig.IsInitialized);
            Assert.AreEqual(xBase.LogErrorConfig.TableName, "T_Logs_Errors");
            Assert.AreEqual(xBase.LogErrorConfig.DateFieldName, "CreationDateErrors");
            Assert.AreEqual(xBase.LogErrorConfig.SourceApplicationFieldName, "SourceApplicationErrors");
            Assert.AreEqual(xBase.LogErrorConfig.TypeLogFieldName, "TypeErrors");
            Assert.AreEqual(xBase.LogErrorConfig.MessageFieldName, "MessageErrors");
            Assert.IsNull(xBase.LogErrorConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogErrorConfig.Comment2FieldName);

            try
            {
                bTableExists = xBase.TableExits("T_Logs_Infos");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsFalse(bTableExists);

            try
            {
                xBase.PersistentConnection = false;
                bTableExists = xBase.TableExits("T_logs_Errors");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsFalse(bTableExists);

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_Initialization_Default_Values_Separate_Tables() 
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_Initialization_User_Values_Create_Separate_Tables()
        {
            Boolean bTableExists;
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            bTableExists = false;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "Initialization Default Values Create Separate Tables", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            xBase.InitInformationLogs(tableName: "T_Logs_Infos", dateFieldName: "CreationDateInfos", sourceApplicationFieldName: "SourceApplicationInfos", logTypeFieldName: "TypeInfos", messageFieldName: "MessageInfos", createMissingTable: true);
            xBase.InitErrorLogs(tableName: "T_Logs_Errors", dateFieldName: "CreationDateErrors", sourceApplicationFieldName: "SourceApplicationErrors", logTypeFieldName: "TypeErrors", messageFieldName: "MessageErrors", createMissingTable: true);

            Assert.IsTrue(xBase.LogInfoConfig.IsInitialized);
            Assert.AreEqual(xBase.LogInfoConfig.TableName, "T_Logs_Infos");
            Assert.AreEqual(xBase.LogInfoConfig.DateFieldName, "CreationDateInfos");
            Assert.AreEqual(xBase.LogInfoConfig.SourceApplicationFieldName, "SourceApplicationInfos");
            Assert.AreEqual(xBase.LogInfoConfig.TypeLogFieldName, "TypeInfos");
            Assert.AreEqual(xBase.LogInfoConfig.MessageFieldName, "MessageInfos");
            Assert.IsNull(xBase.LogInfoConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogInfoConfig.Comment2FieldName);

            Assert.IsTrue(xBase.LogErrorConfig.IsInitialized);
            Assert.AreEqual(xBase.LogErrorConfig.TableName, "T_Logs_Errors");
            Assert.AreEqual(xBase.LogErrorConfig.DateFieldName, "CreationDateErrors");
            Assert.AreEqual(xBase.LogErrorConfig.SourceApplicationFieldName, "SourceApplicationErrors");
            Assert.AreEqual(xBase.LogErrorConfig.TypeLogFieldName, "TypeErrors");
            Assert.AreEqual(xBase.LogErrorConfig.MessageFieldName, "MessageErrors");
            Assert.IsNull(xBase.LogErrorConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogErrorConfig.Comment2FieldName);

            try
            {
                bTableExists = xBase.TableExits(xBase.LogInfoConfig.TableName);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bTableExists);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM " + xBase.LogInfoConfig.TableName).Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.DateFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.SourceApplicationFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.TypeLogFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.MessageFieldName));
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                bTableExists = xBase.TableExits(xBase.LogErrorConfig.TableName);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bTableExists);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xBase.PersistentConnection = false;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM " + xBase.LogErrorConfig.TableName).Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.DateFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.SourceApplicationFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.TypeLogFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.MessageFieldName));
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_Initialization_Default_Values_Create_Separate_Tables()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_No_Initialization_Exception()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;
            OrionEventManager xEventManager;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "No Initialization Exception", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xEventManager.ReportEvent("Information log test", EventTypes.Information);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "OrionDatatable information log have to be initialized;");

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xEventManager.ReportEvent("Error log test", EventTypes.Warning);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "OrionDatatable error log have to be initialized;");

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xEventManager.ReportEvent("Error log test", EventTypes.Error);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "OrionDatatable error log have to be initialized;");

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xEventManager.ReportEvent("Error log test", EventTypes.CriticalError);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "OrionDatatable error log have to be initialized;");

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_No_Initialization_Exception()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_ReportEvent_Information_Default_Table_Ok()
        {
            Boolean bTableExists;
            String strTargetBaseFilePath;
            DataRow xRow;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;
            OrionEventManager xEventManager;

            bTableExists = true;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "ReportEvent Information Default Table Ok", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xEventManager = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            xBase.InitLogs(createMissingTable: true);
            Assert.IsTrue(xBase.LogInfoConfig.IsInitialized);
            Assert.AreEqual(xBase.LogInfoConfig.TableName, "T_Logs");
            Assert.AreEqual(xBase.LogInfoConfig.DateFieldName, "CreationDate");
            Assert.AreEqual(xBase.LogInfoConfig.SourceApplicationFieldName, "SourceApplication");
            Assert.AreEqual(xBase.LogInfoConfig.TypeLogFieldName, "Type");
            Assert.AreEqual(xBase.LogInfoConfig.MessageFieldName, "Message");
            Assert.IsNull(xBase.LogInfoConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogInfoConfig.Comment2FieldName);

            Assert.IsTrue(xBase.LogErrorConfig.IsInitialized);
            Assert.AreEqual(xBase.LogErrorConfig.TableName, xBase.LogInfoConfig.TableName);
            Assert.AreEqual(xBase.LogErrorConfig.DateFieldName, xBase.LogInfoConfig.DateFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.SourceApplicationFieldName, xBase.LogInfoConfig.SourceApplicationFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.TypeLogFieldName, xBase.LogInfoConfig.TypeLogFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.MessageFieldName, xBase.LogInfoConfig.MessageFieldName);
            Assert.IsNull(xBase.LogErrorConfig.Comment1FieldName);
            Assert.IsNull(xBase.LogErrorConfig.Comment2FieldName);

            try
            {
                bTableExists = xBase.TableExits("T_Logs");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bTableExists);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xEventManager.ReportEvent("Information log test", EventTypes.Information);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xBase.PersistentConnection = false;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM " + xBase.LogInfoConfig.TableName).Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.DateFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.SourceApplicationFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.TypeLogFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.MessageFieldName));
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);
            Assert.AreEqual(xResults.Rows.Count, 1);

            xRow = xResults.Rows[0];
            Assert.AreEqual(xRow[xBase.LogInfoConfig.SourceApplicationFieldName], xEventManager.Log.SourceApplicationName);
            Assert.AreEqual(xRow[xBase.LogInfoConfig.TypeLogFieldName], xEventManager.Log.EventType.ToString());
            Assert.AreEqual(xRow[xBase.LogInfoConfig.MessageFieldName], xEventManager.Log.LogMessage);

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_ReportEvent_Default_Table_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_ReportEvent_Warning_User_Table_Ok()
        {
            Boolean bTableExists;
            String strTargetBaseFilePath;
            DataRow xRow;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;
            OrionEventManager xEventManager;

            bTableExists = true;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "ReportEvent Warning User Table Ok", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xEventManager = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            xBase.InitLogs(tableName: "T_Logs_Informations", dateFieldName: "LogDate", sourceApplicationFieldName: "Application", logTypeFieldName: "EventType", messageFieldName: "Message", comment1FieldName: "Comment1", comment2FieldName: "Comment2", createMissingTable: true);
            Assert.IsTrue(xBase.LogInfoConfig.IsInitialized);
            Assert.AreEqual(xBase.LogInfoConfig.TableName, "T_Logs_Informations");
            Assert.AreEqual(xBase.LogInfoConfig.DateFieldName, "LogDate");
            Assert.AreEqual(xBase.LogInfoConfig.SourceApplicationFieldName, "Application");
            Assert.AreEqual(xBase.LogInfoConfig.TypeLogFieldName, "EventType");
            Assert.AreEqual(xBase.LogInfoConfig.MessageFieldName, "Message");
            Assert.AreEqual(xBase.LogInfoConfig.Comment1FieldName, "Comment1");
            Assert.AreEqual(xBase.LogInfoConfig.Comment2FieldName, "Comment2");

            Assert.IsTrue(xBase.LogErrorConfig.IsInitialized);
            Assert.AreEqual(xBase.LogErrorConfig.TableName, xBase.LogInfoConfig.TableName);
            Assert.AreEqual(xBase.LogErrorConfig.DateFieldName, xBase.LogInfoConfig.DateFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.SourceApplicationFieldName, xBase.LogInfoConfig.SourceApplicationFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.TypeLogFieldName, xBase.LogInfoConfig.TypeLogFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.MessageFieldName, xBase.LogInfoConfig.MessageFieldName);
            Assert.AreEqual(xBase.LogErrorConfig.Comment1FieldName, xBase.LogInfoConfig.Comment1FieldName);
            Assert.AreEqual(xBase.LogErrorConfig.Comment2FieldName, xBase.LogInfoConfig.Comment2FieldName);

            try
            {
                bTableExists = xBase.TableExits("T_Logs_Informations");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bTableExists);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xEventManager.ReportEvent("Information log test", EventTypes.Warning, "Test comment 1", "Test comment 2");
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xBase.PersistentConnection = false;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM " + xBase.LogInfoConfig.TableName).Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.DateFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.SourceApplicationFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.TypeLogFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.MessageFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.Comment1FieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.Comment2FieldName));
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);
            Assert.AreEqual(xResults.Rows.Count, 1);

            xRow = xResults.Rows[0];
            Assert.AreEqual(xRow[xBase.LogInfoConfig.SourceApplicationFieldName], xEventManager.Log.SourceApplicationName);
            Assert.AreEqual(xRow[xBase.LogInfoConfig.TypeLogFieldName], xEventManager.Log.EventType.ToString());
            Assert.AreEqual(xRow[xBase.LogInfoConfig.MessageFieldName], xEventManager.Log.LogMessage);
            Assert.AreEqual(xRow[xBase.LogInfoConfig.Comment1FieldName], xEventManager.Log.Comment1);
            Assert.AreEqual(xRow[xBase.LogInfoConfig.Comment2FieldName], xEventManager.Log.Comment2);

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_ReportEvent_Warning_User_Table_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Logs_ReportEvent_User_Tables_Ok()
        {
            Boolean bTableExists;
            String strTargetBaseFilePath;
            DataRow xRow;
            DataTable xResults;
            OrionLogInfos xLogErrors, xLogInformations;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;
            OrionEventManager xEventManager;

            bTableExists = true;
            xLogInformations = null;
            xLogErrors = null;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Logs", "ReportEvent User Tables Ok", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xEventManager = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            xBase.InitInformationLogs(tableName: "T_Logs_Informations", dateFieldName: "LogDateInfos", sourceApplicationFieldName: "ApplicationInfos", logTypeFieldName: "EventTypeInfos", messageFieldName: "MessageInfos", comment1FieldName: "Comment1Infos", comment2FieldName: "Comment2Infos", createMissingTable: true);
            Assert.IsTrue(xBase.LogInfoConfig.IsInitialized);
            Assert.AreEqual(xBase.LogInfoConfig.TableName, "T_Logs_Informations");
            Assert.AreEqual(xBase.LogInfoConfig.DateFieldName, "LogDateInfos");
            Assert.AreEqual(xBase.LogInfoConfig.SourceApplicationFieldName, "ApplicationInfos");
            Assert.AreEqual(xBase.LogInfoConfig.TypeLogFieldName, "EventTypeInfos");
            Assert.AreEqual(xBase.LogInfoConfig.MessageFieldName, "MessageInfos");
            Assert.AreEqual(xBase.LogInfoConfig.Comment1FieldName, "Comment1Infos");
            Assert.AreEqual(xBase.LogInfoConfig.Comment2FieldName, "Comment2Infos");

            xBase.InitErrorLogs(tableName: "T_Logs_Errors", dateFieldName: "LogDateErrors", sourceApplicationFieldName: "ApplicationErrors", logTypeFieldName: "EventTypeErrors", messageFieldName: "MessageErrors", comment1FieldName: "Comment1Errors", comment2FieldName: "Comment2Errors", createMissingTable: true);
            Assert.IsTrue(xBase.LogErrorConfig.IsInitialized);
            Assert.AreEqual(xBase.LogErrorConfig.TableName, "T_Logs_Errors");
            Assert.AreEqual(xBase.LogErrorConfig.DateFieldName, "LogDateErrors");
            Assert.AreEqual(xBase.LogErrorConfig.SourceApplicationFieldName, "ApplicationErrors");
            Assert.AreEqual(xBase.LogErrorConfig.TypeLogFieldName, "EventTypeErrors");
            Assert.AreEqual(xBase.LogErrorConfig.MessageFieldName, "MessageErrors");
            Assert.AreEqual(xBase.LogErrorConfig.Comment1FieldName, "Comment1Errors");
            Assert.AreEqual(xBase.LogErrorConfig.Comment2FieldName, "Comment2Errors");

            try
            {
                bTableExists = xBase.TableExits("T_Logs_Informations");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bTableExists);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                bTableExists = xBase.TableExits("T_Logs_Errors");
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(bTableExists);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xLogInformations = xEventManager.ReportEvent("Information log test", EventTypes.Information, "Test information comment 1", "Test information comment 2");
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xEventManager = new OrionEventManager(xBase);
                xLogErrors = xEventManager.ReportEvent("Warning log test", EventTypes.Warning, "Test warning comment 1", "Test warning comment 2");
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM " + xBase.LogInfoConfig.TableName).Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.DateFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.SourceApplicationFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.TypeLogFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.MessageFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.Comment1FieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogInfoConfig.Comment2FieldName));
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);
            Assert.AreEqual(xResults.Rows.Count, 1);

            xRow = xResults.Rows[0];
            Assert.AreEqual(xRow[xBase.LogInfoConfig.SourceApplicationFieldName], xLogInformations.SourceApplicationName);
            Assert.AreEqual(xRow[xBase.LogInfoConfig.TypeLogFieldName], xLogInformations.EventType.ToString());
            Assert.AreEqual(xRow[xBase.LogInfoConfig.MessageFieldName], xLogInformations.LogMessage);
            Assert.AreEqual(xRow[xBase.LogInfoConfig.Comment1FieldName], xLogInformations.Comment1);
            Assert.AreEqual(xRow[xBase.LogInfoConfig.Comment2FieldName], xLogInformations.Comment2);


            try
            {
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM " + xBase.LogErrorConfig.TableName).Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.DateFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.SourceApplicationFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.TypeLogFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.MessageFieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.Comment1FieldName));
            Assert.IsTrue(xResults.Columns.Contains(xBase.LogErrorConfig.Comment2FieldName));
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);
            Assert.AreEqual(xResults.Rows.Count, 1);

            xRow = xResults.Rows[0];
            Assert.AreEqual(xRow[xBase.LogErrorConfig.SourceApplicationFieldName], xLogErrors.SourceApplicationName);
            Assert.AreEqual(xRow[xBase.LogErrorConfig.TypeLogFieldName], xLogErrors.EventType.ToString());
            Assert.AreEqual(xRow[xBase.LogErrorConfig.MessageFieldName], xLogErrors.LogMessage);
            Assert.AreEqual(xRow[xBase.LogErrorConfig.Comment1FieldName], xLogErrors.Comment1);
            Assert.AreEqual(xRow[xBase.LogErrorConfig.Comment2FieldName], xLogErrors.Comment2);

            xBase.Dispose();
            xBase = null;
        }// NoPassword_Logs_ReportEvent_Information_User_Tables_Ok()
        #endregion

        #region Base creation tests
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void CreateBase_NullFilePath_OrionException()
        {
            OrionException xOrionException;

            xOrionException = null;

            try
            {
                OrionDatabaseSQLite.CreateDatabaseFile(null);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The SQLite database file path can't be null or an empty string.");
        }// CreateBase_NullFilePath_OrionException()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void CreateBase_InvalidFilePath_OrionException()
        {
            String strTargetBaseFile;
            OrionException xOrionException;

            strTargetBaseFile = "c:\\|Datas.dtx";
            xOrionException = null;

            try
            {
                OrionDatabaseSQLite.CreateDatabaseFile(strTargetBaseFile);
            }
            catch (Exception ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The database file path is invalid.");
            Assert.IsInstanceOfType(xOrionException.InnerException, typeof(ArgumentException));
            Assert.AreEqual(xOrionException.InnerException.Message, "Caractères non conformes dans le chemin d'accès.");
            Assert.AreEqual(xOrionException.Data["DatabaseFilePath"], strTargetBaseFile);
        }// CreateBase_InvalidFilePath_OrionException()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void CreateBase_MissingDirectoryPath_OrionException()
        {
            String strTargetBaseFile;
            OrionException xOrionException;

            strTargetBaseFile = "z:\\Datas.dtx";
            xOrionException = null;

            try
            {
                OrionDatabaseSQLite.CreateDatabaseFile(strTargetBaseFile);
            }
            catch (Exception ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The database directory path can't be found.");
            Assert.AreEqual(xOrionException.Data["DatabaseFilePath"], strTargetBaseFile);
        }// CreateBase_MissingDirectoryPath_OrionException()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void CreateBase_ExistingFile_OrionException()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, "Create Database Existing File", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                OrionDatabaseSQLite.CreateDatabaseFile(strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The specified SQLite database file already exists.");
            Assert.AreEqual(xOrionException.Data["DatabaseFilePath"], strTargetBaseFilePath);
        }// CreateBase_ExistingFile_OrionException()
        #endregion

        #region Base initialization
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void InitializeBase_NullFilePath_OrionException()
        {
            OrionException xException;
            OrionDatabaseSQLite xBase;

            xException = null;

            try
            {
                xBase = new OrionDatabaseSQLite(null);
            }
            catch (Exception ex)
            {
                xException = ex as OrionException;
            }
            Assert.IsNotNull(xException);
            Assert.AreEqual(xException.Message, "The SQLite database file path can't be null or an empty string.");
        }// InitializeBase_NullFilePath_OrionException()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void InitializeBase_InvalidFilePath_OrionException()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = "c:\\|Datas.dtx";
            xOrionException = null;

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The database file path is invalid.");
            Assert.IsInstanceOfType(xOrionException.InnerException, typeof(ArgumentException));
            Assert.AreEqual(xOrionException.InnerException.Message, "Caractères non conformes dans le chemin d'accès.");
            Assert.AreEqual(xOrionException.Data["DatabaseFilePath"], strTargetBaseFilePath);
        }// InitializeBase_InvalidFilePath_OrionException()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void InitializeBase_MissingDatabaseDirectory_OrionException()
        {
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            xOrionException = null;

            try
            {
                xBase = new OrionDatabaseSQLite("Z:\\Datas.dtx");
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The database directory path can't be found.");
            Assert.AreEqual(xOrionException.Data.Contains("DatabaseFilePath"), true);
            Assert.AreEqual(xOrionException.Data["DatabaseFilePath"], "Z:\\Datas.dtx");
        }// InitializeBase_MissingDatabaseDirectory_OrionException()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void InitializeBase_MissingDatabaseFile_XException()
        {
            String strTargetBaseFilePath;
            Exception xException;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Missing Database", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xException = null;
            xOrionException = null;

            try
            {
                if (File.Exists(strTargetBaseFilePath) == true) File.Delete(strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException, "Can't delete existing test database file;");

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The specified SQLite database file is missing.");
            Assert.AreEqual(xOrionException.Data.Contains("DatabaseFilePath"), true);
            Assert.AreEqual(xOrionException.Data["DatabaseFilePath"], strTargetBaseFilePath);
        }// InitializeBase_MissingDatabaseFile_XException()
        #endregion

        #region No Password Initializations
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_CreateBase_ValidFilePath_IsCreated()
        {
            String strTargetBaseFilePath;
            Exception xException;
            OrionException xOrionException;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Create Database", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xException = null;
            xOrionException = null;

            try
            {
                if (File.Exists(strTargetBaseFilePath) == true) File.Delete(strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException, "Can't delete existing test database file;");

            try
            {
                OrionDatabaseSQLite.CreateDatabaseFile(strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(File.Exists(strTargetBaseFilePath));
        }// NoPassword_CreateBase_ValidFilePath_IsCreated()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_InitializeBase_ValidFilePath_Ok()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Initialize Database", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_InitializeBase_ValidFilePath_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_ConnectBase_ValidInformations_ConnectedDisconnected()
        {
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Connect Database", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.Connect();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                OrionSelectQuery xQuery = xBase.PrepareQuerySelect("T_OrionDatabases_Tests");
                xResults = (DataTable)xQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 12);

            try
            {
                xBase.Disconnect();
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            if (xBase != null) xBase.Dispose();
        }// NoPassword_ConnectBase_ValidInformations_ConnectedDisconnected()
        #endregion

        #region Password initializations
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void Password_CreateBase_ValidFilePath_IsCreated()
        {
            String strTargetBaseFilePath;
            Exception xException;
            OrionException xOrionException;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsPasswordDirectoryPath, "Create Database", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xException = null;
            xOrionException = null;

            try
            {
                if (File.Exists(strTargetBaseFilePath) == true) File.Delete(strTargetBaseFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException, "Can't delete existing test database file;");

            try
            {
                OrionDatabaseSQLite.CreateDatabaseFile(strTargetBaseFilePath, OrionDatabaseSQLiteTests.strPASSWORD);
            }
            catch (Exception ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsTrue(File.Exists(strTargetBaseFilePath));
        }// Password_CreateBase_ValidFilePath_IsCreated()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void Password_InitializeBase_ValidFilePath_Ok()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsPasswordDirectoryPath, "Initialize Database", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath, OrionDatabaseSQLiteTests.strPASSWORD);
            }
            catch (Exception ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// Password_InitializeBase_ValidFilePath_Ok()  
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void Password_ConnectBase_ValidInformations_ConnectedDisconnected()
        {
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsPasswordDirectoryPath, "Connect Database", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath, true);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath, OrionDatabaseSQLiteTests.strPASSWORD);
                xBase.Connect();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xBase.StartTransaction();
                OrionSelectQuery xQuery = xBase.PrepareQuerySelect("T_OrionDatabases_Tests");
                xResults = (DataTable)xQuery.Execute();
                xBase.CommitTransaction();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 12);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            if (xBase != null) xBase.Dispose();
        }// Password_ConnectBase_ValidInformations_ConnectedDisconnected()
        #endregion

        #region Delete queries
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Delete_NoTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Delete Queries", "No Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryDelete(null);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Delete query needs a table name.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Delete_NoTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Delete_UnknownTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Delete Queries", "Unknown Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryDelete("T_Foo").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.IsInstanceOfType(xOrionException.InnerException, typeof(SQLiteException));
            Assert.AreEqual(xOrionException.InnerException.Message, "SQL logic error" + Environment.NewLine + "no such table: T_Foo");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Delete_UnknownTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Delete_WholeTable_Ok()
        {
            Int32 iChangedRows;
            Int64 lRowCount;
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            iChangedRows = 0;
            lRowCount = 0L;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Delete Queries", "Whole Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                lRowCount = (Int64)xBase.PrepareQueryRowCount("T_OrionDatabases_Tests").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreNotEqual(lRowCount, 0L);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                iChangedRows = (Int32)xBase.PrepareQueryDelete("T_OrionDatabases_Tests").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(iChangedRows, lRowCount);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                lRowCount = (Int64)xBase.PrepareQueryRowCount("T_OrionDatabases_Tests").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, 0L);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Delete_WholeTable_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Delete_RowWithParameters_Ok()
        {
            Int32 iChangedRows;
            Int64 lRowCount;
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDeleteQuery xDeleteQuery;
            OrionInsertQuery xInsertQuery;
            OrionRowCountQuery xRowCountQuery;
            OrionDatabaseSQLite xBase;

            iChangedRows = 0;
            lRowCount = 0L;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Delete Queries", "Row With Parameters", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xInsertQuery = null;
            xRowCountQuery = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xRowCountQuery = xBase.PrepareQueryRowCount("T_OrionDatabases_Tests");
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, Commons.NewRowValues.Count);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xInsertQuery = xBase.PrepareQueryInsert("T_OrionDatabases_Tests", "Id", "Titre", "Nationalite", "titre_Original");
                xInsertQuery["Id"] = 666;
                xInsertQuery["Titre"] = "Les Charlots Contre Dracula";
                xInsertQuery["Nationalite"] = "FR";
                xInsertQuery["Titre_Original"] = "Les Charlots Contre Dracula";
                iChangedRows = (Int32)xInsertQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(iChangedRows, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, Commons.NewRowValues.Count + 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xDeleteQuery = xBase.PrepareQueryDelete("T_OrionDatabases_Tests", "Id=@idvalue");
                xDeleteQuery.AddParameter("idvalue", 666);
                iChangedRows = (Int32)xDeleteQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(iChangedRows, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, Commons.NewRowValues.Count);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            xBase.Dispose();
        }// NoPassword_Delete_WholeTable()
        #endregion

        #region Execute queries
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Execute_NoQuery_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Execute Queries", "No Query", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath, true);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryExecute(null);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Query needs a query string to be initialized.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Execute_NoQuery_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Execute_BadQuery_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Execute Queries", "Bad Query", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryExecute("SELECT * FROM Fool").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The SQL query should not be a SELECT one;");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Execute_BadQuery_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Execute_OK()
        {
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Execute Queries", "OK", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xResults = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_OrionDatabases_Tests WHERE Nationalite='FR';").Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 4);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xBase.PrepareQueryExecute("DELETE FROM T_OrionDatabases_Tests WHERE Titre='La Haine';").Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xBase.PersistentConnection = false;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_OrionDatabases_Tests WHERE Nationalite='FR';").Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 3);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Execute_OK()
        #endregion

        #region Insert queries
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Insert_NoTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries", "No Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath, OrionDatabaseSQLiteTests.strPASSWORD);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryInsert(null);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Insert query needs a table name.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Insert_NoTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Insert_UnknownTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries", "Unknown Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryInsert("T_Foo", "Id").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.IsNotNull(xOrionException.InnerException);
            Assert.AreEqual(xOrionException.InnerException.Message, "SQL logic error\r\nno such table: T_Foo");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Insert_UnknownTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Insert_NoFieldNames_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries", "No Field Names", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryInsert("T_Activities", new String[] { });
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Insert query needs field names.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Insert_NoFieldNames_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Insert_NoRow_Error()
        {
            String strTargetBaseFilePath;
            DataRow xRow;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries", "No Row", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xRow = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryInsert("T_Activities", xRow);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Insert query needs a source DataRow.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Insert_NoRow_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Insert_WholeRow_Ok()
        {
            Int32 iChangedRows;
            Int64 lRowCount, lRowCount2;
            String strTargetBaseFilePath;
            DataRow xNewRowTemp;
            DataTable xResults;
            OrionDeleteQuery xDeleteQuery;
            OrionRowCountQuery xRowCountQuery;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            iChangedRows = 0;
            lRowCount = 0L;
            lRowCount2 = 0L;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries", "Whole Row", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xRowCountQuery = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xRowCountQuery = xBase.PrepareQueryRowCount("T_OrionDatabases_Tests");
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_OrionDatabases_Tests WHERE Id=-1").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xNewRowTemp = xResults.NewRow();
                xNewRowTemp["Id"] = 666;
                xNewRowTemp["Titre"] = "Les Charlots Contre Dracula";
                xNewRowTemp["Nationalite"] = "FR";
                xNewRowTemp["Titre_Original"] = "Les Charlots Contre Dracula";
                xResults.Rows.Add(xNewRowTemp);

                iChangedRows = (Int32)xBase.PrepareQueryInsert("T_OrionDatabases_Tests", xNewRowTemp).Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(iChangedRows, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount2 = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount2, lRowCount + 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xDeleteQuery = xBase.PrepareQueryDelete("T_OrionDatabases_Tests", "Id>=@param1");
                xDeleteQuery.AddParameter("Param1", 666);
                xDeleteQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, Commons.NewRowValues.Count);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            xBase.Dispose();
        }// NoPassword_Insert_WholeRow_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Insert_RowByParameters_Ok()
        {
            Int32 iChangedRows;
            Int64 lRowCount, lRowCount2;
            String strTargetBaseFilePath;
            OrionDeleteQuery xDeleteQuery;
            OrionInsertQuery xInsertQuery;
            OrionRowCountQuery xRowCountQuery;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            iChangedRows = 0;
            lRowCount = 0L;
            lRowCount2 = 0L;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries", "Row By Parameters", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xRowCountQuery = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xRowCountQuery = xBase.PrepareQueryRowCount("T_OrionDatabases_Tests");
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xInsertQuery = xBase.PrepareQueryInsert("T_OrionDatabases_Tests", "Id", "Titre", "Nationalite", "Titre_Original");
                xInsertQuery["Id"] = 666;
                xInsertQuery["Titre"] = "Les Charlots Contre Dracula";
                xInsertQuery["Nationalite"] = "FR";
                xInsertQuery["Titre_Original"] = "Les Charlots Contre Dracula";

                iChangedRows = (Int32)xInsertQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(iChangedRows, 1L);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount2 = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount2, lRowCount + 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xDeleteQuery = xBase.PrepareQueryDelete("T_OrionDatabases_Tests", "Id>=@param1");
                xDeleteQuery.AddParameter("Param1", 666);
                xDeleteQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, Commons.NewRowValues.Count);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            xBase.Dispose();
        }// NoPassword_Insert_RowByParameters_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Insert_RowByParameters_Get_NewId_Ok()
        {
            Int64 lNewId, lRowCount, lRowCount2;
            String strTargetBaseFilePath;
            OrionDeleteQuery xDeleteQuery;
            OrionInsertQuery xInsertQuery;
            OrionRowCountQuery xRowCountQuery;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            lNewId = 0L;
            lRowCount = 0L;
            lRowCount2 = 0L;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Insert Queries", "Row By Parameters Get New Id Ok", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xRowCountQuery = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                if (xBase != null) xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xRowCountQuery = xBase.PrepareQueryRowCount("T_Movies");
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xInsertQuery = xBase.PrepareQueryInsert("T_Movies", "TitleOriginal", "TitleFrench", "Country", "Year");
                xInsertQuery["TitleOriginal"] = "Backdraft";
                xInsertQuery["TitleFrench"] = "Backdraft";
                xInsertQuery["Country"] = "US";
                xInsertQuery["year"] = 1989;

                lNewId = (Int64)xInsertQuery.Execute("Id");
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lNewId, 21L);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount2 = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount2, lRowCount + 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xDeleteQuery = xBase.PrepareQueryDelete("T_Movies", "Id>=@param1");
                xDeleteQuery.AddParameter("Param1", 21L);
                xDeleteQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                lRowCount = (Int64)xRowCountQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, lRowCount);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            xBase.Dispose();
        }// NoPassword_Insert_RowByParameters_Get_NewId_Ok()
        #endregion

        #region RowCount Query
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_RowCount_NoTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "RowCount Queries", "No Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryRowCount(null);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "RowCount query needs a table name.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_RowCount_NoTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_RowCount_UnknownTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "RowCount Queries", "Unknown Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryRowCount("T_Foo").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.InnerException.Message, "SQL logic error" + Environment.NewLine + "no such table: T_Foo");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_RowCount_UnknownTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_RowCount_With_Parameters()
        {
            Int64 lRowCount;
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;
            OrionRowCountQuery xQuery;

            lRowCount = 0;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "RowCount Queries", "With Parameters", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xQuery = xBase.PrepareQueryRowCount("T_OrionDatabases_Tests", "Nationalite=@param1");
                xQuery.AddParameter("Param1", "FR");
                lRowCount = (Int64)xQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, Commons.NewRowValues.Where(x => x.Country == "FR").Count());
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_RowCount_With_Parameters()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_RowCount_Whole_Table_Ok()
        {
            Int64 lRowCount;
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            lRowCount = 0L;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "RowCount Queries", "Ok", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                lRowCount = (Int64)xBase.PrepareQueryRowCount("T_OrionDatabases_Tests").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(lRowCount, Commons.NewRowValues.Count);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_RowCount_Whole_Table_Ok()
        #endregion

        #region Select queries
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Select_NoQuery_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Select Queries", "No Query", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQuerySelect(null);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Query needs a query string to be initialized.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Select_NoQuery_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Select_BadQuery_Error()
        {
            String strSqlQuery, strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strSqlQuery = null;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Select Queries", "Bad Query", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                strSqlQuery = "DELETE * FROM Fool";
                xResults = (DataTable)xBase.PrepareQuerySelect(strSqlQuery).Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "The SQL query is not a valid SELECT one;");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Select_BadQuery_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Select_Ok()
        {
            String strSqlQuery, strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Select Queries", "Ok", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                strSqlQuery = "SELECT * FROM T_Directors";
                xResults = (DataTable)xBase.PrepareQuerySelect(strSqlQuery).Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 16);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Select_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Select_WithParameters_Ok()
        {
            String strSqlQuery, strTargetBaseFilePath;
            DataTable xResults;
            OrionSelectQuery xQuery;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strSqlQuery = null;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Select Queries", "With Parameters Ok", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                strSqlQuery = "SELECT * FROM T_Directors WHERE FirstName=@Param1 AND LastName=@Param2";
                xQuery = xBase.PrepareQuerySelect(strSqlQuery);
                xQuery.AddParameter("Param1", "Stanley");
                xQuery.AddParameter("Param2", "Kubrick");

                xResults = (DataTable)xQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Select_WithParameters_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Select_WithMissingParameter_Ok()
        {
            String strSqlQuery, strTargetBaseFilePath;
            OrionSelectQuery xQuery;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strSqlQuery = null;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Select Queries", "With Missing Parameter", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                strSqlQuery = "SELECT * FROM T_Directors WHERE FirstName=@Param1 AND LastName=@Param2";
                xQuery = xBase.PrepareQuerySelect(strSqlQuery);
                xQuery.AddParameter("@Param1", "Stanley");

                xQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.IsInstanceOfType(xOrionException.InnerException, typeof(SQLiteException));
            Assert.AreEqual(xOrionException.InnerException.Message, "unknown error\r\nInsufficient parameters supplied to the command");
            Assert.IsNotNull(xOrionException.Data);
            Assert.AreEqual(xOrionException.Data.Count, 1);
            Assert.IsTrue(xOrionException.Data.Contains("SqlQuery"));
            Assert.AreEqual(xOrionException.Data["SqlQuery"], strSqlQuery);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Select_WithMissingParameter_Ok()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Select_WithUnnamedParameter_Ok()
        {
            String strSqlQuery, strTargetBaseFilePath;
            OrionSelectQuery xQuery;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strSqlQuery = null;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Select Queries", "With Unnamed Parameter", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                strSqlQuery = "SELECT * FROM T_Directors WHERE FirstName=@Param1 AND LastName=@Param2";
                xQuery = xBase.PrepareQuerySelect(strSqlQuery);
                xQuery.AddParameter(null, "Stanley");
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Parameter name must be provided;");
            Assert.IsNotNull(xOrionException.Data);
            Assert.AreEqual(xOrionException.Data.Count, 1);
            Assert.IsTrue(xOrionException.Data.Contains("SqlQuery"));
            Assert.AreEqual(xOrionException.Data["SqlQuery"], strSqlQuery);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Select_WithUnnamedParameter_Ok()
        #endregion

        #region Transactions
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Transactions_Ok()
        {
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionSelectQuery xQuery;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Transactions", "OK", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_OrionDatabases_Tests WHERE Nationalite='FR';").Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 4);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xBase.StartTransaction();
                xBase.PrepareQueryExecute("DELETE FROM T_OrionDatabases_Tests WHERE Titre='La Haine';").Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xBase.PrepareQueryExecute("DELETE FROM T_OrionDatabases_Tests WHERE Titre='Léon';").Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xBase.CommitTransaction();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);

            try
            {
                xBase.PersistentConnection = false;
                xResults = (DataTable)xBase.PrepareQuerySelect("SELECT * FROM T_OrionDatabases_Tests WHERE Nationalite='FR';").Execute();
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.IsNotNull(xResults);
            Assert.AreEqual(xResults.Rows.Count, 2);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Transactions_Ok()
        #endregion

        #region Update Queries
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Update_NoTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Update Queries", "No Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryUpdate(null, "Id=@Param1", "Id", "Titre", "Nationalite", "Titre_Original");
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Update query needs a table name.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Update_NoTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Update_NoFieldNames_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Update Queries", "No Field Names", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryUpdate("T_OrionDatabases_Tests", "Id=@Param1", null);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.AreEqual(xOrionException.Message, "Update query needs field names.");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Update_NoFieldNames_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Update_UnknownTable_Error()
        {
            String strTargetBaseFilePath;
            OrionException xOrionException;
            OrionDatabaseSQLite xBase;

            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Update Queries", "Unknown Table", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xOrionException = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
            }
            catch (OrionException ex)
            {
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                xBase.PrepareQueryUpdate("T_Foo", "Id=@Param1", "Id", "Titre", "Nationalite", "Titre_Original").Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.IsNotNull(xOrionException.InnerException);
            Assert.AreEqual(xOrionException.InnerException.Message, "SQL logic error" + Environment.NewLine + "no such table: T_Foo");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// NoPassword_Update_UnknownTable_Error()
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void NoPassword_Update_Row_Ok()
        {
            Int32 iChangedRows;
            String strTargetBaseFilePath;
            DataTable xResults;
            OrionException xOrionException;
            OrionSelectQuery xSelectQuery;
            OrionUpdateQuery xUpdateQuery;
            OrionDatabaseSQLite xBase;

            iChangedRows = 0;
            strTargetBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strTestsNoPasswordDirectoryPath, "Update Queries", "Row", OrionDatabaseSQLiteTests.strTESTBASEFILENAME);
            xResults = null;
            xOrionException = null;
            xSelectQuery = null;
            xBase = null;

            OrionDatabaseSQLiteTests.CheckTestDatabaseFile(strTargetBaseFilePath);

            try
            {
                xBase = new OrionDatabaseSQLite(strTargetBaseFilePath);
                xBase.PersistentConnection = true;
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            try
            {
                Commons.PopulateTable(xBase, "T_OrionDatabases_Tests", true);
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xSelectQuery = xBase.PrepareQuerySelect("SELECT * FROM T_OrionDatabases_Tests WHERE Id=@Param1");
                xSelectQuery.AddParameter("Param1", 2);
                xResults = (DataTable)xSelectQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xResults.Rows.Count, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xUpdateQuery = xBase.PrepareQueryUpdate("T_OrionDatabases_Tests", "Id=@Param1", "Id");
                xUpdateQuery.AddParameter("Param1", 2);
                xUpdateQuery["Id"] = 666;
                iChangedRows = (Int32)xUpdateQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(iChangedRows, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xSelectQuery["Param1"] = 2;
                xResults = (DataTable)xSelectQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xResults.Rows.Count, 0);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                iChangedRows = 0;
                xUpdateQuery = xBase.PrepareQueryUpdate("T_OrionDatabases_Tests", "Id=@Param1", "Id");
                xUpdateQuery.AddParameter("Param1", 666);
                xUpdateQuery["Id"] = 2;
                iChangedRows = (Int32)xUpdateQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(iChangedRows, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            try
            {
                xSelectQuery["Param1"] = 2;
                xResults = (DataTable)xSelectQuery.Execute();
            }
            catch (OrionException ex)
            {
                xBase.Dispose();
                xOrionException = ex as OrionException;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xResults.Rows.Count, 1);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Open);

            xBase.Dispose();
        }// NoPassword_Update_Row_Ok()
        #endregion
        #endregion

        #region Utility procedures
        private static void CheckTestDatabaseFile(String targetBaseFilePath, Boolean password = false)
        {
            String strSourceBaseFilePath, strTargetBaseDirectoryPath;
            Exception xException;

            xException = null;

            if (password == false)
                strSourceBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strSourceDirectoryPath, OrionDatabaseSQLiteTests.strSOURCENOPASSWORDBASEFILENAME);
            else
                strSourceBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strSourceDirectoryPath, OrionDatabaseSQLiteTests.strSOURCEPASSWORDBASEFILENAME);
            strTargetBaseDirectoryPath = Path.GetDirectoryName(targetBaseFilePath);

            if (Directory.Exists(strTargetBaseDirectoryPath) == false) Directory.CreateDirectory(strTargetBaseDirectoryPath);

            try
            {
                if (File.Exists(targetBaseFilePath) == true) File.Delete(targetBaseFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException, "Can't delete existing database file;");

            try
            {
                File.Copy(strSourceBaseFilePath, targetBaseFilePath);
            }
            catch (Exception ex)
            {
                xException = ex;
            }
            Assert.IsNull(xException, "Can't copy source database file;");
        }// CheckTestDatabaseFile()
        private static void InitializeDirectories(String rootDirectoryPath, String[] strDirectoryNames)
        {
            String strTargetDirectoryPath;

            if (Directory.Exists(rootDirectoryPath) == false) Directory.CreateDirectory(rootDirectoryPath);

            foreach (String strDirectoryNameTemp in strDirectoryNames)
            {
                strTargetDirectoryPath = Path.Combine(rootDirectoryPath, strDirectoryNameTemp);
                if (Directory.Exists(strTargetDirectoryPath) == false) Directory.CreateDirectory(strTargetDirectoryPath);
            }
        }// InitializeDirectories()
        #endregion
    }
}
