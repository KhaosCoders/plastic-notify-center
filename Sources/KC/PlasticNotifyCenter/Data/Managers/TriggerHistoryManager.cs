using System.Collections.Generic;
using System.Linq;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Manages the history of trigger calls
    /// </summary>
    public interface ITriggerHistoryManager
    {
        /// <summary>
        /// Returns a list of stats for trigger calls
        /// </summary>
        IEnumerable<TriggerStats> GetTriggerStats();

        /// <summary>
        /// Return a list of all known trigger types
        /// </summary>
        IEnumerable<string> GetAllTriggerTypes();

        /// <summary>
        /// Returns a list of all known envirmonment variables for a trigger type
        /// </summary>
        /// <param name="trigger">Name of trigger type</param>
        IEnumerable<TriggerVariable> GetEnvironmentVariables(string trigger);

        /// <summary>
        /// Returns the most recent entry from the history of trigger calls for the desired type of trigger
        /// </summary>
        /// <param name="trigger">Name of trigger type</param>
        TriggerHistory GetLatestTriggerHistory(string trigger);
    }

    /// <summary>
    /// Implementation of ITriggerHistoryManager
    /// </summary>
    public class TriggerHistoryManager : ITriggerHistoryManager
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;

        public TriggerHistoryManager(PncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Get stats

        /// <summary>
        /// Returns a list of stats for trigger calls
        /// </summary>
        public IEnumerable<TriggerStats> GetTriggerStats() =>
            _dbContext.TriggerHistory
                .GroupBy(o => o.Trigger)
                .Select(g => new TriggerStats()
                {
                    Name = g.Key,
                    Count = g.Count()
                });

        #endregion

        #region Trigger types


        /// <summary>
        /// Return a list of all known trigger types
        /// </summary>
        public IEnumerable<string> GetAllTriggerTypes() =>
            _dbContext.TriggerHistory.GroupBy(r => r.Trigger).Select(g => g.Key);

        #endregion

        #region Trigger variables

        /// <summary>
        /// Returns a list of all known envirmonment variables for a trigger type
        /// </summary>
        /// <param name="trigger">Name of trigger type</param>
        public IEnumerable<TriggerVariable> GetEnvironmentVariables(string trigger) =>
            _dbContext.TriggerVariables
                .Where(v => v.Trigger == trigger)
                .OrderBy(v => v.Variable);

        /// <summary>
        /// Returns the most recent entry from the history of trigger calls for the desired type of trigger
        /// </summary>
        /// <param name="trigger">Name of trigger type</param>
        public TriggerHistory GetLatestTriggerHistory(string trigger) =>
            _dbContext.TriggerHistory
                .Where(h => h.Trigger == trigger)
                .OrderByDescending(h => h.TimeStamp)
                .FirstOrDefault();

        #endregion

    }
}