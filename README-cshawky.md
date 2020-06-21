# WinCertes - ACME Client for Windows

WinCertes is a simple ACMEv2 Client for Windows, able to manage the automatic issuance and renewal of SSL Certificates, for IIS or other web servers. It is based on [Certes](https://github.com/fszlin/certes) Library and [Certes](https://github.com/aloopkin/WinCertes). Pre-compiled binaries are available from GitHub (just look for the standard GitHub menu entry).

![GPLv3 License](https://www.gnu.org/graphics/gplv3-88x31.png)

- cshawky\WinCertes updates:
    - All registry details now stored under a subkey
    - Support for existing information in the root key (but prefer user moves the values into the named subkey)
    - Support for --extra as an alternate naming convention for compatibility with aloopkin/wincertes
    - Apologies but the options interface has been reworked such that read/write to the registry is separate to
      the main programme.  In doing so, two passes of the command line parameters was necessary to maintain full
      backward compatibility.
    - Registry key create rewritten and tested. Special registry permissions (reserved SIDs) are also inherited whilst removing user access.
    - More diagnostic debugging added
    - Lots of testing of --certname -d -x -a --reset --show --creatednskeys -l -e -f
    - Added built in support to create key, pem, combined. Reverted some to align with aloopkin. Creation of the full Certificate
      now has to be done in a Powershell script.
        - Powershell - Create full certificate PEM with private key (e.g. VisualSVN Server)
        - Certificate and key files now created (e.g. for use with hMailServer)
        - PFX for IIS unchanged, still need IIS certificate to revoke it (why have not researched)
    - Added support for password entry
    - Added attempt for Elevation if not run as administrator or UAC active
    - All certificate files stored in a folder = Environment.CurrentDirectory + "\\Certificates"
    - Assumes WinCertes.exe is installed to C:\Program Files\WinCertes or current working directory.
    - Tested on Windows 10, Windows 2016
    - TODO Get github community to confirm IIS, DNS registration features work
    - TODO .\Samples\scripts don't do anything yet. Update and simplify scripts.
    		For non IIS server with hMailServer or VisualSVN or another service, a wrapper script is required around wincertes.
    		I run a VPS without IIS and keep it fully locked down, no public access. Therefore firewall changes are required:
    		- Enable firewall for desired https authentication on required port to match DNS redirections if any.
    		- Start IIS if you wish to use IIS
    		- Run wincertes
    		- Post process new certificate if generated, like combine key and cert into single file for Visual SVN
    		- If new certificate provided, stop the relevant application server and update the certificate, start the service.
    		- stop IIS if you wish
    		- disable firewall rule if desired

Command Line Options
-------------

```dos
WinCertes.exe:
  -s, --service=VALUE        the ACME Service URI to be used (optional,
                               defaults to Let's Encrypt)
  -e, --email=VALUE          the account email to be used for ACME requests (
                               optional, defaults to no email)
  -d, --domain=VALUE         the domain(s) to enroll (mandatory)
  -w, --webserver[=ROOT]     toggles the local web server use and sets its ROOT
                               directory (default c:\inetpub\wwwroot).
                               Activates HTTP validation mode.
  -p, --periodic             should WinCertes create the Windows Scheduler task
                               to handle certificate renewal (default=no)
  -b, --bindname=VALUE       IIS site name to bind the certificate to, e.g. "
                               Default Web Site". Defaults to no binding.
  -f, --scriptfile=VALUE     PowerShell Script file e.g. "C:\Temp\script.ps1"
                               to execute upon successful enrollment (default=
                               none)
  -a, --standalone           should WinCertes create its own WebServer for
                               validation. Activates HTTP validation mode.
                               WARNING: it will use port 80 unless -l is
                               specified.
  -r, --revoke[=REASON]      should WinCertes revoke the certificate identified
                               by its domains (to be used only with -d). REASON
                               is an optional integer between 0 and 5.
  -k, --csp=VALUE            import the certificate into specified csp. By
                               default WinCertes imports in the default CSP.
  -t, --renewal=N            trigger certificate renewal N days before
                               expiration
  -l, --listenport=N         listen on port N in standalone mode (for use with -
                               a switch, default 80)
      --show                 show current configuration parameters
      --reset                reset all configuration parameters
      --extra[=VALUE]        manages additional certificate(s) instead of the
                               default one, with its own settings. Add an
                               integer index optionally to manage more certs.
      --no-csp               does not import the certificate into CSP. Use with
                               caution, at your own risks

cshawky/wincertes:
  -n, --certname=VALUE       Unique Certificate name, also used as cert file name (exclude extension)
                               e.g. "wincertes.com" (default name=first domain name)
      --dnscreatekeys        Create all DNS values in the registry and exit. helper for registry:
                               Use with --certname. Manually edit registry or include parameters below on the command line
      --dnstype=VALUE        DNS Validator type: acme-dns, win-dns
      --dnsurl=VALUE         DNS Server URL: http://blah.net
      --dnshost=VALUE        DNS Server Host
      --dnsuser=VALUE        DNS Server Username
      --dnspassword=VALUE    DNS Server Password
      --dnskey=VALUE         DNS Server Account Key
      --dnssubdomain=VALUE   DNS Server SubDomain
      --dnszone=VALUE        DNS Server Zone
      --debug                Enable extra debug logging
      --extra[=VALUE]        Please use -n instead for support of future file instead of registry
      --password=VALUE       Certificate password min 16 characters (default=random)
      --reset                Reset all configuration parameters for --certname and exit

Typical usage:
	The following two commands are equivalent
	  "WinCertes.exe -a -e me@example.com -d test1.example.com -d test2.example.com -p"
  	"WinCertes.exe -a -e me@example.com -d test1.example.com -d test2.example.com -p -n test1.example.com"
	  "WinCertes.exe -a -e me@example.com -d test1.example.com -d test2.example.com -p --extra=2"
  	"WinCertes.exe -a -e me@example.com -d test1.example.com -d test2.example.com -p -n extra2"

This will create a certificate with certname=test1.example.com and store all information 
for this certificate in the WinCertes registry under a sub key with the same name.

Once the registry key exists, subsequent WinCertes.exe commands may be executed using -n or --certname
and previously used parameters, domains etc will be pulled from registry

  "WinCertes.exe -n test1.example.com" will renew that certificate (if created without a certname or with certname as specificed).
  "WinCertes.exe --extra=2 will renew the 'extra2' certificate (if created with the --extra=2 parameter).
  "WinCertes.exe -n test1.example.com -r" will revoke that certificate.

Be sure to revoke a certificate before deleting registry keys via --reset

  "WinCertes.exe -n test1.example.com --reset" will revoke that certificate.


```

