#region License / Copyright
/* 
Copyright (c) 2013, TorSteroids
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name TorSteroids nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Version Info Header
/*
 * $Id: FluentContextTC.cs 85556 2013-03-13 13:33:33Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Fluent/FluentContextTC.cs $
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
	/// Supports you to write/provide more readable code.
	/// </summary>
	/// <remarks>
	/// For debugging hints, error reporting and logging through the call chain please 
	/// refer to http://en.wikipedia.org/wiki/Fluent_interface#Problems
	/// </remarks>
	/// <typeparam name="TC">The type to be chained by the fluent context.</typeparam>
	/// <see cref="http://en.wikipedia.org/wiki/Fluent_interface"/>
    public class FluentContext<TC> : IFluentInterface
    {
        protected TC Context { get; private set; }

        public FluentContext(TC context)
        {
            this.Context = context;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return Context.Equals(obj);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return Context.GetHashCode();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return Context.GetType();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return Context.ToString();
        }
    }
}