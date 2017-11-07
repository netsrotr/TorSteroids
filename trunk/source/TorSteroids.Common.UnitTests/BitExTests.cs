using System;
using NUnit.Framework;

namespace TorSteroids.Common.UnitTests
{
    [Flags]
    enum ExX
    {
        None = 0, A = 1, B = 2, C = 4, D = 8, All = A | B | C | D
    }

    [TestFixture]
    public class BitExTests
    {

        [Test]
        public void TestSetFlagWithEnum()
        {
            ExX x = ExX.A | ExX.D;
            ExX newX = x.SetFlag(ExX.B);
            Assert.IsTrue((newX & ExX.A) == ExX.A, "A not set in ExX");
            Assert.IsTrue((newX & ExX.B) == ExX.B, "B not set in ExX");
            Assert.IsFalse((newX & ExX.C) == ExX.C, "C set in ExX is unexpected");
            Assert.IsTrue((newX & ExX.D) == ExX.D, "D not set in ExX");

            newX = x.SetFlag(ExX.None);
            Assert.IsTrue(newX == x);

            x = ExX.None;
            newX = x.SetFlag(ExX.B);
            Assert.IsFalse((newX & ExX.A) == ExX.A, "A set in ExX is unexpected");
            Assert.IsTrue((newX & ExX.B) == ExX.B, "B not set in ExX");
            Assert.IsFalse((newX & ExX.C) == ExX.C, "C set in ExX is unexpected");
            Assert.IsFalse((newX & ExX.D) == ExX.D, "D set in ExX is unexpected");

        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestSetFlagWithEnumExpectException()
        {
            ExX x = ExX.A | ExX.D;
            x.SetFlag(X.B);
        }

        [Test]
        public void TestHasFlagWithEnum()
        {
            ExX x = ExX.A | ExX.D;
            Assert.IsTrue(x.HasFlag(ExX.A), "Expected: true, because A is set in x");
            Assert.IsFalse(x.HasFlag(ExX.B), "Expected: false, because B is not set in x");
            Assert.IsTrue(x.HasFlag(ExX.D), "Expected: true, because D is set in x");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestHasFlagWithEnumExpectException()
        {
            ExX x = ExX.A | ExX.D;
            x.HasFlag(X.B);
        }

      

        [Test]
        public void TestClearFlagWithEnum()
        {
            ExX x = ExX.All.ClearFlag(ExX.C);
            Assert.IsTrue((x & ExX.A) == ExX.A, "Expected: true, because A is set in x");
            Assert.IsFalse((x & ExX.C) == ExX.C, "Expected: false, because C should be cleared from x");
            Assert.IsTrue((x & ExX.D) == ExX.D, "Expected: true, because D is set in x");

            x = ExX.All.ClearFlag(ExX.None);
            Assert.IsTrue((x & ExX.A) == ExX.A, "Expected: true, because A is set in x");
            Assert.IsTrue((x & ExX.B) == ExX.B, "Expected: true, because B is set in x");
            Assert.IsTrue((x & ExX.C) == ExX.C, "Expected: true, because C is set in x");
            Assert.IsTrue((x & ExX.D) == ExX.D, "Expected: true, because D is set in x");

            x = ExX.None.ClearFlag(ExX.A);
            Assert.IsFalse((x & ExX.A) == ExX.A, "Expected: false, because x was None");
            Assert.IsFalse((x & ExX.B) == ExX.B, "Expected: false, because x was None");
            Assert.IsFalse((x & ExX.C) == ExX.C, "Expected: false, because x was None");
            Assert.IsFalse((x & ExX.D) == ExX.D, "Expected: false, because x was None");
            
        }

        [Test]
        public void TestClearFlagWithParamsEnum()
        {
            ExX x = ExX.All.ClearFlag(ExX.B, ExX.C);
            Assert.IsTrue((x & ExX.A) == ExX.A, "Expected: true, because A is set in x");
            Assert.IsFalse((x & ExX.B) == ExX.B, "Expected: false, because B should be cleared from x");
            Assert.IsFalse((x & ExX.C) == ExX.C, "Expected: false, because C should be cleared from x");
            Assert.IsTrue((x & ExX.D) == ExX.D, "Expected: true, because D is set in x");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestClearFlagWithEnumExpectException()
        {
            ExX x = ExX.A | ExX.D;
            x.ClearFlag(X.B);
        }

        [Test]
        public void TestToggleFlagWithEnum()
        {
            ExX realX = ExX.A | ExX.C | ExX.D;
            ExX x = realX.ToggleFlag(false, ExX.C);
            Assert.IsTrue((x & ExX.A) == ExX.A, "Expected: true, because A is set in x");
            Assert.IsFalse((x & ExX.C) == ExX.C, "Expected: false, because C should be cleared from x");
            Assert.IsTrue((x & ExX.D) == ExX.D, "Expected: true, because D is set in x");
            Assert.IsFalse((x & ExX.B) == ExX.B, "Expected: false, because B was not set at x");
            x = realX.ToggleFlag(true, ExX.B);
            Assert.IsTrue((x & ExX.B) == ExX.B, "Expected: true, because B was now set at x");
            
            x = realX.ToggleFlag(true, ExX.None);
            Assert.IsTrue(x == realX);
            x = realX.ToggleFlag(false, ExX.None);
            Assert.IsTrue(x == realX);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestToggleFlagWithEnumExpectException()
        {
            ExX realX = ExX.A | ExX.C | ExX.D;
            realX.ToggleFlag(false, X.C);
        }
    }
}
