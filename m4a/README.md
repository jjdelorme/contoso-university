# Migrate for Anthos
This directory contains artifacts generated from M4A.
Run a docker build on this directory to create the container image.

## Connection Strings
Note that the database connection string is stored as a K8S secret.  To reference this secret with no code changes, use the following ```configSource``` in the Web.config file.  Notice the path.

```xml
<configuration>
  ...
  <connectionStrings configSource="C:\\secret\\connectionStrings.config"/>
  ...
</configuration>
```

The secret is created as such from a file that contains just the connectionStrings section, i.e.:

```xml
<connectionStrings>
    <add name="SchoolContext"
         connectionString="Data Source=SERVER-NAME;User=MYUSER;Password=MYPASSWORD;Initial Catalog=ContosoUniversity;"
        providerName="System.Data.SqlClient" />
</connectionStrings>
```

```bash
kubectl create secret generic connection-strings -n contoso --from-file=connectionStrings.config
```

The deployment then references this with:

```yaml
        volumeMounts: 
        - name: connection-strings
          mountPath: "/secret"
          readOnly: true        
...
      volumes:
      - name: connection-strings
        secret:
          secretName: connection-strings
```
## Debuging Windows Containers
Quick note on how you can debug a Windows Container, i.e. to see if the secret properly gets mounted.  You can jump into a command prompt by using kubectl exec:

```bash
kubectl exec -it -n contoso contoso-57d6d9887d-4mdnf cmd
...
C:\>_
```
