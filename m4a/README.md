# Migrate for Anthos
This directory contains artifacts generated from M4A.
Run a docker build on this directory to create the container image.

## Connection Strings
Note that the database connection string is stored as a K8S secret.  To reference this secret with no code changes, use the following ```configSource``` in the Web.config file.  Notice the path is relative to the deployment, a subdirectory in the deployment path.

```xml
<configuration>
  ...
  <connectionStrings configSource="secret\connectionStrings.config"/>
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

The deployment then references this with below.  NOTE the mount path is a fully qualified subdirectory of the deployment (relative to C:\).  Specifying a root directory like /secret for example is at risk of IIS not having permissions to read and will through a 500 error.  Additionally specifying the actual path of deployment (not a subdirectory) will cause it to overwrite the whole deployment directory.

```yaml
        volumeMounts: 
        - name: connection-strings
          mountPath: "/inetpub/wwwroot/ContosoUniversity/secret"
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

## Cloud SQL Issues?
It appears that Cloud SQL Private IP can only peer to a single network? If you have multiple networks in your project you will only be able to peer to a single network.
