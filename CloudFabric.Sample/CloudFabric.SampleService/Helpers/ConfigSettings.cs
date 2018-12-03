namespace CloudFabric.SampleService.Helpers
{
    using System.Fabric;
    using System.Fabric.Description;
    using CloudFabric.Library.Common.Utilities;

    public class ConfigSettings : ConfigUtil
    {
        public string LogglyToken { get; set; }
        public string IdentityServerHost { get; set; }
        public string Environment { get; private set; }
        public string ApplicationName { get; private set; }
        public string InstrumentationKey { get; private set; }
        public string DatabaseConnectionString { get; private set; }

        public ConfigSettings(StatelessServiceContext context) : base(context)
        {

        }

        public override void UpdateConfigSettings(ConfigurationSettings settings)
        {
            LogglyToken = GetValue(settings, "Environment", "LogglyToken");
            ApplicationName = GetValue(settings, "Environment", "ApplicationName");
            Environment = GetValue(settings, "Environment", "Environment");
            IdentityServerHost = GetValue(settings, "Environment", "IdentityServerHost");
            InstrumentationKey = GetValue(settings, "Environment", "InstrumentationKey");
            DatabaseConnectionString = GetValue(settings, "Database", "DatabaseConnectionString");
        }
    }
}
