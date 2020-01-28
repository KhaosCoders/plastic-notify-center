using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Services;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Manages notifiers
    /// </summary>
    public interface INotifierManager
    {
        /// <summary>
        /// Gets a ordered list of all notifiers
        /// </summary>
        IEnumerable<NotifierInfo> GetOrderedNotifiers();

        /// <summary>
        /// Returns a notifier by Id. Or null, if no notifier was found.
        /// </summary>
        /// <param name="id">ID of requested notifier</param>
        BaseNotifierData GetNotifierById(string id);

        /// <summary>
        /// Returns the first notifier
        /// </summary>
        BaseNotifierData GetFirstNotifier();

        /// <summary>
        /// Saves changes to a notifier
        /// </summary>
        /// <param name="id">ID of changed notifier</param>
        /// <param name="jsonBody">JSON string with notifier properties and values</param>
        Task<int> ChangeNotifierAsync(string id, string jsonBody);

        /// <summary>
        /// Creates a new notifier of the provided type
        /// </summary>
        /// <param name="typeId">ID or notifier type</param>
        Task<string> NewNotifierAsync(string typeId);

        /// <summary>
        /// Deleted a notifier by Id
        /// </summary>
        /// <param name="id">ID of notifier to delete</param>
        Task DeleteNotifierByIdAsync(string id);
    }

    /// <summary>
    /// Implementation of INotifierManager
    /// </summary>
    public class NotifierManager : INotifierManager
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;
        private readonly INotifierDefinitionService _notifierDefinitionService;

        public NotifierManager(PncDbContext dbContext,
                            INotifierDefinitionService notifierDefinitionService)
        {
            _dbContext = dbContext;
            _notifierDefinitionService = notifierDefinitionService;
        }

        #endregion

        #region Notifier list

        /// <summary>
        /// Gets a ordered list of all notifiers
        /// </summary>
        public IEnumerable<NotifierInfo> GetOrderedNotifiers() =>
            _dbContext.Notifiers
                .OrderBy(notifier => notifier.DisplayName)
                .ToList()
                .Select(n => new NotifierInfo()
                {
                    Id = n.Id,
                    Name = n.DisplayName,
                    Icon = _notifierDefinitionService.GetIcon(n.GetType())
                });

        #endregion

        #region Get notifier

        /// <summary>
        /// Returns a notifier by Id. Or null, if no notifier was found.
        /// </summary>
        /// <param name="id">ID of requested notifier</param>
        public BaseNotifierData GetNotifierById(string id) =>
            _dbContext.Notifiers
                .FirstOrDefault(n => n.Id.Equals(id));

        /// <summary>
        /// Returns the first notifier
        /// </summary>
        public BaseNotifierData GetFirstNotifier() =>
            _dbContext.Notifiers
                .OrderBy(notifier => notifier.DisplayName)
                .FirstOrDefault();

        #endregion

        #region Change notifier

        /// <summary>
        /// Saves changes to a notifier
        /// </summary>
        /// <param name="id">ID of changed notifier</param>
        /// <param name="jsonBody">JSON string with notifier properties and values</param>
        public async Task<int> ChangeNotifierAsync(string id, string jsonBody)
        {
            // Find notifier by Id
            BaseNotifierData notifier = GetNotifierById(id);
            if (notifier == null)
            {
                throw new KeyNotFoundException("Notifier not found");
            }

            // Apply changes
            await notifier.ApplyJsonPropertiesAsync(jsonBody);

            // Save changes
            return await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region New notifier

        /// <summary>
        /// Creates a new notifier of the provided type
        /// </summary>
        /// <param name="typeId">ID or notifier type</param>
        /// <exception cref=""></exception>
        public async Task<string> NewNotifierAsync(string typeId)
        {
            // Find the type of notifier data
            Type notifierDataType = _notifierDefinitionService.GetNotifierDataType(typeId);
            if (notifierDataType == null)
            {
                throw new TypeAccessException("Notifier type is unknown");
            }

            // Get name of notifier type
            string displayName = $"New {_notifierDefinitionService.GetNotifierTypeName(typeId) ?? "Notifier"}";

            // Create a new instance of the notifier data type
            BaseNotifierData notifier = Activator.CreateInstance(notifierDataType, new object[] { displayName }) as BaseNotifierData;
            if (notifier == null || string.IsNullOrWhiteSpace(notifier.Id))
            {
                throw new TypeAccessException("Can't create notifier data type");
            }

            // Add notifier
            await _dbContext.Notifiers.AddAsync(notifier);

            // Save
            await _dbContext.SaveChangesAsync();

            return notifier.Id;
        }

        #endregion

        #region Delete notifier

        /// <summary>
        /// Deleted a notifier by Id
        /// </summary>
        /// <param name="id">ID of notifier to delete</param>
        public async Task DeleteNotifierByIdAsync(string id)
        {
            // Find notifier by Id
            BaseNotifierData notifier = _dbContext.Notifiers.FirstOrDefault(n => n.Id.Equals(id));
            if (notifier == null)
            {
                throw new KeyNotFoundException("Notifier not found");
            }

            // Remove notifier
            _dbContext.Notifiers.Remove(notifier);

            // Save
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}