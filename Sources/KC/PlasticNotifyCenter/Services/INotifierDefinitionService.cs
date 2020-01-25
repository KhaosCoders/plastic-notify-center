using System;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Notifiers;

namespace PlasticNotifyCenter.Services
{
    /// <summary>
    /// A service that finds definitions for notifiers
    /// </summary>
    public interface INotifierDefinitionService
    {
        /// <summary>
        /// Gets an icon (HTML TAG!) for a type of notifier
        /// </summary>
        /// <param name="notifierName">Name of notifier type</param>
        string GetIcon(string notifierName);

        /// <summary>
        /// Gets an icon (HTML TAG!) for a type of notifier
        /// </summary>
        /// <param name="dataType">Type of notifier data</param>
        string GetIcon(Type dataType);

        /// <summary>
        /// Gets the name of a notifier type by Id
        /// </summary>
        /// <param name="typeId">Id of notifier type</param>
        /// <returns></returns>
        string GetNotifierTypeName(string typeId);

        /// <summary>
        /// Gets all notifier types
        /// </summary>
        NotifierAttribute[] GetAllNotifierTypes();

        /// <summary>
        /// Gets a type definition for a notifier
        /// </summary>
        /// <param name="typeId">Id of notifier type</param>
        Type GetNotifierDataType(string typeId);

        /// <summary>
        /// Gets a type definition for a notifier
        /// </summary>
        /// <param name="dataType">Type of notifier data</param>
        NotifierAttribute GetNotifierAttribute(Type dataType);
    }
}