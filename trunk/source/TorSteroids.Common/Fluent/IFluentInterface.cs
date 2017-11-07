#region Version Info Header
/*
 * $Id: IFluentInterface.cs 85556 2013-03-13 13:33:33Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Fluent/IFluentInterface.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-03-13 14:33:33 +0100 (Mi, 13 Mrz 2013) $
 * $Revision: 85556 $
 */
#endregion

using System;
using System.ComponentModel;

namespace TorSteroids.Common.Fluent
{
    /// <summary>
    /// Interface that is used to build fluent interfaces and hides methods declared by <see cref="object"/> from IntelliSense.
    /// </summary>
    /// <remarks>Code that consumes implementations of this interface should expect one of two things:
    ///  <list type = "number">
    ///    <item>When referencing the interface from within the same solution (project reference), you will still see the methods this interface is meant to hide.</item>
    ///    <item>When referencing the interface through the compiled output assembly (external reference), the standard Object methods will be hidden as intended.</item>
    ///  </list>
    /// See http://bit.ly/ifluentinterface for more information.
    /// <para>
    /// For Resharper users: you just need to configure resharper to respect the <see cref="EditorBrowsableAttribute"/> attribute. 
    /// In Resharper Options, go to Environment | IntelliSense | Completion Appearance and check “Filter members by [EditorBrowsable] attribute.
    /// </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluentInterface
    {
        /// <summary>Redeclaration that hides the <see cref="object.GetType()"/> method from IntelliSense.</summary> 
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();
 
        /// <summary>Redeclaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();
        
        /// <summary> Redeclaration that hides the <see cref="object.ToString()"/> method from IntelliSense.</summary>    
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
        
        /// <summary>Redeclaration that hides the <see cref="object.Equals(object)"/> method from IntelliSense. </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);
    }
}