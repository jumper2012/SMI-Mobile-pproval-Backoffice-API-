using System.Linq;
using JrzAsp.Lib.ProtoCms.Setting.Database;
using JrzAsp.Lib.ProtoCms.Setting.Models;
using JrzAsp.Lib.ProtoCms.Setting.Services;
using JrzAsp.Lib.ProtoCms.Vue.Models;
using JrzAsp.Lib.RepoPattern;
using JrzAsp.Lib.TypeUtilities;

namespace JrzAsp.Lib.ProtoCms.Setting.CoreSettings.WelcomeCard {
    public class WelcomeCardSettingHandler : ISiteSettingHandler {
        public SiteSetting GetSettingObject(ProtoSettingField[] settingFields) {
            var heading = settingFields.FirstOrDefault(
                              x => x.FieldName == nameof(WelcomeCardSetting.WelcomeHeading))?.StringValue
                          ?? "Welcome";
            var body = settingFields.FirstOrDefault(
                           x => x.FieldName == nameof(WelcomeCardSetting.WelcomeBody))?.StringValue
                       ?? "<b>Start</b> managing your contents by accessing the <i>main menu</i>.";

            var ss = new WelcomeCardSetting(heading, body);
            return ss;
        }

        public SiteSettingModifierForm BuildSettingModifierForm(SiteSetting oldSetting) {
            var ss = oldSetting.DirectCastTo<WelcomeCardSetting>();
            var form = new WelcomeCardSettingForm {
                WelcomeHeading = ss.WelcomeHeading,
                WelcomeBody = ss.WelcomeBody
            };
            return form;
        }

        public VueComponentDefinition[] CreateSettingModifierFormVues(SiteSettingModifierForm form,
            SiteSetting oldSetting) {
            var vues = new[] {
                new VueComponentDefinition {
                    Name = "cms-form-field-text",
                    Props = new {
                        label = "Welcome Heading",
                        valuePath = nameof(WelcomeCardSettingForm.WelcomeHeading)
                    }
                },
                new VueComponentDefinition {
                    Name = "cms-form-field-rich-html",
                    Props = new {
                        label = "Welcome Body",
                        valuePath = nameof(WelcomeCardSettingForm.WelcomeBody)
                    }
                }
            };
            return vues;
        }

        public FurtherValidationResult ValidateSettingModifierForm(SiteSettingModifierForm form,
            SiteSetting oldSetting) {
            return FurtherValidationResult.Ok;
        }

        public ProtoSettingField[] UpdateAndRebuildNewSettingFields(SiteSettingModifierForm form,
            SiteSetting oldSetting) {
            var fm = form.DirectCastTo<WelcomeCardSettingForm>();
            var fields = new[] {
                new ProtoSettingField {
                    FieldName = nameof(WelcomeCardSetting.WelcomeHeading),
                    StringValue = fm.WelcomeHeading
                },
                new ProtoSettingField {
                    FieldName = nameof(WelcomeCardSetting.WelcomeBody),
                    StringValue = fm.WelcomeBody
                }
            };
            return fields;
        }
    }
}