# PcfProvider: PowerShell Provider for Pivotal Cloud Foundry

This project provides a PowerShell provider that supports hierarchical access to Pivotal Cloud Foundry artifacts. 

## Installing for usage
Issue the following PowerShell commands.
* This assumes you want to access the Pivotal Web Service.
* The files 'user.txt' and 'password.txt' need to be created with contents being the user name and password you use to access this instance of PCF.
* All paths should be adjusted for your usage.
```
Import-Module -Name .\PcfProvider.dll
New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Get-Content 'user.txt') -Password (Get-Content 'password.txt')
```
## License
[This is an open source project using the MIT license.](.\LICENSE.md)
