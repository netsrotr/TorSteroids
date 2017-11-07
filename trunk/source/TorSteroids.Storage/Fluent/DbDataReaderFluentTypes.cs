#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

using System.Data;
using JetBrains.Annotations;
using TorSteroids.Common.Fluent;
using TorSteroids.Storage.Data;

namespace TorSteroids.Storage.Fluent
{
    public class FluentIndexDataReader : FluentContext<IDataReader>
    {
        public FluentIndexDataReader(IDataReader reader, FieldIndexLookup index) :
            base(reader)
        {
            Index = index;
        }

        public T GetSafe<T>([NotNull] string fieldName, T defaultValueIfNull = default(T)) where T : struct
        {
            return Db.ParseValue(Context.GetValue(Index.Get(fieldName)), defaultValueIfNull);
        }

        public object GetValue([NotNull] string fieldName)
        {
			return Context.GetValue(Index.Get(fieldName));
        }

        protected FieldIndexLookup Index
        {
            get;
            private set;
        }
    }

}
