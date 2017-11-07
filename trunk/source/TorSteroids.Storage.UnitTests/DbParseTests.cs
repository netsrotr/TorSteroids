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
using NUnit.Framework;
using TorSteroids.Storage.Data;

namespace TorSteroids.Storage.UnitTests
{
	/// <summary>
	/// DbParse Tests
	/// </summary>
	[TestFixture]
	public class DbParseTests
	{
		[Test]
		public void TimeStampTests()
		{
			var ts = DbTimeStamp.Now;
			Console.WriteLine("Timestamp: {0}", ts);

			var reparsed = new DbTimeStamp(ts.ToString());

			Assert.AreEqual(ts.ToString(), reparsed.ToString());

		}

		[Test]
		public void ParseStrings()
		{
			Assert.IsNull(Db.ParseValue(DBNull.Value, null), "Expected null (default value)");
			Assert.IsNull(Db.ParseValue(null, null), "Expected null (default value)");
			Assert.IsNotNull(Db.ParseValue("", null), "Expected not null (empty string value)");
			Assert.IsEmpty(Db.ParseValue("", null), "Expected not null (empty string value)");
			Assert.IsNotNull(Db.ParseValue("test", null), "Expected not null (test string value)");
			Assert.IsNotEmpty(Db.ParseValue("test", null), "Expected not null (test string value)");
			Assert.AreEqual("test", Db.ParseValue("test", null));
		}

		[Test]
		public void ParseGenericLong()
		{
			long v = Db.ParseValue(DBNull.Value, -1L);
			Assert.AreEqual(-1L, v, "Expected -1L (default value)");
			Assert.AreEqual(-1L, Db.ParseValue(null, -1L), "Expected -1L (default value)");
			Assert.AreEqual(0L, Db.ParseValue(0, -1L), "Expected 0L (parsed value)");
			Assert.AreEqual(20L, Db.ParseValue(20, -1L), "Expected 20L (parsed value)");
		}

        [Test]
        public void ParseGenericULong()
        {
            ulong v = Db.ParseValue(DBNull.Value, ulong.MaxValue);
            Assert.AreEqual(ulong.MaxValue, v, "Expected MaxValue (default value)");
            Assert.AreEqual(ulong.MinValue, Db.ParseValue(null, ulong.MinValue), "Expected MinValue (default value)");
            Assert.AreEqual(0, Db.ParseValue(0, (ulong)2), "Expected 0 (parsed value)");
            Assert.AreEqual(20, Db.ParseValue(20, (ulong)20), "Expected 20L (parsed value)");
        }

        enum TestEnum
        {
            None = 0,
            A1,
            A2
        }

		[Test]
		public void ParseGenericEnum()
		{
			TestEnum v = Db.ParseValue(DBNull.Value, TestEnum.A1);
            Assert.AreEqual(TestEnum.A1, v, "Expected TestEnum.A1 (default value)");
            Assert.AreEqual(TestEnum.A2, Db.ParseValue(null, TestEnum.A2), "Expected A2 (default value)");
            Assert.AreEqual(TestEnum.None, Db.ParseValue(0, TestEnum.A1), "Expected None (parsed value)");
			Assert.AreEqual(TestEnum.A2, Db.ParseValue(2, TestEnum.None), "Expected A2 (parsed value)");
		}

        [Test]
        public void ParseGenericInt()
        {
            int v = Db.ParseValue(DBNull.Value, -1);
            Assert.AreEqual(-1, v, "Expected -1 (default value)");
            Assert.AreEqual(-1, Db.ParseValue(null, -1), "Expected -1 (default value)");
            Assert.AreEqual(0, Db.ParseValue(0, -1), "Expected 0 (parsed value)");
            Assert.AreEqual(20, Db.ParseValue(20, -1), "Expected 20 (parsed value)");
        }

		[Test]
		public void ParseGenericBool()
		{
			bool v = Db.ParseValue(DBNull.Value, false);
			Assert.AreEqual(false, v, "Expected false (default value)");
			Assert.AreEqual(true, Db.ParseValue(DBNull.Value, true), "Expected true (default value)");
			Assert.AreEqual(false, Db.ParseValue(null, false), "Expected false (default value)");
			Assert.AreEqual(false, Db.ParseValue(false, true), "Expected false (parsed value)");
			Assert.AreEqual(true, Db.ParseValue(true, false), "Expected true (parsed value)");
		}

		[Test]
		public void ParseGenericFloat()
		{
			float v = Db.ParseValue(DBNull.Value, -1f);
			Assert.AreEqual(-1f, v, "Expected -1f (default value)");
			Assert.AreEqual(-1f, Db.ParseValue(null, -1f), "Expected -1f (default value)");
			Assert.AreEqual(0f, Db.ParseValue(0, -1f), "Expected 0f (parsed value)");
			Assert.AreEqual(20f, Db.ParseValue(20, -1f), "Expected 20f (parsed value)");
		}

		[Test]
		public void ParseGenericDouble()
		{
			double v = Db.ParseValue(DBNull.Value, -1d);
			Assert.AreEqual(-1d, v, "Expected -1d (default value)");
			Assert.AreEqual(-1d, Db.ParseValue(null, -1d), "Expected -1d (default value)");
			Assert.AreEqual(0d, Db.ParseValue(0, -1d), "Expected 0d (parsed value)");
			Assert.AreEqual(20d, Db.ParseValue(20, -1d), "Expected 20d (parsed value)");
		}

		[Test]
		public void ParseGenericDecimal()
		{
			decimal dv = -1;
			decimal v = Db.ParseValue(DBNull.Value, dv);
			Assert.AreEqual(-1, v, "Expected -1dec (default value)");
			Assert.AreEqual(-1, Db.ParseValue(null, dv), "Expected -1dec (default value)");
			Assert.AreEqual(0, Db.ParseValue(0, dv), "Expected 0dec (parsed value)");
			Assert.AreEqual(20, Db.ParseValue(20, dv), "Expected 20dec (parsed value)");
		}

		[Test]
		public void ParseGenericDateTime()
		{
			DateTime dv = DateTime.MinValue;
			DateTime v = Db.ParseValue(DBNull.Value, dv);
			Assert.AreEqual(DateTime.MinValue, v, "Expected DateTime.MinValue (default value)");
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue(null, dv), "Expected DateTime.MinValue (default value)");

			// string input:
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue("", dv), "Expected DateTime.MinValue (default value)");
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue("0", dv), "Expected DateTime.MinValue (default value)");
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue("1966", dv));
			Assert.AreEqual(new DateTime(1966, 6, 30), Db.ParseValue("19660630", dv));

			// decimal input:
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue((decimal)0, dv), "Expected DateTime.MinValue (default value)");
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue((decimal)1966, dv));
			Assert.AreEqual(new DateTime(1966, 6, 30), Db.ParseValue((decimal)19660630, dv));

			// integer input:
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue((int)0, dv), "Expected DateTime.MinValue (default value)");
			Assert.AreEqual(DateTime.MinValue, Db.ParseValue((int)1966, dv));
			Assert.AreEqual(new DateTime(1966, 6, 30), Db.ParseValue((int)19660630, dv));
		}

		[Test,ExpectedException(typeof(DataException))]
		public void ParseGenericDateTimeDecimalNegativeNumberShouldFail()
		{
			Db.ParseValue((decimal) -1, DateTime.MinValue);
		}

		[Test, ExpectedException(typeof(DataException))]
		public void ParseGenericDateTimeIntegerNegativeNumberShouldFail()
		{
			Db.ParseValue((int)-1, DateTime.MinValue);
		}

		[Test, ExpectedException(typeof(DataException))]
		public void ParseGenericDateTimeStringNegativeNumberShouldFail()
		{
			Db.ParseValue("-1", DateTime.MinValue);
		}

		[Test]
		public void ParseNullableStringsDbNull()
		{
			string[] colNames = new string[] { "DBNULL_COL" };
			object[] testData = new object[] { DBNull.Value };
			string[] expected = new[] { (string)null };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, 0));
				Assert.AreEqual(expected[0], reader.GetSafe(0));
			}
		}

		[Test]
		public void ParseNullableStringsEmpty()
		{
			string[] colNames = new string[] { "DBNULL_COL" };
			object[] testData = new object[] { "" };
			string[] expected = new[] { "" };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "DBNULL_COL"));
				Assert.AreEqual(expected[0], reader.GetSafe("DBNULL_COL"));
			}
		}

		[Test]
		public void ParseNullableStringsValue()
		{
			string[] colNames = new string[] { "DBNULL_COL" };
			object[] testData = new object[] { "test ä" };
			string[] expected = new[] { "test ä" };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, 0));
				Assert.AreEqual(expected[0], reader.GetSafe(0));
			}
		}


		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseNullableStringsFailInvalidFieldName()
		{
			string[] colNames = new string[] { "DBNULL_COL" };
			object[] testData = new object[] { "" };
			string[] expected = new[] { "" };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "NON_EXISTING_COL"));
			}
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetSafeNullableStringsFailInvalidFieldName()
        {
            string[] colNames = new string[] { "DBNULL_COL" };
            object[] testData = new object[] { "" };
            string[] expected = new[] { "" };

            IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
            while (reader.Read())
            {
                Assert.AreEqual(expected[0], reader.GetSafe("NON_EXISTING_COL"));
            }
        }

		[Test, ExpectedException(typeof(IndexOutOfRangeException))]
		public void ParseInvalidIndex()
		{

			string[] colNames = new string[] { "DBNULL_COL" };
			object[] testData = new object[] { DBNull.Value };
			DateTime[] expected = new DateTime[] { DateTime.MinValue };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(Decimal), colNames, testData));
			// read one record with invalid index, expected IndexOutOfRangeException
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, 1, DateTime.MinValue));
			}
		}

        [Test, ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetSafeInvalidIndex()
        {

            string[] colNames = new string[] { "DBNULL_COL" };
            object[] testData = new object[] { DBNull.Value };
            DateTime[] expected = new DateTime[] { DateTime.MinValue };

            IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(Decimal), colNames, testData));
            // read one record with invalid index, expected IndexOutOfRangeException
            while (reader.Read())
            {
                Assert.AreEqual(expected[0], reader.GetSafe(1, DateTime.MinValue));
            }
        }

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ParseInvalidFieldName()
		{

			string[] colNames = new string[] { "DBNULL_COL" };
			object[] testData = new object[] { DBNull.Value };
			DateTime[] expected = new DateTime[] { DateTime.MinValue };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(Decimal), colNames, testData));
			// read one record with invalid field name, expected ArgumentOutOfRangeException
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "COLUMN_1", DateTime.MinValue));
			}
		}

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetSafeInvalidFieldName()
        {

            string[] colNames = new string[] { "DBNULL_COL" };
            object[] testData = new object[] { DBNull.Value };
            DateTime[] expected = new DateTime[] { DateTime.MinValue };

            IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(Decimal), colNames, testData));
			// read one record with invalid field name, expected ArgumentOutOfRangeException
            while (reader.Read())
            {
                Assert.AreEqual(expected[0], reader.GetSafe("COLUMN_1", DateTime.MinValue));
            }
        }

		//test ArgumentNullException
		[Test]
		public void ParseInvalidReaders()
		{
			try { Db.Parse(null, "COLUMN_X", "Empty"); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, "COLUMN_X", 0); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, "COLUMN_X", DateTime.Now); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, "COLUMN_X", true); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, "COLUMN_X", 0f); }
			catch (ArgumentNullException) { }
		}

		//test ArgumentNullException
		[Test]
		public void ParseInvalidColumnName()
		{
			try { Db.Parse(null, null, "Empty"); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, "", 0); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, null, DateTime.Now); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, String.Empty, true); }
			catch (ArgumentNullException) { }
			try { Db.Parse(null, "", 0f); }
			catch (ArgumentNullException) { }
		}

        [Test, ExpectedException(typeof(DataException))]
		public void ParseInvalidStringFieldContentToBool()
		{

			string[] colNames = new string[] { "INVALID_COL" };
			object[] testData = new object[] { "chees" };
			bool[] expected = new bool[] { true };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record with invalid content, expected DataException
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "INVALID_COL", false));
			}
		}

        [Test, ExpectedException(typeof(DataException))]
		public void ParseInvalidStringFieldContentToInt()
		{

			string[] colNames = new string[] { "INVALID_COL" };
			object[] testData = new object[] { "chees" };
			int[] expected = new int[] { 1 };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record with invalid content, expected DataException
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "INVALID_COL", 0));
			}
		}

        [Test, ExpectedException(typeof(DataException))]
		public void ParseInvalidStringFieldContentToFloat()
		{

			string[] colNames = new string[] { "INVALID_COL" };
			object[] testData = new object[] { "chees" };
			float[] expected = new float[] { 1 };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record with invalid content, expected DataException
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "INVALID_COL", 0f));
			}
		}

        [Test, ExpectedException(typeof(DataException))]
		public void ParseInvalidStringFieldContentToDouble()
		{

			string[] colNames = new string[] { "INVALID_COL" };
			object[] testData = new object[] { "chees" };
			double[] expected = new double[] { 1D };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record with invalid content, expected DataException
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "INVALID_COL", 0.0D));
			}
		}

		[Test, ExpectedException(typeof(DataException))]
		public void ParseInvalidStringFieldContentToDecimal()
		{

			string[] colNames = new string[] { "INVALID_COL" };
			object[] testData = new object[] { "chees" };
			decimal[] expected = new decimal[] { 1M };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record with invalid content, expected DataException
			while (reader.Read())
			{
				Assert.AreEqual(expected[0], Db.Parse(reader, "INVALID_COL", 0.0M));
			}
		}

		[Test]
		public void ParseIntegerOnDbFieldInteger()
		{
			const int valueIfNull = 1;

			var colNames = new string[] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "NEGATIVE_COL", "VAL_COL", "MIN_COL", "MAX_COL" };
			var testData = new object[] { DBNull.Value, null, 0, -1, 1966, Int32.MinValue, Int32.MaxValue };
			var expected = new int[]    { valueIfNull, valueIfNull, 0, -1, 1966, Int32.MinValue, Int32.MaxValue };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(int), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					Assert.AreEqual(expected[i], Db.Parse(reader, colNames[i], valueIfNull), "failed to parse '{0}'.", colNames[i]);
					Assert.AreEqual(expected[i], Db.Parse(reader, i, valueIfNull), "failed to parse column at index {0} ({1}).", i, colNames[i]);
				}
			}
		}

		[Test]
		public void ParseSByteOnDbFieldInteger()
		{
			const sbyte valueIfNull = 1;

			var colNames = new string[] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "NEGATIVE_COL", "VAL_COL", "MIN_COL", "MAX_COL" };
			var testData = new object[] { DBNull.Value, null, 0, -1, 126, SByte.MinValue, SByte.MaxValue };
			var expected = new sbyte[] { valueIfNull, valueIfNull, 0, -1, 126, SByte.MinValue, SByte.MaxValue };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(int), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					Assert.AreEqual(expected[i], Db.Parse(reader, colNames[i], valueIfNull), "failed to parse '{0}'.", colNames[i]);
					Assert.AreEqual(expected[i], Db.Parse(reader, i, valueIfNull), "failed to parse column at index {0} ({1}).", i, colNames[i]);
				}
			}
		}

		[Test]
		public void ParseDateTimeOnDbFieldInteger()
		{

			string[] colNames =
				new string[] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "NEGATIVE_COL", "PARTITIAL_COL", "FULLDATE_COL" };
			object[] testData =
				new object[] { DBNull.Value, null, 0, -1, 1966, 19660630 };
			DateTime[] expected =
				new DateTime[] { DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, new DateTime(1966, 6, 30) };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(int), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					if (i == 3)
					{
						try
						{
							Db.Parse(reader, colNames[i], DateTime.MinValue);
							Assert.Fail("DataException expected");
						}
						catch (DataException)
						{
							//OK
						}
						try
						{
							Db.Parse(reader, i, DateTime.MinValue);
							Assert.Fail("DataException expected");
						}
						catch (DataException)
						{
							//OK
						}

						continue;
					}

					Assert.AreEqual(expected[i], Db.Parse(reader, colNames[i], DateTime.MinValue), "failed to parse '{0}'.", colNames[i]);
					Assert.AreEqual(expected[i], Db.Parse(reader, i, DateTime.MinValue), "failed to parse column at index {0} ({1}).", i, colNames[i]);
				}
			}
		}

		[Test]
		public void ParseEnumOnDbFieldInteger()
		{
			string[] colNames =
				new [] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "VALUE_COL_1", "VALUE_COL_2", "NOTDEFINED_ENUM_VALUE_COL" };
			object[] testData =
				new object[] { DBNull.Value, null, 0, 1, 2, 777 };
			TestEnum[] expected =
				new [] { TestEnum.None, TestEnum.None, TestEnum.None, TestEnum.A1, TestEnum.A2 ,(TestEnum)777};

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(int), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					TestEnum parsed = Db.Parse(reader, colNames[i], TestEnum.None);
					Assert.AreEqual(expected[i], parsed, "failed to parse '{0}'.", colNames[i]);
					parsed = Db.Parse(reader, i, TestEnum.None);
					Assert.AreEqual(expected[i], parsed, "failed to parse column at index {0} ({1}).", i, colNames[i]);

					if (colNames[i] == "NOTDEFINED_ENUM_VALUE_COL")
					{
						Assert.IsFalse(Enum.IsDefined(typeof(TestEnum), parsed));
						Assert.AreEqual((TestEnum)777, parsed);
					}
				}
			}
		}

		[Test]
		public void ParseDateTimeOnDbFieldDecimal()
		{

			string[] colNames =
				new string[] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "NEGATIVE_COL", "PARTITIAL_COL", "FULLDATE_COL" };
			object[] testData =
				new object[] { DBNull.Value, null, 0.0, -1.0, 1966.0, 19660630.0 };
			DateTime[] expected =
				new DateTime[] { DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, new DateTime(1966, 6, 30) };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(Decimal), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					if (i == 3)
					{
						try
						{
							Db.Parse(reader, colNames[i], DateTime.MinValue);
							Assert.Fail("DataException expected");
						}
						catch (DataException)
						{
							//OK
						}
						try
						{
							Db.Parse(reader, i, DateTime.MinValue);
							Assert.Fail("DataException expected");
						}
						catch (DataException)
						{
							//OK
						}
						
						continue;
					}

					Assert.AreEqual(expected[i], Db.Parse(reader, colNames[i], DateTime.MinValue), "failed to parse '{0}'.", colNames[i]);
					Assert.AreEqual(expected[i], Db.Parse(reader, i, DateTime.MinValue), "failed to parse column at index {0} ({1}).", i, colNames[i]);
				}
			}
		}

		[Test]
		public void ParseDateTimeOnDbFieldString()
		{

			string[] colNames =
				new string[] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "NEGATIVE_COL", "PARTITIAL_COL", "FULLDATE_COL" };
			object[] testData =
				new object[] { DBNull.Value, null, "", "-1", "1966", "19660630" };
			DateTime[] expected =
				new DateTime[]{DateTime.MinValue, DateTime.MinValue, 
				               DateTime.MinValue, DateTime.MinValue, 
				               DateTime.MinValue, 
				               new DateTime(1966, 6, 30)};

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					if (i == 3)
					{
						try
						{
							Db.Parse(reader, colNames[i], DateTime.MinValue);
							Assert.Fail("DataException expected");
						}
						catch (DataException)
						{
							//OK
						}
						try
						{
							Db.Parse(reader, i, DateTime.MinValue);
							Assert.Fail("DataException expected");
						}
						catch (DataException)
						{
							//OK
						}

						continue;
					}
					Assert.AreEqual(expected[i], Db.Parse(reader, colNames[i], DateTime.MinValue), "failed to parse '{0}'.", colNames[i]);
					Assert.AreEqual(expected[i], Db.Parse(reader, i, DateTime.MinValue), "failed to parse column at index {0} ({1}).", i, colNames[i]);
				}
			}
		}

		[Test]
		public void ParseStringOnDbFieldString()
		{

			string[] colNames =
				new string[] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "SPECIALCHARS_COL", "SPECIALDBCHARS_COL", "QUOTECHARS_COL" };
			object[] testData =
				new object[] { DBNull.Value, null, "", "öüüÄÖÜß", @"%[]*_/\", "D'Art\"´`" };
			string[] expected =
				new string[] { String.Empty, String.Empty, String.Empty, "öüüÄÖÜß", @"%[]*_/\", "D'Art\"´`" };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					Assert.AreEqual(expected[i], Db.Parse(reader, colNames[i], String.Empty), "failed to parse '{0}'.", colNames[i]);
					Assert.AreEqual(expected[i], Db.Parse(reader, i, String.Empty), "failed to parse column at index {0} ({1}).", i, colNames[i]);
				}
			}
		}

		[Test]
		public void ParseBoolOnDbFieldString()
		{

			string[] colNames =
				new string[] { "DBNULL_COL", "NULL_COL", "ZERO_COL", "STRINGREP1_COL", "STRINGREP2_COL", "STRINGREP3_COL", "VALID_T_COL", "VALID_F_COL" };
			object[] testData =
				new object[] { DBNull.Value, null, "", "true", "TRue", "TRUE", "1", "0" };
			bool[] expected =
				new bool[] { false, false, false, true, true, true, true, false };

			IDataReader reader = new DataTableReader(CreateTestDataTable(typeof(string), colNames, testData));
			// read one record:
			while (reader.Read())
			{
				for (int i = 0; i < colNames.Length; i++)
				{
					Assert.AreEqual(expected[i], Db.Parse(reader, colNames[i], false), "failed to parse '{0}'.", colNames[i]);
					Assert.AreEqual(expected[i], Db.Parse(reader, i, false), "failed to parse column at index {0} ({1}).", i, colNames[i]);
				}
			}
		}

		/// <summary>
		/// Creates the test data table.
		/// </summary>
		/// <param name="dbFieldType">Type of the db field.</param>
		/// <param name="fieldNames">The field names.</param>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		DataTable CreateTestDataTable(Type dbFieldType, string[] fieldNames, object[] values)
		{
			DataTable testData = new DataTable("TestData");
			for (int i = 0; i < fieldNames.Length; i++)
				testData.Columns.Add(new DataColumn(fieldNames[i], dbFieldType));

			testData.Rows.Add(values);
			return testData;
		}
	}
}
