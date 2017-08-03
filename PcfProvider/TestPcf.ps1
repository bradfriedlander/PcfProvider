Import-Module -Name .\PcfProvider.dll
Get-PSProvider
echo "Create PCF drive."
#New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Get-Content 'user.txt') -Password (Read-Host "Password")
New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -Uri run.pivotal.io -UserName (Get-Content 'user.txt') -Password (Get-Content 'password.txt')
#New-PSDrive PCF -PSProvider 'Pcf' -Root '\' -IsLocal
echo "dir PCF:"
dir PCF:
echo "dir PCF:\apps"
dir PCF:\apps
echo "cd PCF:; dir"
cd PCF:
dir
#echo "cd .\apps; dir"
#cd .\apps
#dir
echo "gci | ft Name,MemoryKb,DiskKb,Instances"
gci .\apps | ft Name,MemoryKb,DiskKb,Instances
echo "gci apps | %{echo $_.ServiceBindings.ServiceInstance}"
gci apps | %{echo $_.ServiceBindings.ServiceInstance}
#echo "cd ..\services; dir"
#cd ..\services
#dir
echo "gci | ft Name,Active,UniqueId,Description"
gci .\services |ft Name,Active,UniqueId,Description
