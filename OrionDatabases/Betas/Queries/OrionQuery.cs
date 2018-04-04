using System;

namespace OrionDatabases.Queries
{
    /// <summary>
    /// Base class used to describe a query.
    /// </summary>
    public abstract class OrionQuery
    {
        #region Properties
        public Boolean Executed { get; internal set; }
        public Boolean Failed { get; protected set; }
        public QueryTypes QueryType { get; protected set; }
        public String QueryString { get; protected set; }
        protected OrionDatabase ParentOrionDatabase { get; }
        #endregion

        #region Constructors
        protected OrionQuery(OrionDatabase parentXDatabase)
        {
            this.ParentOrionDatabase = parentXDatabase;
        }// OrionQuery()
        #endregion

        #region Abstract and virtual interface
        public abstract Object Execute();
        #endregion
    }
}
