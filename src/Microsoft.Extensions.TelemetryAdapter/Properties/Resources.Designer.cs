// <auto-generated />
namespace Microsoft.Extensions.TelemetryAdapter
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.Extensions.TelemetryAdapter.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// The property '{0}' on type '{1}' must define a getter to support proxy generation.
        /// </summary>
        internal static string Converter_PropertyMustHaveGetter
        {
            get { return GetString("Converter_PropertyMustHaveGetter"); }
        }

        /// <summary>
        /// The property '{0}' on type '{1}' must define a getter to support proxy generation.
        /// </summary>
        internal static string FormatConverter_PropertyMustHaveGetter(object p0, object p1)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Converter_PropertyMustHaveGetter"), p0, p1);
        }

        /// <summary>
        /// The property '{0}' on type '{1}' must not use index parameters to support proxy generation.
        /// </summary>
        internal static string Converter_PropertyMustNotHaveIndexParameters
        {
            get { return GetString("Converter_PropertyMustNotHaveIndexParameters"); }
        }

        /// <summary>
        /// The property '{0}' on type '{1}' must not use index parameters to support proxy generation.
        /// </summary>
        internal static string FormatConverter_PropertyMustNotHaveIndexParameters(object p0, object p1)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Converter_PropertyMustNotHaveIndexParameters"), p0, p1);
        }

        /// <summary>
        /// The property '{0}' on type '{1}' must not define a setter to support proxy generation.
        /// </summary>
        internal static string Converter_PropertyMustNotHaveSetter
        {
            get { return GetString("Converter_PropertyMustNotHaveSetter"); }
        }

        /// <summary>
        /// The property '{0}' on type '{1}' must not define a setter to support proxy generation.
        /// </summary>
        internal static string FormatConverter_PropertyMustNotHaveSetter(object p0, object p1)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Converter_PropertyMustNotHaveSetter"), p0, p1);
        }

        /// <summary>
        /// Type '{0}' must be an interface in order to support proxy generation from source type '{1}'.
        /// </summary>
        internal static string Converter_TypeMustBeInterface
        {
            get { return GetString("Converter_TypeMustBeInterface"); }
        }

        /// <summary>
        /// Type '{0}' must be an interface in order to support proxy generation from source type '{1}'.
        /// </summary>
        internal static string FormatConverter_TypeMustBeInterface(object p0, object p1)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Converter_TypeMustBeInterface"), p0, p1);
        }

        /// <summary>
        /// The type '{0}' defines multiple properties with varying casings of '{1}'. This is ambiguous for proxy method generation.
        /// </summary>
        internal static string Converter_TypeMustNotHavePropertiesWithDifferentCasing
        {
            get { return GetString("Converter_TypeMustNotHavePropertiesWithDifferentCasing"); }
        }

        /// <summary>
        /// The type '{0}' defines multiple properties with varying casings of '{1}'. This is ambiguous for proxy method generation.
        /// </summary>
        internal static string FormatConverter_TypeMustNotHavePropertiesWithDifferentCasing(object p0, object p1)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Converter_TypeMustNotHavePropertiesWithDifferentCasing"), p0, p1);
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
