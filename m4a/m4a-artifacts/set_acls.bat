


icacls "c:\inetpub\wwwroot" /grant "NT AUTHORITY\IUSR":(OI)(CI)(RX)
icacls "c:\inetpub\wwwroot" /grant "BUILTIN\IIS_IUSRS":(OI)(CI)(RX)
icacls "c:\inetpub\wwwroot" /grant "IIS APPPOOL\DefaultAppPool":(OI)(CI)(RX)
icacls "c:\inetpub\wwwroot" /grant "BUILTIN\IIS_IUSRS":(RX)
icacls "c:\inetpub\wwwroot" /grant "BUILTIN\IIS_IUSRS":(S)
icacls "c:\inetpub\wwwroot" /grant "BUILTIN\IIS_IUSRS":(CI)(OI)(S)
icacls "c:\inetpub\wwwroot\contosouniversity" /grant "NT AUTHORITY\IUSR":(CI)(OI)(R)
icacls "c:\inetpub\wwwroot\contosouniversity" /grant "NT AUTHORITY\IUSR":(CI)(OI)(S)



icacls "c:\inetpub\wwwroot\contosouniversity" /grant "NT AUTHORITY\IUSR":(OI)(CI)(RX)
icacls "c:\inetpub\wwwroot\contosouniversity" /grant "BUILTIN\IIS_IUSRS":(OI)(CI)(RX)
icacls "c:\inetpub\wwwroot\contosouniversity" /grant "IIS APPPOOL\ContosoUniversity":(OI)(CI)(RX)
icacls "c:\inetpub\wwwroot\contosouniversity" /grant "NT AUTHORITY\IUSR":(CI)(OI)(R)
icacls "c:\inetpub\wwwroot\contosouniversity" /grant "NT AUTHORITY\IUSR":(CI)(OI)(S)
