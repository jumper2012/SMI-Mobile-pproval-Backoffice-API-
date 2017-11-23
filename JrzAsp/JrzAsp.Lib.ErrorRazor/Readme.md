# How to Install in ASP MVC Web App #

1.  Include libs (except any that you already have)
2.  In web.config in configuration/system.webServer/modules add
    `<add name="ErrorRazor" type="JrzAsp.Lib.ErrorRazor.Modules.ErrorRazorHttpModule"/>`
3.  Adjust web.config as needed:
    -   `JrzAsp.Lib.ErrorRazor:HandledStatusCodes`
        Defaults to "4xx,5xx".
        Fill it with something like '4xx,5xx' to handle all 400-599 status code. The 'x' is like any number placeholder. Use comma ',' to separate different values.
    -   `JrzAsp.Lib.ErrorRazor:ErrorViewNamePrefix`
        Defaults to "_error_view_".
        Adjust the view name prefix to search for when error came. For example, if error 400 happens, and view name prefix is "_error_view_", then the module will search for "_error_view_400" view.