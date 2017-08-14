# PcfProvider: PowerShell Provider for Pivotal Cloud Foundry

This project provides a PowerShell provider that supports hierarchical access to Pivotal Cloud Foundry artifacts. 

## Installing for usage
### 64-bit Windows
There is a setup project that supports 64-bit Windows. Follow these steps.
1. Build the solution.
2. Execute this command in the output folder (\bin\Debug) of the PcfProviderSetup64 project.
```powershell
msiexec /i PcfProviderSetup.msi /l* PcfProviderSetup.log
```
3. Issue the following PowerShell commands.
* This assumes you want to access the Pivotal Web Service.
* The files 'user.txt' and 'password.txt' need to be created with contents being the user name and password you use to access this instance of PCF.
* All paths should be adjusted for your usage.
    * 'user.txt' and 'password.txt' will need a path prefix if they are not in the current folder.
* The **-Verbose** switch can be removed with no impact.
```powershell
Import-Module PcfProvider -Verbose
New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Get-Content 'user.txt') -Password (Get-Content 'password.txt')
```
### 32-bit Windows
Build the solution and issue the following PowerShell commands.
* This assumes you want to access the Pivotal Web Service.
* The files 'user.txt' and 'password.txt' need to be created with contents being the user name and password you use to access this instance of PCF.
* All paths should be adjusted for your usage.
    * '.' in '.\PcfProvider' is replaced by the path to the output of the PcfProvider project.
    * 'user.txt' and 'password.txt' will need a path prefix if they are not in the current folder.
* The **-Verbose** switch can be removed with no impact.
```powershell
Import-Module -Name .\PcfProvider -Verbose
New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Get-Content 'user.txt') -Password (Get-Content 'password.txt')
```
## Drive Structure
This provider uses a folder structure to map Pivotal Cloud Foundry artifacts into PowerShell objects.
* Level 1 is the category.
* Level 2 is an entity under the category. All categories have level 2 items.
* Level 3 is the set of subcategories for an entity. The supported folders depend on the category.
* Level 4 is an entity for a subcategory.

### Categories and Subcategories
* apps
    * Application Entity
        * serviceBindings
* routes
    * Route Entity
* organizations
    * Organization Entity
        * domains
        * managers
        * users
* services
    * Service Entity
		* plans

## CF Equivalents
Some of the CF functions that retrieve PCF information can be emulated using this provider. These are still being augmented.

PowerShell aliases can be used for the commands.

```powershell
Write-Host "Replacement for 'cf m'"
Get-Item .\services\*\plans\* | Format-Table
```
```powershell
Write-Host "Replacement for 'cf apps' except url"
Get-ChildItems .\apps | Format-Table
```
```powershell

Write-Host "Replacement for 'cf domains' (partial)"
Get-ChildItem .\organizations\*\domains\* | Format-Table -GroupBy PSParentPath Name
```

## License
[This is an open source project using the MIT license.](docs/LICENSE.md)
