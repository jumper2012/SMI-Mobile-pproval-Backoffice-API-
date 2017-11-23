using System;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using log4net;
using log4net.Config;

namespace JrzAsp.Lib.Logging {
    public static class LogService {

        public delegate ILog FuncGetLoggerByName(string name);

        public delegate ILog FuncGetLoggerByType(Type type);
        private const string DEFAULT_CONFIG_RESOURCE_NAME = "JrzAsp.Lib.Logging.Res.DefaultLog4NetConfig.xml";

        private static string _defaultConfigXml;

        public static FuncGetLoggerByName DefaultGetLoggerByName = name => new DefaultLog(LogManager.GetLogger(name));
        public static FuncGetLoggerByType DefaultGetLoggerByType = type => new DefaultLog(LogManager.GetLogger(type));

        static LogService() {
            var configAbsolutePath = HostingEnvironment.MapPath(MyAppSettings.ConfigRelativePath);

            if (!File.Exists(configAbsolutePath)) {
                var configDir = Path.GetDirectoryName(configAbsolutePath);
                if (!Directory.Exists(configDir)) Directory.CreateDirectory(configDir);
                File.WriteAllText(configAbsolutePath, DefaultConfigXml);
            }

            var cfgFinfo = new FileInfo(configAbsolutePath);
            XmlConfigurator.ConfigureAndWatch(cfgFinfo);

            GetLoggerByName = DefaultGetLoggerByName;
            GetLoggerByType = DefaultGetLoggerByType;
        }

        private static string DefaultConfigXml {
            get {
                if (_defaultConfigXml != null) return _defaultConfigXml;
                var myAsm = Assembly.GetExecutingAssembly();
                using (var stream = myAsm.GetManifestResourceStream(DEFAULT_CONFIG_RESOURCE_NAME))
                using (var reader = new StreamReader(stream)) {
                    _defaultConfigXml = reader.ReadToEnd();
                }
                return _defaultConfigXml;
            }
        }

        public static FuncGetLoggerByName GetLoggerByName { get; set; }
        public static FuncGetLoggerByType GetLoggerByType { get; set; }

        public static ILog GetLogger(string name) {
            return GetLoggerByName(name);
        }

        public static ILog GetLogger(Type type) {
            return GetLoggerByType(type);
        }

        private static Stream GenerateStreamFromString(string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}