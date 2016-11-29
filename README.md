# HuShienBot
An artifical variant of Mr Tan Hu Shien. 
[![Build Status](https://travis-ci.org/nixxholas/HuShienBot.svg?branch=master)](https://travis-ci.org/nixxholas/HuShienBot) 



### Getting Started 
Before you begin testing/deploying this bot, be sure to have a web.config prepared. 
It looks like this: 
```
<?xml version="1.0" encoding="utf-8"?>
<!--
  For more information on how to configure your ASP.NET application, please visit
  http://go.microsoft.com/fwlink/?LinkId=301879
  -->
<configuration>
  <appSettings>
    <!-- update these with your BotId, Microsoft App Id and your Microsoft App Password-->
    <add key="BotId" value="xxxxxx" />
    <add key="MicrosoftAppId" value="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" />
    <add key="MicrosoftAppPassword" value="xxxxxxxxxxxxxxxxxxxxxx" />
  </appSettings>
  <!--
    For a description of web.config changes see http://go.microsoft.com/fwlink/?LinkId=235367.

    The following attributes can be set on the <httpRuntime> tag.
      <system.Web>
        <httpRuntime targetFramework="4.6" />
      </system.Web>
  -->
  <system.web>
    <customErrors mode="Off" />
    <compilation debug="true" targetFramework="4.6" />
    <httpRuntime targetFramework="4.6" />
  </system.web>
  <system.webServer>
    <defaultDocument>
      <files>
        <clear />
        <add value="default.htm" />
      </files>
    </defaultDocument>
    <handlers>
      <remove name="ExtensionlessUrlHandler-Integrated-4.0" />
      <remove name="OPTIONSVerbHandler" />
      <remove name="TRACEVerbHandler" />
      <add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="*" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />
    </handlers>
  </system.webServer>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Helpers" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-3.0.0.0" newVersion="3.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-5.2.3.0" newVersion="5.2.3.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.WebPages" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-3.0.0.0" newVersion="3.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-8.0.0.0" newVersion="8.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Net.Http.Primitives" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.2.29.0" newVersion="4.2.29.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Net.Http.Formatting" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-5.2.3.0" newVersion="5.2.3.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
  </configuration>
``` 

Within the BasicDialog.cs, you'll notice that there'll be a line like this: 
```
[LuisModel("e01e77ec-d2a9-47a1-bf48-5c37ef5055ad", "3d9fbecb4439471ca22498c171ed9716")]
```
That is my LUIS Token, feel free to use it for my own contexts. However, you're advised to use your own LUIS token and LUIS Project 
to build upon as I won't have similar data to what you have. What I've provided you so far is just a foundation for you to build upon. 
You're strongly encouraged to revamp the database. 