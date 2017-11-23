# Dev Notes for Custom Implementation #

1.  Change `[assembly: AssemblyTitle("WebApp")]` and `[assembly: Guid("b208b93a-1e36-4858-94dc-4fe9ee0a246e")]`
    in *~/Properties/AssemblyInfo.cs*. Also change the assembly name in WebApp's project properties.
    If you change the assembly name, then make sure to also change
    `config.SetDocumentationProvider(new XmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/bin/WebApp.xml")));`
    so that the generated doc xml location is correct (see the WebApp's project properties).

2.  Change `DefaultConnection` *connection string* inside *~/Web.config*, replace string `aspnet-WebApp-20171024073236`
    with other string representing the custom web app being built.

3.  Open *WebApp.csproj* in text editor, inside xml path *Project/ProjectExtensions/VisualStudio/FlavorProperties/WebProjectProperties*
    change *DevelopmentServerPort* and match the port change to *IISUrl* as well. Reload project after changing the
    csproj.

4.  Change `Authentication:CookieAuthName` app setting in web.config to be more representative of the web app being
    built.

5.  Define your app content types in *~/Features/ApplicationContentTypes/AppContentTypesProvider.cs*.

6.  Define your app datum types in *~/Features/ApplicationDatumTypes/AppDatumTypesProvider.cs*.

# Default AssemblyInfo and Csproj values #

## AssemblyInfo.cs ##

```C#
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WebApp")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("WebApp")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("b208b93a-1e36-4858-94dc-4fe9ee0a246e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
```

## Csproj IIS Part ##

```XML
<ProjectExtensions>
    <VisualStudio>
        <FlavorProperties GUID="{349c5851-65df-11da-9384-00065b846f21}">
            <WebProjectProperties>
                <UseIIS>True</UseIIS>
                <AutoAssignPort>True</AutoAssignPort>
                <DevelopmentServerPort>58559</DevelopmentServerPort>
                <DevelopmentServerVPath>/</DevelopmentServerVPath>
                <IISUrl>http://localhost:58559/</IISUrl>
                <NTLMAuthentication>False</NTLMAuthentication>
                <UseCustomServer>False</UseCustomServer>
                <CustomServerUrl>
                </CustomServerUrl>
                <SaveServerSettingsInUserFile>False</SaveServerSettingsInUserFile>
            </WebProjectProperties>
        </FlavorProperties>
    </VisualStudio>
</ProjectExtensions>
```