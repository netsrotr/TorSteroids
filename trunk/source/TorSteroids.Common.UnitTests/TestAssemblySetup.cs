#region Version Info Header
/*
 * $Id: TestAssemblySetup.cs 85556 2013-03-13 13:33:33Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common.UnitTests/TestAssemblySetup.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-03-13 14:33:33 +0100 (Mi, 13 Mrz 2013) $
 * $Revision: 85556 $
 */
#endregion


using System.Diagnostics.Contracts;
using NUnit.Framework;

/// <summary>
/// See http://www.nunit.org/index.php?p=setupFixture&r=2.5.10
/// </summary>
[SetUpFixture]
// ReSharper disable CheckNamespace
public class TestAssemblySetup
// ReSharper restore CheckNamespace
{
    public TestAssemblySetup(){}

    [SetUp]
    public static void AssemblyInitialize()
    {
        // setup contract failures to be test failures: 
        Contract.ContractFailed += (sender, e) =>
        {
            e.SetUnwind(); // cause code to abort after event
            Assert.Fail(e.FailureKind.ToString() + ":" + e.Message);
        };
    }

    [TearDown]
    public static void AssemblyTearDown()
    {
            
    }

}
