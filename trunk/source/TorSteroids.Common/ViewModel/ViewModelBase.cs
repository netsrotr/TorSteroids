using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using TorSteroids.Common.Extensions;

namespace TorSteroids.Common.ViewModel
{
    /// <summary>
    /// A base class for the ViewModel classes in the MVVM pattern.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Design",
        "CA1012",
        Justification = "Constructors should remain public to allow serialization.")]
    public abstract class ViewModelBase : ObservableObject
    {
        private static bool? _isInDesignMode;


        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running under Blend or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Non static member needed for data binding")]
        protected abstract bool IsInDesignMode { get; }

       
        /// <summary>
        /// Raises the PropertyChanged event if needed
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        protected virtual void OnPropertyChanged<T>(string propertyName, T oldValue, T newValue)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than other alternatives.")]
        protected virtual void OnPropertyChanged<T>(Expression<Func<object>> propertyExpression, T oldValue, T newValue)
        {
            if (propertyExpression == null)
            {
                return;
            }

            var handler = PropertyChangedHandler;

            if (handler != null)
            {
                //var body = propertyExpression.Body as MemberExpression;
                //var expression = body.Expression as ConstantExpression;
                string name = propertyExpression.GetPropertyName();
                OnPropertyChanged(name);
            }
        }
    }
}
