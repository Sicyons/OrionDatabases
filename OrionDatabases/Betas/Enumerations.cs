using System;

namespace OrionDatabases
{
    /// <summary>
    /// Defines a SQL query type.
    /// </summary>
    public enum QueryTypes
    {
        /// <summary>
        /// The query type has not been identified.
        /// </summary>
        Unknown,
        /// <summary>
        /// DELETE...FROM query type.
        /// </summary>
        Delete,
        /// <summary>
        /// Other query types.
        /// </summary>
        Execute,
        /// <summary>
        /// INSERT INTO...VALUES query type.
        /// </summary>
        Insert,
        /// <summary>
        /// SELECT...FROM...WHERE...GROUP BY...ORDER BY query type.
        /// </summary>
        Select,
        /// <summary>
        /// UPDATE...SET...WHERE query type.
        /// </summary>
        Update,
        /// <summary>
        /// SELECT COUNT(*) FROm table query.
        /// </summary>
        RowCount
    }

    /// <summary>
    /// Defines a current transaction state.
    /// </summary>
    public enum TransactionStates
    {
        /// <summary>
        /// No transaction has been used.
        /// </summary>
        None = 0,
        /// <summary>
        /// A transaction has been started and is neither committed nor rollbacked.
        /// </summary>
        Started = 1,
        /// <summary>
        /// A transaction has been committed.
        /// </summary>
        Committed = 2,
        /// <summary>
        /// A transaction has been rollbacked.
        /// </summary>
        Rollbacked = 3
    }
}
