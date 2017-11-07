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
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SqlClient;
using JetBrains.Annotations;
using NUnit.Framework;

namespace TorSteroids.Storage.UnitTests
{
    /// <summary>
    /// Engine Tests
    /// </summary>
    [TestFixture]
    public class FluentDbParameterTests
    {
        #region Test Setup / ctor

        public FluentDbParameterTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Runs once for the whole Test Fixture (class)
        ///</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            // we can use TestContext.CurrentContext.TestDirectory
            // or / and TestContext.CurrentContext.WorkDirectory to handle local files/resources here per test fixture...
            // see http://nunit.org/index.php?p=testContext&r=2.6.2

            //Console.WriteLine("TestContext.CurrentContext.TestDirectory: " + TestContext.CurrentContext.TestDirectory);
            //Console.WriteLine("TestContext.CurrentContext.WorkDirectory: " + TestContext.CurrentContext.WorkDirectory);
        }

        /// <summary>
        /// Runs before every Test (method)
        ///</summary>
        [SetUp]
        public void TestSetup()
        {

        }

        #endregion

        #region Tests

        [Test]
        public void AsStringExtensionWithSqLiteFactoryDbParameter()
        {
            var par = SQLiteFactory.Instance.CreateParameter();
            AsStringExtensionWithDbParameterType(par);
        }

        private void AsStringExtensionWithDbParameterType([NotNull]DbParameter providerSpecific)
        {
            var par = providerSpecific;
            var contiunationPar = par.AsString("name");

            // check initial settings
            Assert.IsNotNull(contiunationPar);
            Assert.AreEqual("name", par.ParameterName);
            Assert.AreEqual("name", par.SourceColumn);
            Assert.AreEqual(ParameterDirection.Input, par.Direction);
            Assert.AreEqual(-1, par.Size);
            Assert.AreEqual(DbType.String, par.DbType);
            Assert.AreEqual(DBNull.Value, par.Value);

            // now change:
            contiunationPar.SourceColumn("columnName");
            Assert.AreEqual("name", par.ParameterName);
            Assert.AreEqual("columnName", par.SourceColumn);

            // now change:
            contiunationPar.Size(8);
            Assert.AreEqual(8, par.Size);

            // now change/test values:
            contiunationPar.SetValue("");
            Assert.AreEqual("", par.Value);
            Assert.AreEqual(DbType.String, par.DbType);

            contiunationPar.SetNull();
            Assert.AreEqual(DBNull.Value, par.Value);
            Assert.AreEqual(DbType.String, par.DbType);

            contiunationPar.SetValue("öäü");
            Assert.AreEqual("öäü", par.Value);
            Assert.AreEqual(DbType.String, par.DbType);

            contiunationPar.SetValue("1");
            Assert.AreEqual("1", par.Value);
            Assert.AreEqual(DbType.String, par.DbType);

            contiunationPar.SetValue(null);
            Assert.AreEqual(DBNull.Value, par.Value);
            Assert.AreEqual(DbType.String, par.DbType);

            var lastParamInstance = contiunationPar.SetValue("test");
            Assert.AreEqual("test", par.Value);
            Assert.AreEqual(DbType.String, par.DbType);

            Assert.IsTrue(ReferenceEquals(lastParamInstance, par));
        }

        [Test]
        public void AsInt32ExtensionWithSQLiteFactoryDbParameter()
        {
            var par = SQLiteFactory.Instance.CreateParameter();
            AsInt32ExtensionWithDbParameterType(par);
        }

        void AsInt32ExtensionWithDbParameterType([NotNull]DbParameter providerSpecific)
        {
            var par = providerSpecific;
            var contiunationPar = par.AsInt32("name");

            // check initial settings
            Assert.IsNotNull(contiunationPar);
            Assert.AreEqual("name", par.ParameterName);
            Assert.AreEqual("name", par.SourceColumn);
            Assert.AreEqual(ParameterDirection.Input, par.Direction);
            Assert.AreEqual(0, par.Size);
            Assert.AreEqual(DbType.Int32, par.DbType);
            Assert.AreEqual(DBNull.Value, par.Value);

            // now change:
            contiunationPar.SourceColumn("columnName");
            Assert.AreEqual("name", par.ParameterName);
            Assert.AreEqual("columnName", par.SourceColumn);

            // now change:
            contiunationPar.Size(8);
            Assert.AreEqual(8, par.Size);

            // now change/test values:
            contiunationPar.SetValue(1);
            Assert.AreEqual(1, par.Value);
            Assert.AreEqual(DbType.Int32, par.DbType);

            contiunationPar.SetNull();
            Assert.AreEqual(DBNull.Value, par.Value);
            Assert.AreEqual(DbType.Int32, par.DbType);

            contiunationPar.SetValue(0);
            Assert.AreEqual(0, par.Value);
            Assert.AreEqual(DbType.Int32, par.DbType);

            contiunationPar.SetValue(-1);
            Assert.AreEqual(-1, par.Value);
            Assert.AreEqual(DbType.Int32, par.DbType);

            contiunationPar.SetValue(Int32.MaxValue);
            Assert.AreEqual(Int32.MaxValue, par.Value);
            Assert.AreEqual(DbType.Int32, par.DbType);

            var lastParamInstance = contiunationPar.SetValue(Int32.MinValue);
            Assert.AreEqual(Int32.MinValue, par.Value);
            Assert.AreEqual(DbType.Int32, par.DbType);

            Assert.IsTrue(ReferenceEquals(lastParamInstance, par));
        }
        
        [Test]
        public void AsEnumExtensionWithSqlClientFactoryDbParameter()
        {
            var par = SqlClientFactory.Instance.CreateParameter();
            AsEnumExtensionWithParameterType(par);
        }

        [Test]
        public void AsEnumExtensionWithSQLiteFactoryDbParameter()
        {
            var par = SQLiteFactory.Instance.CreateParameter();
            AsEnumExtensionWithParameterType(par);
        }

        private void AsEnumExtensionWithParameterType([NotNull]DbParameter providerSpecific)
        {
            // use this particular parameter type:
            var par = providerSpecific;

            var contiunationPar = par.AsEnum("name");
            // check initial settings
            Assert.IsNotNull(contiunationPar);
            Assert.AreEqual("name", par.ParameterName);
            Assert.AreEqual("name", par.SourceColumn);
            Assert.AreEqual(ParameterDirection.Input, par.Direction);
            Assert.AreEqual(0, par.Size);
            Assert.AreEqual(DbType.Int32, par.DbType);
            Assert.AreEqual(DBNull.Value, par.Value);


            // now change:
            contiunationPar.SourceColumn("columnName");
            Assert.AreEqual("name", par.ParameterName);
            Assert.AreEqual("columnName", par.SourceColumn);

            // now change:
            contiunationPar.Size(8);
            Assert.AreEqual(8, par.Size);

            // now change/test values:
            contiunationPar.SetValue(TestEnumByte.E2);
            Assert.AreEqual(byte.MaxValue, par.Value);
            Assert.AreEqual(DbType.Byte, par.DbType);

            contiunationPar.SetValue(TestEnumSByte.E2);
            Assert.AreEqual(sbyte.MaxValue, par.Value);
            
            // MS SQL Client fails on SByte DbType, so we map to the next higher size:
            if (par is SqlParameter)
                Assert.AreEqual(DbType.Int16, par.DbType);
            else
                Assert.AreEqual(DbType.SByte, par.DbType);

            contiunationPar.SetValue(TestEnumInt16.E2);
            Assert.AreEqual(short.MaxValue, par.Value);
            Assert.AreEqual(DbType.Int16, par.DbType);

            contiunationPar.SetValue(TestEnumUInt16.E2);
            Assert.AreEqual(ushort.MaxValue, par.Value);
            // MS SQL Client fails on UInt16 DbType, so we map to the next higher size:
            if (par is SqlParameter)
                Assert.AreEqual(DbType.Int32, par.DbType);
            else
                Assert.AreEqual(DbType.UInt16, par.DbType);

            // does also change the recent DbType from above (initial one, or from last value assignment!)
            contiunationPar.SetNull();
            Assert.AreEqual(DBNull.Value, par.Value);
            Assert.AreNotEqual(DbType.UInt16, par.DbType);  // !!!
            Assert.AreEqual(DbType.Int32, par.DbType);  // !!!

            contiunationPar.SetValue(TestStandardEnum.E2);
            Assert.AreEqual(Int32.MaxValue, par.Value);
            Assert.AreEqual(DbType.Int32, par.DbType);

            contiunationPar.SetValue(TestEnumUInt32.E2);
            Assert.AreEqual(UInt32.MaxValue, par.Value);
            // MS SQL Client fails on UInt32 DbType, so we map to the next higher size:
            if (par is SqlParameter)
                Assert.AreEqual(DbType.Int64, par.DbType);
            else
                Assert.AreEqual(DbType.UInt32, par.DbType);

            contiunationPar.SetValue(TestEnumInt64.E2);
            Assert.AreEqual(long.MaxValue, par.Value);
            Assert.AreEqual(DbType.Int64, par.DbType);

            var lastParamInstance = contiunationPar.SetValue(TestEnumUInt64.E2);
            Assert.AreEqual(ulong.MaxValue, par.Value);
            // MS SQL Client fails on UInt64 DbType, so we map to the possibly way too small size (?):
            if (par is SqlParameter)
                Assert.AreEqual(DbType.Int64, par.DbType);
            else
                Assert.AreEqual(DbType.UInt64, par.DbType);

            Assert.IsTrue(ReferenceEquals(lastParamInstance, par));
        }
        
        #endregion

        #region Test data

        internal enum TestEnumByte : byte
        {
            E1 = (byte)1,
            E2 = byte.MaxValue
        }

        internal enum TestEnumSByte : sbyte
        {
            E1 = (sbyte)1,
            E2 = sbyte.MaxValue
        }

        internal enum TestEnumInt16 : short
        {
            E1 = (short)1,
            E2 = short.MaxValue
        }

        internal enum TestEnumUInt16 : ushort
        {
            E1 = (ushort)1,
            E2 = ushort.MaxValue
        }

        internal enum TestStandardEnum
        {
            E1 = 1,
            E2 = Int32.MaxValue
        }

        internal enum TestEnumUInt32 : uint
        {
            E1 = (uint)1,
            E2 = uint.MaxValue
        }

        internal enum TestEnumInt64 : long
        {
            E1 = (long)1,
            E2 = long.MaxValue
        }

        internal enum TestEnumUInt64 : ulong
        {
            E1 = (ulong)1,
            E2 = ulong.MaxValue
        }

        #endregion

    }
}
