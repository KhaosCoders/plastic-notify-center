using System;
using System.Collections.Generic;
using System.Linq;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Notifiers;

namespace PlasticNotifyCenter.Services
{
    /// <summary>
    /// Implementation of a INotifierIconService
    /// </summary>
    public class NotifierDefinitionService : INotifierDefinitionService
    {
        // A dictionary of all notifier data types with their notifier definitions
        private static readonly Lazy<Dictionary<Type, NotifierAttribute>> notifierDataTypes = new Lazy<Dictionary<Type, NotifierAttribute>>(() =>
            new Dictionary<Type, NotifierAttribute>(
                typeof(BaseNotifierData).Assembly
                    .GetTypes()
                    .Where(t => typeof(BaseNotifierData).IsAssignableFrom(t))
                    .Select(t => new KeyValuePair<Type, NotifierAttribute>(
                        t,
                        t.GetCustomAttributes(false)
                            .Where(a => a is NotifierAttribute)
                            .Cast<NotifierAttribute>()
                            .FirstOrDefault()
                    ))
                    .Where(p => p.Value != null)
            )
        );

        // A dictionary of all notifier attributes by name
        private static readonly Lazy<Dictionary<string, NotifierAttribute>> attributesByName = new Lazy<Dictionary<string, NotifierAttribute>>(() =>
            new Dictionary<string, NotifierAttribute>(
                notifierDataTypes.Value.Values.Select(a => new KeyValuePair<string, NotifierAttribute>(a.Name, a))
            )
        );

        // A dictionary of all notifier attributes by Id
        private static readonly Lazy<Dictionary<string, NotifierAttribute>> attributesById = new Lazy<Dictionary<string, NotifierAttribute>>(() =>
            new Dictionary<string, NotifierAttribute>(
                notifierDataTypes.Value.Values.Select(a => new KeyValuePair<string, NotifierAttribute>(a.Id, a))
            )
        );

        // A dictionary of all notifier data types by Id
        private static readonly Lazy<Dictionary<string, Type>> dataTypesById = new Lazy<Dictionary<string, Type>>(() =>
            new Dictionary<string, Type>(
                notifierDataTypes.Value.Select(p => new KeyValuePair<string, Type>(p.Value.Id, p.Key))
            )
        );

        public NotifierAttribute GetNotifierAttribute(Type dataType)
        {
            if (dataType != null
                && notifierDataTypes.Value.TryGetValue(dataType, out NotifierAttribute def))
            {
                return def;
            }
            return null;
        }

        public NotifierAttribute[] GetAllNotifierTypes() =>
            notifierDataTypes.Value.Values.ToArray();

        public Type GetNotifierDataType(string typeId)
        {
            if (!string.IsNullOrWhiteSpace(typeId)
                && dataTypesById.Value.TryGetValue(typeId, out Type type))
            {
                return type;
            }
            return null;
        }

        public string GetNotifierTypeName(string typeId)
        {
            if (!string.IsNullOrWhiteSpace(typeId)
                && attributesById.Value.TryGetValue(typeId, out NotifierAttribute def))
            {
                return def.Name;
            }
            return string.Empty;
        }

        public string GetIcon(string notifierName)
        {
            if (!string.IsNullOrWhiteSpace(notifierName)
                && attributesByName.Value.TryGetValue(notifierName, out NotifierAttribute def))
            {
                return def.Icon;
            }
            return string.Empty;
        }

        public string GetIcon(Type dataType)
        {
            if (dataType != null
               && notifierDataTypes.Value.TryGetValue(dataType, out NotifierAttribute def))
            {
                return def.Icon;
            }
            return string.Empty;
        }
    }
}