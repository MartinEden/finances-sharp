using System;
using System.Collections.Generic;

namespace FinancesSharp.Migrate
{
    public class Mapper
    {
        private Dictionary<Type, Dictionary<long, object>> mapping;

        public Mapper()
        {
            mapping = new Dictionary<Type, Dictionary<long, object>>();
        }

        public void RecordMapping<T>(long oldId, T newObject)
        {
            var type = typeof(T);
            Dictionary<long, object> typeMap;
            if (!mapping.TryGetValue(type, out typeMap))
            {
                mapping[type] = (typeMap = new Dictionary<long, object>());
            }
            if (typeMap.ContainsKey(oldId))
            {
                throw new Exception(string.Format("Id '{0}' for '{1}' already exists in map", oldId, type.Name));
            }
            typeMap[oldId] = newObject;
        }

        public T Map<T>(long oldId)
        {
            return (T)mapping[typeof(T)][oldId];
        }
        public T MapIfNotNull<T>(object oldId)
        {
            if (oldId == DBNull.Value)
            {
                return default(T);
            }
            return Map<T>((long)oldId);
        }
    }
}
