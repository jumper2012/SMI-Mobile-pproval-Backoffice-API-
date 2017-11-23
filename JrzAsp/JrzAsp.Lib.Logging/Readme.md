# How to Install in ASP MVC Web App #

1.  Include libs (except any that you already have).
2.  ~/App_Data must be writable by the IIS AppPool. ~/App_Data/Logs is where all log files will be stored.
3.  Add to web.config into configuration/system.webServer/modules
    `<add name="ErrorLogger" type="JrzAsp.Lib.Logging.Modules.ErrorLoggerHttpModule"/>`
4.  Adjust config as needed:
    -   `JrzAsp.Lib.Logging:MinErrorHttpCode`
        Minimum http status code to log as error. Defaults to 400.
    -   `JrzAsp.Lib.Logging:ConfigRelativePath`
        log4net.config file location. Defaults to ~/App_Data/JrzAsp-Lib-Logging/log4net.config. A default log4net config will be generated and saved there if the file hasn't exists on webapp start.
        