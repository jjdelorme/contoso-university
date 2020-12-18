# escape=`

FROM mcr.microsoft.com/dotnet/framework/aspnet:3.5-windowsservercore-1909
# Set shell to powershell
SHELL ["powershell", "-Command", "$ProgressPreference = 'SilentlyContinue'; $VerbosePreference = 'Continue'; $ErrorActionPreference = 'Stop';"]

# Add support for ASP.NET 45
RUN Add-WindowsFeature NET-Framework-45-ASPNET; `
    Add-WindowsFeature Web-Asp-Net45;

# Install Roslyn compilers and ngen binaries
RUN Invoke-WebRequest https://api.nuget.org/packages/microsoft.net.compilers.2.9.0.nupkg -OutFile c:\microsoft.net.compilers.2.9.0.zip; `
    Expand-Archive -Path c:\microsoft.net.compilers.2.9.0.zip -DestinationPath c:\RoslynCompilers; `
    Remove-Item c:\microsoft.net.compilers.2.9.0.zip -Force; `
    &C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ngen.exe update; `
    &C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe update; `
    &C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ngen.exe install c:\RoslynCompilers\tools\csc.exe /ExeConfig:c:\RoslynCompilers\tools\csc.exe | `
    &C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ngen.exe install c:\RoslynCompilers\tools\vbc.exe /ExeConfig:c:\RoslynCompilers\tools\vbc.exe | `
    &C:\Windows\Microsoft.NET\Framework64\v4.0.30319\ngen.exe install c:\RoslynCompilers\tools\VBCSCompiler.exe /ExeConfig:c:\RoslynCompilers\tools\VBCSCompiler.exe | `
    &C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe install c:\RoslynCompilers\tools\csc.exe /ExeConfig:c:\RoslynCompilers\tools\csc.exe | `
    &C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe install c:\RoslynCompilers\tools\vbc.exe /ExeConfig:c:\RoslynCompilers\tools\vbc.exe | `
    &C:\Windows\Microsoft.NET\Framework\v4.0.30319\ngen.exe install c:\RoslynCompilers\tools\VBCSCompiler.exe  /ExeConfig:c:\RoslynCompilers\tools\VBCSCompiler.exe

ENV ROSLYN_COMPILER_LOCATION c:\RoslynCompilers\tools

# Enabling extra Windows features
RUN Enable-WindowsOptionalFeature -Online -All -FeatureName IIS-NetFxExtensibility45; `
Enable-WindowsOptionalFeature -Online -All -FeatureName IIS-WebServerManagementTools

# Setting up IIS
RUN c:\windows\system32\inetsrv\appcmd.exe set config \"/section:httpLogging\" \"/dontLog:False\"

# Removing default IIS Website
RUN Remove-Website -Name 'Default Web Site'

# Creating IIS AppPools
RUN New-WebAppPool -Name 'CymbalUniversity' -Force; `
c:\windows\system32\inetsrv\appcmd.exe set apppool \"CymbalUniversity\" \"/processModel.identityType:ApplicationPoolIdentity\";

# Adding website CymbalUniversity
RUN New-Website -Name 'CymbalUniversity' -PhysicalPath 'c:\inetpub\wwwroot\cymbaluniversity' -Port 80 -Force; `
c:\windows\system32\inetsrv\appcmd.exe set site \"CymbalUniversity\" \"/[path='/'].applicationPool:CymbalUniversity\";

COPY . \deploy

WORKDIR \deploy

RUN ContosoUniversity.deploy.cmd /Y