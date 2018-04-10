using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrionCore.ErrorManagement;
using OrionDatabases.Queries;
using OrionDatabases;
using OrionCore;

namespace OrionDatabasesTests
{
    [TestClass()]
    public static class Commons
    {
        #region Structures
        internal struct StructNewRowValue
        {
            #region Properties
            internal Int32 Id { get; private set; }
            internal String Title { get; private set; }
            internal String Country { get; private set; }
            internal String OriginalTitle { get; private set; }
            #endregion

            #region Constructors
            internal StructNewRowValue(Int32 id, String title, String country, String originalTitle)
            {
                this.Id = id;
                this.Title = title;
                this.Country = country;
                this.OriginalTitle = originalTitle;
            }// StructNewRowValue()
            #endregion
        }
        #endregion

        #region Properties
        internal static List<StructNewRowValue> NewRowValues { get; private set; }
        #endregion

        #region Initializations & cleanup
        [AssemblyInitialize()]
        public static void Initialize(TestContext context)
        {
            OrionException xOrionException;
            OrionDatabase xLocalOrionDatabase;

            xOrionException = null;
            xLocalOrionDatabase = null;

            Commons.NewRowValues = new List<StructNewRowValue>();
            Commons.NewRowValues.Add(new StructNewRowValue(1, "La Guerre Des Etoiles", "USA", "Star Wars"));
            Commons.NewRowValues.Add(new StructNewRowValue(2, "Les Dents De La Mer", "USA", "Jaws"));
            Commons.NewRowValues.Add(new StructNewRowValue(3, "Léon", "FR", "Léon"));
            Commons.NewRowValues.Add(new StructNewRowValue(4, "Le Bâteau", "DE", "Das Boot"));
            Commons.NewRowValues.Add(new StructNewRowValue(5, "Apocalypse Now", "USA", "Apocalypse Now"));
            Commons.NewRowValues.Add(new StructNewRowValue(6, "Il Etait Une Fois En Amérique", "USA", "Once Upon A Time In America"));
            Commons.NewRowValues.Add(new StructNewRowValue(7, "Gremlins", "USA", "Gremlins"));
            Commons.NewRowValues.Add(new StructNewRowValue(8, "La Haine", "FR", "La Haine"));
            Commons.NewRowValues.Add(new StructNewRowValue(9, "Willow", "USA", "Willow"));
            Commons.NewRowValues.Add(new StructNewRowValue(10, "Le Samouraï", "FR", "Le Samouraï"));
            Commons.NewRowValues.Add(new StructNewRowValue(11, "L'armée Des ombres", "FR", "L'armée Des ombres"));
            Commons.NewRowValues.Add(new StructNewRowValue(12, "Casino", "USA", "Casino"));

            //** Sqlite No Password. **
            try
            {
                xLocalOrionDatabase = new OrionDatabaseSQLite(Path.Combine(OrionDeploymentInfos.DataFolder, "OrionDatabaseSqliteTests", "Source", OrionDatabaseSQLiteTests.strSOURCENOPASSWORDBASEFILENAME));

                Commons.PopulateTable(xLocalOrionDatabase, "T_OrionDatabases_Tests", true);
            }
            catch (OrionException ex)
            {
                xOrionException = ex;
            }
            finally
            {
                if (xLocalOrionDatabase != null) xLocalOrionDatabase.Dispose();
            }
            Assert.IsNull(xOrionException);
        }// Initialize()
        [AssemblyCleanup()]
        public static void Cleanup()
        {
        }// Cleanup()
        #endregion

        #region Internal methods
        internal static void PopulateTable(OrionDatabase sourceBase, String tableName, Boolean clearTable)
        {
            OrionInsertQuery xQueryTemp;

            if (clearTable == true) sourceBase.PrepareQueryDelete(tableName).Execute();

            xQueryTemp = sourceBase.PrepareQueryInsert(tableName, "id", "titre", "nationalite", "titre_original");

            foreach (StructNewRowValue xNewRowValuesTemp in Commons.NewRowValues)
            {
                xQueryTemp["id"] = xNewRowValuesTemp.Id;
                xQueryTemp["titre"] = xNewRowValuesTemp.Title;
                xQueryTemp["nationalite"] = xNewRowValuesTemp.Country;
                xQueryTemp["titre_original"] = xNewRowValuesTemp.OriginalTitle;

                xQueryTemp.Execute();
            }
        }// PopulateTable()
        #endregion
    }
}