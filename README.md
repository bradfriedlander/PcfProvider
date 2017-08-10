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

## License
[This is an open source project using the MIT license.](docs/LICENSE.md)
