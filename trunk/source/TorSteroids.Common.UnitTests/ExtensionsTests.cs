using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NUnit.Framework;
using TorSteroids.Common.Extensions;

namespace TorSteroids.Common.UnitTests
{
    /// <summary>
    /// Common Tests
    /// </summary>
    [TestFixture]
    public class ExtensionsTests: INotifyPropertyChanged
    {
        #region Test Setup / ctor

        public ExtensionsTests()
        {
            
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

		#region Async Tests

		[Test]
		public void AsncTimeoutExtensionTest()
		{
			var timeout = TimeSpan.FromSeconds(5);
			var taskExecTime = TimeSpan.FromSeconds(10);

			long measureTime = 0;
			PerformanceHelper.StartMeasure(ref measureTime);

			Assert.Catch<TimeoutException>(async ()=> await CreateDelayTask(taskExecTime).TimeoutAfter(timeout));
			var timeRequired = PerformanceHelper.StopMeasure(measureTime);
		}

		[Test]
		public void AsncNoTimeoutExtensionTest()
		{
			var timeout = TimeSpan.FromSeconds(10);
			var taskExecTime = TimeSpan.FromSeconds(2);

			long measureTime = 0;
			PerformanceHelper.StartMeasure(ref measureTime);
			var t = CreateDelayTask(taskExecTime);
			Assert.DoesNotThrow(async () => await t.TimeoutAfter(timeout));
			var timeRequired = PerformanceHelper.StopMeasure(measureTime);
		}

		[Test]
		public void AsncSimpleExtensionTest()
		{
			var taskExecTime = TimeSpan.FromSeconds(2);
			Assert.DoesNotThrow(() => CreateDelayTask(taskExecTime));
		}

		private Task<object> CreateDelayTask(TimeSpan runTime)
	    {
		    var task = Task.Factory.StartNew(()=> TestTaskMethod(runTime));
			return task;
	    }

	    private object TestTaskMethod(TimeSpan runTime)
	    {
		    Thread.Sleep(runTime);
		    return null;
	    }

		#endregion

		#region Other Tests

		[Test]
		public void DateTimeExtDateToInteger()
		{
			DateTime date = new DateTime(2005, 10, 30, 13, 24, 11, 123, DateTimeKind.Unspecified);
			Assert.AreEqual(20051030, date.DateToInteger());

			Assert.AreEqual(10101, DateTime.MinValue.DateToInteger());
			Assert.AreEqual(99991231, DateTime.MaxValue.DateToInteger());

		}

		[Test]
		public void DateTimeExtTimeToInteger()
		{
			DateTime date = new DateTime(2005, 10, 30, 9, 24, 7, 23, DateTimeKind.Utc);
			Assert.AreEqual(92407023, date.TimeToInteger());
			Assert.AreEqual(92407023, date.TimeToInteger(true));

		}

		[Test]
		public void DateTimeExtFromUnixTimeSeconds()
		{
			const long unixTimestamp = 1449137110;
			var converted = DateTimeExtensions.FromUnixTimeSeconds(unixTimestamp);
			DateTime expectedUtc = new DateTime(2015, 12, 3, 10, 5, 10, 0, DateTimeKind.Utc);

			Assert.AreEqual(expectedUtc, converted);
		}

		[Test]
		public void DateTimeExtToUnixTimeSeconds()
		{
			const long expected = 1449137110;
			DateTime toConvertUtc = new DateTime(2015, 12, 3, 10, 5, 10, 0, DateTimeKind.Utc);

			Assert.AreEqual(expected, toConvertUtc.ToUnixTimeSeconds());
		}


		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
	    public void ExpressionExtNotifyFailWithArgumentException()
	    {
		    ExpressionExtensions.Notify(null, null);
	    }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpressionExtGetPropertyNameFailWithArgumentException()
		{
			ExpressionExtensions.GetPropertyName(null);
		}

		[Test]
		public void ExpressionExtGetPropertyNameSimpleProperty()
		{
			Assert.AreEqual("SimpleTestProperty", ExpressionExtensions.GetPropertyName(()=>SimpleTestProperty));
		}

		[Test]
		public void FormatWithNewLines()
		{
			Assert.AreEqual("Test\r\nString", "Test{0}String".FormatWith(Environment.NewLine));
			// Attention! If a string is concatenated like the following, .FormatWith() does not work on the whole concatenated string!
			// These strings are single instances and .FormatWith() only works on the latest it is "attached":
			Assert.AreEqual("Bad news: the Google Reader itself is not anymore available and so{0}" +
					"we use now feedly.com.\r\n", "Bad news: the Google Reader itself is not anymore available and so{0}" +
					"we use now feedly.com.{0}".FormatWith(Environment.NewLine));
		}

		[Test]
		public void ExpressionExtTestPropertyRaiseChangeEvent()
		{
			string eventCalledWithPropertyName = null;
			PropertyChanged += (sender, args) => { eventCalledWithPropertyName = args.PropertyName; };

			TestPropertyRaiseChangeEvent = "a test";
			Assert.AreEqual("a test", TestPropertyRaiseChangeEvent);
			Assert.AreEqual("TestPropertyRaiseChangeEvent", eventCalledWithPropertyName);
		}

        #endregion

		#region Support classes / properties

	    public string SimpleTestProperty { get; set; }

	    private string _testPropertyValue;
		
		public string TestPropertyRaiseChangeEvent {
			get { return _testPropertyValue; }
			set
			{
				_testPropertyValue = value;
				PropertyChanged.Notify(()=>TestPropertyRaiseChangeEvent);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged(string propertyName)
	    {
		    PropertyChangedEventHandler handler = PropertyChanged;
		    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
	    } 
		
		#endregion
    }
}
