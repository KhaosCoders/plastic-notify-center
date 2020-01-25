using System;

namespace PlasticNotifyCenter.Notifiers
{
    /// <summary>
    /// Attribute used on notifier data models to specify which notifier it is used for
    /// </summary>
    public class NotifierAttribute : Attribute
    {
        /// <summary>
        /// Gets a unique identifier for the notifier type
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets a name for the notifier type
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets an icon HTML code used for the notifier type
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// Gets the Type of the notifier
        /// </summary>
        public Type NotifierType { get; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="notifierType">Type of notifier this data model is used for</param>
        /// <param name="id">Unique identifier for the type of notifier</param>
        /// <param name="name">Name of the notifier type</param>
        /// <param name="icon">Icon HTML code for the type of notifier</param>
        public NotifierAttribute(Type notifierType, string id, string name, string icon)
        {
            if (!typeof(INotifier).IsAssignableFrom(notifierType))
            {
                throw new InvalidOperationException("notifierType must be of kind INotifier");
            }
            NotifierType = notifierType;
            Id = id;
            Name = name;
            Icon = icon;
        }
    }
}