#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
 */
#endregion

using JetBrains.Annotations;
using TorSteroids.Storage.Data;
using TorSteroids.Storage.Fluent;


namespace System.Data
{
    public static class DbDataReaderExtensions
    {
        public static FluentIndexDataReader Use(this IDataReader reader, FieldIndexLookup lookup)
        {
            return new FluentIndexDataReader(reader, FieldIndexLookup.FromDataReader(reader));
        }
    }
}
