using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using TorSteroids.Common.Extensions;

namespace TorSteroids.Common.ViewModel
{
	/// <summary>
	/// A base class for View Model classes in the MVVM pattern.
	/// </summary>
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Provides access to the PropertyChanged event handler to derived classes.
		/// </summary>
		protected PropertyChangedEventHandler PropertyChangedHandler
		{
			get
			{
				return PropertyChanged;
			}
		}


		#region INotifyPropertyChanged and related

		protected virtual void OnPropertyChanged(Expression<Func<object>> expression)
		{
			string name = expression.GetPropertyName();
			OnPropertyChanged(name);
		}

		/// <summary>
		/// Raises the PropertyChanged event if needed.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed.</param>
		protected virtual void OnPropertyChanged(
#if !SILVERLIGHT
			[CallerMemberName]
#endif
			string propertyName = null)
		{
			VerifyPropertyName(propertyName);
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected void OnPropertyChanged(params String[] propertyNames)
		{
			const String STR_PROPERTYNAMES = "propertyNames";

			if (propertyNames == null)
				throw new ArgumentNullException(STR_PROPERTYNAMES);

			if (propertyNames.Length == 0)
				throw new ArgumentOutOfRangeException(STR_PROPERTYNAMES, "Empty array");

			foreach (var propertyName in propertyNames)
			{
				this.OnPropertyChanged(propertyName);
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var handler = this.PropertyChangedHandler;
			if (handler != null)
			{
				handler(this, e);
			}
		}


		/// <summary>
		/// Warns the developer if this Object does not have a public property with
		/// the specified name. This method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(String propertyName)
		{
			// verify that the property name matches a real,  
			// public, instance property on this object.
#if !PORTABLE
			Debug.Assert(GetType().GetProperty(propertyName) != null, "Invalid property name: " + propertyName);
#endif
		}

		#endregion
	}
}
