# Contoso University PCF Sample
This sample is a slight modification of the ASP.NET MVC 5 application that
demonstrates Contoso University.

To get this to work, you must have only one user-defined service. This service
needs a single field called connectionString, a sample looks like this:

 "Data Source=(SERVER);Integrated Security=False;User ID=(USER);Password=(PASSWORD);Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False; Initial Catalog=ContosoUniversity2"

 If you are doing this for the first time and the database has not been created,
 then uncomment the connection string in web.config, paste in your own, open up
 nuget package manager console and execute 'update-database -Verbose'. This will create the ContosoUniversity2 database on whatever server you chose. Once you've created the database using nuget console manager, remove or comment out the connection string from web.config, you won't need it any longer.

 If this database already exists, you do not need to perform this step.


 Make sure your UPS is bound to your diego-hosted application. Once you've done
 that, you should be able to follow the usual diego deployment instructions to get this working.
