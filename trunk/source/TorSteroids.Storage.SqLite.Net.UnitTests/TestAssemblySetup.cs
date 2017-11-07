#region Version Info Header
/*
 * $Id$
 * $HeadURL$
 * Last modified by $Author$
 * Last modified at $Date$
 * $Revision$
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
