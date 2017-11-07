using System;
using NUnit.Framework;

namespace TorSteroids.Common.UnitTests
{

	[Flags]
	enum X
	{
		None = 0, A = 1, B = 2, C = 4, D = 8
	}

	[TestFixture]
	public class BitTests
	{
        [Test]
        public void TestBitRotateRightAndLeftOnUInt32()
        {
            UInt32 val = 1497;

            UInt32 resR = val.R(7); // rotate right
            UInt32 resL = resR.L(7); // rotate left
            Assert.AreEqual(val, resL);

            resR = val.R(129);
            resL = resR.L(129);
            Assert.AreEqual(val, resL);

            val = 300666;
            resL = val.L(11);
            resR = resL.R(11);
            Assert.AreEqual(val, resR);
        }

        [Test]
        public void TestBitRotateRightAndLeftOnUInt16()
        {
            UInt16 val = 1497;

            UInt16 resR = val.R(7); // rotate right
            UInt16 resL = resR.L(7); // rotate left
            Assert.AreEqual(val, resL);

            resR = val.R(129);
            resL = resR.L(129);
            Assert.AreEqual(val, resL);

            val = 30066;
            resL = val.L(11);
            resR = resL.R(11);
            Assert.AreEqual(val, resR);
        }

        [Test]
        public void TestBitRotateRightAndLeftOnByte()
        {
            Byte val = 197;

            Byte resR = val.R(7); // rotate right
            Byte resL = resR.L(7); // rotate left
            Assert.AreEqual(val, resL);

            resR = val.R(129);
            resL = resR.L(129);
            Assert.AreEqual(val, resL);

            val = 66;
            resL = val.L(11);
            resR = resL.R(11);
            Assert.AreEqual(val, resR);
        }

        [Test]
        public void TestBitRotateRightAndLeftWithExactByteSize()
        {
            Byte val = 197;

            Byte resR = val.R(8); // rotate right by 8, on a byte should not modify the content:
            Assert.AreEqual(val, resR);

            Byte resL = resR.L(8); // rotate left by 8, on a byte should not modify the content:
            Assert.AreEqual(val, resL);
        }

        [Test]
        public void TestBitRotateRightAndLeftZeroByte()
        {
            Byte val = 197;

            Byte resR = val.R(0); // rotate right by 0, on a byte should not modify the content:
            Assert.AreEqual(val, resR);

            Byte resL = resR.L(0); // rotate left by 0, on a byte should not modify the content:
            Assert.AreEqual(val, resL);
        }


        [Test]
        public void TestBitRotateRightAndLeftWithExactUInt16Size()
        {
            UInt16 val = 197;

            UInt16 resR = val.R(16); // rotate right by 16, on a byte should not modify the content:
            Assert.AreEqual(val, resR);

            UInt16 resL = resR.L(16); // rotate left by 16, on a byte should not modify the content:
            Assert.AreEqual(val, resL);
        }

        [Test]
        public void TestBitRotateRightAndLeftZeroUInt16()
        {
            UInt16 val = 197;

            UInt16 resR = val.R(0); // rotate right by 0, on a byte should not modify the content:
            Assert.AreEqual(val, resR);

            UInt16 resL = resR.L(0); // rotate left by 0, on a byte should not modify the content:
            Assert.AreEqual(val, resL);
        }

        [Test]
        public void TestBitRotateRightAndLeftWithExactUInt32Size()
        {
            UInt32 val = 197;

            UInt32 resR = val.R(32); // rotate right by 32, on a byte should not modify the content:
            Assert.AreEqual(val, resR);

            UInt32 resL = resR.L(32); // rotate left by 32, on a byte should not modify the content:
            Assert.AreEqual(val, resL);
        }

        [Test]
        public void TestBitRotateRightAndLeftZeroUInt32()
        {
            UInt32 val = 197;

            UInt32 resR = val.R(0); // rotate right by 0, on a byte should not modify the content:
            Assert.AreEqual(val, resR);

            UInt32 resL = resR.L(0); // rotate left by 0, on a byte should not modify the content:
            Assert.AreEqual(val, resL);
        }


		[Test]
		public void TestSetWithEnum() {
			X x = X.A | X.D;
			X newX = (X)Bit.Set(x, X.B);
			Assert.IsTrue((newX & X.A) == X.A, "A not set in X");
			Assert.IsTrue((newX & X.B) == X.B, "B not set in X");
			Assert.IsTrue((newX & X.D) == X.D, "D not set in X");
		}
		[Test]
		public void TestIsSetWithEnum()
		{
			X x = X.A | X.D;
			Assert.IsTrue(Bit.IsSet(x, X.A), "Expected: true, because A is set in x");
			Assert.IsFalse(Bit.IsSet(x, X.B), "Expected: false, because B is not set in x");
			Assert.IsTrue(Bit.IsSet(x, X.D), "Expected: true, because D is set in x");
		}
		[Test]
		public void TestUnSetWithEnum()
		{
			X realX = X.A | X.C | X.D;
			X x = (X)Bit.UnSet(realX, X.C);
			Assert.IsTrue((x & X.A) == X.A, "Expected: true, because A is set in x");
			Assert.IsFalse((x & X.C) == X.C, "Expected: false, because C should be cleared from x");
			Assert.IsTrue((x & X.D) == X.D, "Expected: true, because D is set in x");
		}
		[Test]
		public void TestToggleWithEnum()
		{
			X realX = X.A | X.C | X.D;
			X x = (X)Bit.Toggle(false, realX, X.C);
			Assert.IsTrue((x & X.A) == X.A, "Expected: true, because A is set in x");
			Assert.IsFalse((x & X.C) == X.C, "Expected: false, because C should be cleared from x");
			Assert.IsTrue((x & X.D) == X.D, "Expected: true, because D is set in x");
			Assert.IsFalse((x & X.B) == X.B, "Expected: false, because B was not set at x");
			x = (X)Bit.Toggle(true, realX, X.B);
			Assert.IsTrue((x & X.B) == X.B, "Expected: true, because B was now set at x");
		}

		private const int A = 0x1;
		private const int B = 0x2;
		private const int C = 0x4;
		private const int D = 0x8;

		[Test]
		public void TestSetWithInt()
		{
			int x = A | D;
			int newX = Bit.Set(x, B);
			Assert.IsTrue((newX & A) == A, "A not set in newX");
			Assert.IsTrue((newX & B) == B, "B not set in newX");
			Assert.IsTrue((newX & D) == D, "D not set in newX");
		}
		[Test]
		public void TestIsSetWithInt()
		{
			int x = A | D;
			Assert.IsTrue(Bit.IsSet(x, A), "Expected: true, because A is set in x");
			Assert.IsFalse(Bit.IsSet(x, B), "Expected: false, because B is not set in x");
			Assert.IsTrue(Bit.IsSet(x, D), "Expected: true, because D is set in x");
		}
		[Test]
		public void TestUnSetWithInt()
		{
			int realX = A | C | D;
			int x = Bit.UnSet(realX, C);
			Assert.IsTrue((x & A) == A, "Expected: true, because A is set in x");
			Assert.IsFalse((x & C) == C, "Expected: false, because C should be cleared from x");
			Assert.IsTrue((x & D) == D, "Expected: true, because D is set in x");
		}
		[Test]
		public void TestToggleWithInt()
		{
			int realX = A | C | D;
			int x = Bit.Toggle(false, realX, C);
			Assert.IsTrue((x & A) == A, "Expected: true, because A is set in x");
			Assert.IsFalse((x & C) == C, "Expected: false, because C should be cleared from x");
			Assert.IsTrue((x & D) == D, "Expected: true, because D is set in x");
			Assert.IsFalse((x & B) == B, "Expected: false, because B was not set at x");
			x = Bit.Toggle(true, realX, B);
			Assert.IsTrue((x & B) == B, "Expected: true, because B was now set at x");
		}

		private const long AA = 0x10;
		private const long BB = 0x20;
		private const long CC = 0x40;
		private const long DD = 0x80;

		[Test]
		public void TestSetWithLong()
		{
			long x = AA | DD;
			long newX = Bit.Set(x, BB);
			Assert.IsTrue((newX & AA) == AA, "AA not set in newX");
			Assert.IsTrue((newX & BB) == BB, "BB not set in newX");
			Assert.IsTrue((newX & DD) == DD, "DD not set in newX");
		}
		[Test]
		public void TestIsSetWithLong()
		{
			long x = AA | DD;
			Assert.IsTrue(Bit.IsSet(x, AA), "Expected: true, because AA is set in x");
			Assert.IsFalse(Bit.IsSet(x, BB), "Expected: false, because BB is not set in x");
			Assert.IsTrue(Bit.IsSet(x, DD), "Expected: true, because DD is set in x");
		}
		[Test]
		public void TestUnSetWithLong()
		{
			long realX = AA | CC | DD;
			long x = Bit.UnSet(realX, CC);
			Assert.IsTrue((x & AA) == AA, "Expected: true, because AA is set in x");
			Assert.IsFalse((x & CC) == CC, "Expected: false, because CC should be cleared from x");
			Assert.IsTrue((x & DD) == DD, "Expected: true, because D is set in x");

		}
		[Test]
		public void TestToggleWithLong()
		{
			long realX = AA | CC | DD;
			long x = Bit.Toggle(false, realX, CC);
			Assert.IsTrue((x & AA) == AA, "Expected: true, because AA is set in x");
			Assert.IsFalse((x & CC) == CC, "Expected: false, because CC should be cleared from x");
			Assert.IsTrue((x & DD) == DD, "Expected: true, because DD is set in x");
			Assert.IsFalse((x & BB) == BB, "Expected: false, because BB was not set at x");
			x = Bit.Toggle(true, realX, BB);
			Assert.IsTrue((x & BB) == BB, "Expected: true, because BB was now set at x");
		}
	}
}
