#region Version Info Header
/*
 * $Id: ExpressionExtensions.cs 87603 2013-11-05 11:11:23Z unknown $
 * $HeadURL: https://torsteroids.svn.codeplex.com/svn/trunk/source/TorSteroids.Common/Extensions/ExpressionExtensions.cs $
 * Last modified by $Author: unknown $
 * Last modified at $Date: 2013-11-05 12:11:23 +0100 (Di, 05 Nov 2013) $
 * $Revision: 87603 $
 */
#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace TorSteroids.Common.Extensions
{
	/// <summary>
	/// Linq expression extensions
	/// </summary>
	/// <remarks>
	/// Thanks to: From http://www.ingebrigtsen.info/post/2008/12/11/INotifyPropertyChanged-revisited.aspx
	/// </remarks>
	public static class ExpressionExtensions
	{
		/// <summary>
		/// Extends the specified event handler using a LINQ expression instead of plain string literals for property names.
		/// </summary>
		/// <param name="eventHandler">The event handler.</param>
		/// <param name="expression">The expression.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="expression"/> is null</exception>
		public static void Notify(this PropertyChangedEventHandler eventHandler, [NotNull] Expression<Func<Object>> expression)
		{
			expression.ExceptionIfNull("expression");

			if (null == eventHandler)
				return;
			
			LambdaExpression lambda = expression;
			MemberExpression memberExpression;
			if (lambda.Body is UnaryExpression)
			{
				UnaryExpression unaryExpression = lambda.Body as UnaryExpression;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else
			{
				memberExpression = lambda.Body as MemberExpression;
			}
			ConstantExpression constantExpression = memberExpression.Expression as ConstantExpression;
			PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;

			foreach (var del in eventHandler.GetInvocationList())
			{
				del.DynamicInvoke(new [] { constantExpression.Value, new PropertyChangedEventArgs(propertyInfo.Name) });
			}

		}

		/// <summary>
		/// Gets the name of a property provided as a lambda expression, e.g. "() =&gt; MyPropertyName".
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">expression</exception>
		public static string GetPropertyName([NotNull] this Expression<Func<Object>> expression)
		{
			expression.ExceptionIfNull("expression");

			LambdaExpression lambda = expression;
			MemberExpression memberExpression;
			if (lambda.Body is UnaryExpression)
			{
				UnaryExpression unaryExpression = lambda.Body as UnaryExpression;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else
			{
				memberExpression = lambda.Body as MemberExpression;
			}

			PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;

			return propertyInfo.Name;

		}
	}
}
