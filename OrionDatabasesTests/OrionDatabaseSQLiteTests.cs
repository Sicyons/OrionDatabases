using System;
using System.IO;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrionCore.ErrorManagement;
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
        #endregion

        #region Initializations
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            String strTargetDirectoryPath;
            Exception xException;

            xException = null;
            OrionDatabaseSQLiteTests.strTestsDirectoryPath = Path.Combine(OrionDeploymentInfos.DataFolder, "OrionDatabaseSQLiteTests");
            OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsDirectoryPath, "Miscellaneous");
            OrionDatabaseSQLiteTests.strSourceDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsDirectoryPath, "Source");

            Assert.IsTrue(Directory.Exists(OrionDatabaseSQLiteTests.strSourceDirectoryPath));

            try
            {
                if (Directory.Exists(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath) == false) Directory.CreateDirectory(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath);
                strTargetDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, "Missing Dll");
                if (Directory.Exists(strTargetDirectoryPath) == false) Directory.CreateDirectory(strTargetDirectoryPath);
                strTargetDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, "Set Password");
                if (Directory.Exists(strTargetDirectoryPath) == false) Directory.CreateDirectory(strTargetDirectoryPath);
                //strTargetDirectoryPath = Path.Combine(OrionDatabaseSQLiteTests.strTestsMiscellaneousDirectoryPath, "Create Database Existing File");
                //if (Directory.Exists(strTargetDirectoryPath) == false) Directory.CreateDirectory(strTargetDirectoryPath);
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
        [TestMethod, TestCategory("OrionDatabaseSQLite")]
        public void Miscellaneous_MissingSQLiteDll_XException()
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
        }// Miscellaneous_MissingSQLiteDll_XException()
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
                xBase.Dispose();
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
                xBase.Dispose();
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
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNotNull(xOrionException);
            Assert.IsInstanceOfType(xOrionException.InnerException, typeof(SQLiteException));
            Assert.AreEqual(xOrionException.InnerException.Message, "file is not a database\r\nfile is not a database");
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();

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
                xBase.Dispose();
                xOrionException = ex;
            }
            Assert.IsNull(xOrionException);
            Assert.AreEqual(xResults.Rows.Count, 16);
            Assert.AreEqual(xBase.ConnectionState, ConnectionState.Closed);

            xBase.Dispose();
        }// Miscellaneous_SetPassword_OkDisconnected()
        #endregion
        #endregion

        #region Utility procedures
        private static void CheckTestDatabaseFile(String targetBaseFilePath, Boolean password = false)
        {
            String strSourceBaseFilePath;
            Exception xException;

            xException = null;

            if (password == false)
                strSourceBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strSourceDirectoryPath, OrionDatabaseSQLiteTests.strSOURCENOPASSWORDBASEFILENAME);
            else
                strSourceBaseFilePath = Path.Combine(OrionDatabaseSQLiteTests.strSourceDirectoryPath, OrionDatabaseSQLiteTests.strSOURCEPASSWORDBASEFILENAME);

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
        #endregion
    }
}
