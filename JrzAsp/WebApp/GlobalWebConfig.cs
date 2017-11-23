using System;
using System.Web.Configuration;

namespace WebApp {
    public static class GlobalWebConfig {
        public static bool DbMigration_MigrateOnAppStart {
            get {
                var boolStr = WebConfigurationManager.AppSettings["DbMigration:MigrateOnAppStart"];
                if (bool.TryParse(boolStr, out var yes)) {
                    return yes;
                }
                return false;
            }
        }

        public static bool DbMigration_AllowDataLoss {
            get {
                var boolStr = WebConfigurationManager.AppSettings["DbMigration:AllowDataLoss"];
                if (bool.TryParse(boolStr, out var yes)) {
                    return yes;
                }
                return false;
            }
        }

        public static string Authentication_CookieAuthName {
            get {
                var name = WebConfigurationManager.AppSettings["Authentication:CookieAuthName"];
                if (string.IsNullOrWhiteSpace(name)) {
                    throw new InvalidOperationException(
                        $"App setting 'Authentication:CookieAuthName' must be defined in Web.config.");
                }
                return name.Trim();
            }
        }

        public static string MailSender_DefaultFromEmailAddress {
            get {
                var emailAddr = WebConfigurationManager.AppSettings["MailSender:DefaultFromEmailAddress"];
                if (string.IsNullOrWhiteSpace(emailAddr)) {
                    throw new InvalidOperationException(
                        $"App setting 'MailSender:DefaultFromEmailAddress' must be defined in Web.config.");
                }
                return emailAddr.Trim();
            }
        }
    }
}