# Cymbal University Sample
This sample is a slight modification of the ASP.NET MVC 5 application that demonstrates a legacy ASP.NET & Entity Framework IIS application using SQL Server as a backing store.

## Deployment

To deploy you will follow these instructions to create a SQL Server database and an IIS Site to deploy from a Web Deploy package located in this repo at ~~~./WebDeploy/

### Create Database

1. Create an empty SQL Server database named CymbalUniversity 
1. Create a SQL user with db_owner access to the database
1. Create a config file named ```connectionStrings.config``` that contains the connection string and deploy this in the same directory as ```Web.config```

```xml
<connectionStrings>
    <add name="SchoolContext"
         connectionString="Data Source=SERVER-NAME;User=MYUSER;Password=MYPASSWORD;Initial Catalog=CymbalUniversity;"
        providerName="System.Data.SqlClient" />
</connectionStrings>
```

Note that the connectionStrings.config file is referenced from ```Web.config```:
```xml
<configuration>
  ...
  <connectionStrings configSource="connectionStrings.config"/>
  ...
</configuration>
```

### Populate database with sample data

 If you are doing this for the first time and the database has not been created,open up nuget package manager console and execute:
 ```
 update-database -Verbose
 ```

### Create an IIS Site

1. Create a host directory

```bash
mkdir c:\inetpub\wwwroot\CymbalUniversity
```

1. Create a new IIS website through the IIS Manager or with PowerShell:

```bash
New-Website -Name 'CymbalUniversity' -PhysicalPath 'c:\inetpub\wwwroot\cymbaluniversity' -Port 80 -Force; `
c:\windows\system32\inetsrv\appcmd.exe set site \"CymbalUniversity\" \"/[path='/'].applicationPool:CymbalUniversity\";
```

### Deploy the application

1. Execute the deployment command from the root of where you cloned this repo:

```bash
cd WebDeploy
CymbalUniversity.deploy.cmd /Y
```

1. Test the application in your browser: ```http://localhost```

