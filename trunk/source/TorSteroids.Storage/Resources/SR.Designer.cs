﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TorSteroids.Storage.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TorSteroids.Storage.Resources.SR", typeof(SR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find table field with name &apos;{0}&apos;.
        /// </summary>
        internal static string AccessToUnknownTableFieldException {
            get {
                return ResourceManager.GetString("AccessToUnknownTableFieldException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find table field with index &apos;{0}&apos;.
        /// </summary>
        internal static string AccessToUnknownTableFieldIndexException {
            get {
                return ResourceManager.GetString("AccessToUnknownTableFieldIndexException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid DateTimeKind value.
        /// </summary>
        internal static string ArgumentOutOfRangeInvalidDateTimeKind {
            get {
                return ResourceManager.GetString("ArgumentOutOfRangeInvalidDateTimeKind", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid date format.
        /// </summary>
        internal static string DbDateInvalidFormatException {
            get {
                return ResourceManager.GetString("DbDateInvalidFormatException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only digits allowed.
        /// </summary>
        internal static string DbDateTimeAcceptsDigitsOnlyFormatException {
            get {
                return ResourceManager.GetString("DbDateTimeAcceptsDigitsOnlyFormatException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid time format.
        /// </summary>
        internal static string DbTimeInvalidFormatException {
            get {
                return ResourceManager.GetString("DbTimeInvalidFormatException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A general storage exception occured.
        /// </summary>
        internal static string GeneralStorageException {
            get {
                return ResourceManager.GetString("GeneralStorageException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid cast from &apos;{0}&apos; to &apos;{1}&apos;..
        /// </summary>
        internal static string InvalidCast_FromTo {
            get {
                return ResourceManager.GetString("InvalidCast_FromTo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &apos;{0}&apos; of type &apos;{1}&apos; can not be converted to the target type &apos;{2}&apos;.
        ///Additional info: &apos;{3}&apos;.
        /// </summary>
        internal static string InvalidDbTypeCastException {
            get {
                return ResourceManager.GetString("InvalidDbTypeCastException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The database field &apos;{0}&apos; value &apos;{1}&apos; of type &apos;{2}&apos; does not match and can not be converted to the target type &apos;{3}&apos;.
        ///Additional info: &apos;{4}&apos;.
        /// </summary>
        internal static string InvalidDbTypeCastExceptionAtField {
            get {
                return ResourceManager.GetString("InvalidDbTypeCastExceptionAtField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &apos;{0}&apos; is not a defined member of target Enum &apos;{1}&apos; and can not be converted
        ///Additional info: &apos;{3}&apos;.
        /// </summary>
        internal static string InvalidEnumCastException {
            get {
                return ResourceManager.GetString("InvalidEnumCastException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage delete command failed on &apos;{0}&apos;: {1}.
        /// </summary>
        internal static string StorageDeleteException {
            get {
                return ResourceManager.GetString("StorageDeleteException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage execute non-query command failed: &apos;{0}&apos;.
        /// </summary>
        internal static string StorageExecuteNonQueryException {
            get {
                return ResourceManager.GetString("StorageExecuteNonQueryException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage execute data reader command failed: &apos;{0}&apos;.
        /// </summary>
        internal static string StorageExecuteReaderException {
            get {
                return ResourceManager.GetString("StorageExecuteReaderException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage execute scalar command failed: &apos;{0}&apos;.
        /// </summary>
        internal static string StorageExecuteScalarException {
            get {
                return ResourceManager.GetString("StorageExecuteScalarException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage insert command failed on &apos;{0}&apos;: {1}.
        /// </summary>
        internal static string StorageInsertException {
            get {
                return ResourceManager.GetString("StorageInsertException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage mark deleted command failed on &apos;{0}&apos;: {1}.
        /// </summary>
        internal static string StorageMarkDeletedException {
            get {
                return ResourceManager.GetString("StorageMarkDeletedException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Open the Storage failed: &apos;{0}&apos;.
        /// </summary>
        internal static string StorageOpenException {
            get {
                return ResourceManager.GetString("StorageOpenException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Storage update command failed on &apos;{0}&apos;: {1}.
        /// </summary>
        internal static string StorageUpdateException {
            get {
                return ResourceManager.GetString("StorageUpdateException", resourceCulture);
            }
        }
    }
}