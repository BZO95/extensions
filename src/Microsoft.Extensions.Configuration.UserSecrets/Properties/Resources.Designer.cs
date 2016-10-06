// <auto-generated />
namespace Microsoft.Extensions.Configuration.UserSecrets
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.Extensions.Configuration.UserSecrets.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// Value cannot be null or an empty string.
        /// </summary>
        internal static string Common_StringNullOrEmpty
        {
            get { return GetString("Common_StringNullOrEmpty"); }
        }

        /// <summary>
        /// Value cannot be null or an empty string.
        /// </summary>
        internal static string FormatCommon_StringNullOrEmpty()
        {
            return GetString("Common_StringNullOrEmpty");
        }

        /// <summary>
        /// Invalid character '{0}' found in 'userSecretsId' value at index '{1}'.
        /// </summary>
        internal static string Error_Invalid_Character_In_UserSecrets_Id
        {
            get { return GetString("Error_Invalid_Character_In_UserSecrets_Id"); }
        }

        /// <summary>
        /// Invalid character '{0}' found in 'userSecretsId' value at index '{1}'.
        /// </summary>
        internal static string FormatError_Invalid_Character_In_UserSecrets_Id(object p0, object p1)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_Invalid_Character_In_UserSecrets_Id"), p0, p1);
        }

        /// <summary>
        /// Unable to locate a project.json at '{0}'.
        /// </summary>
        internal static string Error_Missing_Project_Json
        {
            get { return GetString("Error_Missing_Project_Json"); }
        }

        /// <summary>
        /// Unable to locate a project.json at '{0}'.
        /// </summary>
        internal static string FormatError_Missing_Project_Json(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_Missing_Project_Json"), p0);
        }

        /// <summary>
        /// Missing 'userSecretsId' in '{0}'.
        /// </summary>
        internal static string Error_Missing_UserSecretId_In_Project_Json
        {
            get { return GetString("Error_Missing_UserSecretId_In_Project_Json"); }
        }

        /// <summary>
        /// Missing 'userSecretsId' in '{0}'.
        /// </summary>
        internal static string FormatError_Missing_UserSecretId_In_Project_Json(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_Missing_UserSecretId_In_Project_Json"), p0);
        }

        /// <summary>
        /// Could not find 'UserSecretsIdAttribute' on assembly '{0}'.
        /// </summary>
        internal static string Error_Missing_UserSecretsIdAttribute
        {
            get { return GetString("Error_Missing_UserSecretsIdAttribute"); }
        }

        /// <summary>
        /// Could not find 'UserSecretsIdAttribute' on assembly '{0}'.
        /// </summary>
        internal static string FormatError_Missing_UserSecretsIdAttribute(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Error_Missing_UserSecretsIdAttribute"), p0);
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
