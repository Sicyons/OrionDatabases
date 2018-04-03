using System;

namespace OrionDatabases
{
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
