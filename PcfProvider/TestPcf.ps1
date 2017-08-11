Import-Module -Name .\PcfProvider -Verbose
Get-PSProvider
echo "Create PCF drive."
#New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Read-Host "User Name") -Password (Read-Host "Password")
New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Get-Content 'user.txt') -Password (Get-Content 'password.txt') #-IsLogItems
#New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -IsLocal -IsLogItems
echo "dir PCF:"
dir PCF:
echo "dir PCF:\apps"
dir PCF:\apps
echo "cd PCF:; dir"
cd PCF:
dir
echo "gci .\apps | ft"
gci .\apps | ft
echo "gci .\apps | fl"
gci .\apps | fl
echo "gci .\apps | %{echo $_.ServiceBindings.ServiceInstance}"
gci .\apps | %{echo $_.ServiceBindings.ServiceInstance}
echo "gci .\services | ft Name,Active,InstanceId,Description"
gci .\services | ft Name,Active,InstanceId,Description
echo "gci .\organizations | ft Name,Status"
gci .\organizations | ft Name,Status
echo "gci .\services\*\plans\* | ft -GroupBy PSParentPath Name,Active,Free,Description"
gci .\services\*\plans\* | ft -GroupBy PSParentPath Name,Active,Free,Description
