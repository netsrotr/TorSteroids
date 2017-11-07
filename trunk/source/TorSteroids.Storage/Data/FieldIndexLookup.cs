#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace TorSteroids.Storage.Data
{
	[Serializable]
    public sealed class FieldIndexLookup : Dictionary<string, int>
    {
        public static FieldIndexLookup FromDataReader([NotNull]IDataReader reader)
        {
            reader.ExceptionIfNull("reader");
            
            var fieldCount = reader.FieldCount;
            var lookup = new FieldIndexLookup(fieldCount);
            
            for (int i = fieldCount - 1; 0 <= i; i--)
            {
                var str = reader.GetName(i);
                var ord = reader.GetOrdinal(str);
                lookup.Add(str, ord);
            }

            return lookup;
        }

        public FieldIndexLookup():
            base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public FieldIndexLookup(int capacity) :
            this(capacity, StringComparer.OrdinalIgnoreCase)
        {
        }

        
        public FieldIndexLookup(int capacity, IEqualityComparer<string> comparer):
            base(capacity, comparer)
        {
        }

		internal FieldIndexLookup(SerializationInfo info, StreamingContext context):
			base(info, context)
		{
			
		}

        public int Get([NotNull]string fieldName)
        {
            // resharper should warn, but no real param check for performance here...
            int index;
            if (TryGetValue(fieldName, out index))
                return index;
            
            return -1;
        }
    }
}
