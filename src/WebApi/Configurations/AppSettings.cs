using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assistance.Operational.WebApi.Configurations
{
    /// <summary>
    /// Application settings
    /// </summary>
    public class AppSettings
    {
        private readonly Regex _semverRegex = new Regex(
            @"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Name of the API
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Application Namespace
        /// </summary>
        public string ApplicationAssemblyFullNamespace => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// Origins allowed for CORS requests
        /// </summary>
        public string[] CorsOrigins { get; set; } = { };

        /// <summary>
        /// Major component of the version number
        /// </summary>
        public int DefaultMajorVersion => int.Parse(SemanticVersionInfo["major"]);

        /// <summary>
        /// Minor component of the version number
        /// </summary>
        public int DefaultMinorVersion => int.Parse(SemanticVersionInfo["minor"]);

        /// <summary>
        /// Locale of the application - used for API business error messages 
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Version number
        /// </summary>
        public string Version { get; set; } = "0.0.0";

        /// <summary>
        /// Semver informations of the application
        /// </summary>
        private IDictionary<string, string> SemanticVersionInfo =>
            _semverRegex.Match(Version).Groups.Values.ToDictionary(x => x.Name, x => x.Value);
    }
}
