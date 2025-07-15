$path = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Definition)
$name = "PackageToolsTest"
function InstallSite([string] $version,
                    [switch]$portal){
    $versionString = $version.Replace(".", "_").Substring(0,4);
    $installDir = "$($path)\$($name)_$($versionString)";
    if($portal){
        $installDir += "Portal"
    }
    $siteName = "$($name)_$($versionString)";
    
    if (Test-Path (Join-Path $installDir "web.config")) {
        Write-Output "Site Already Exists"
        return;
    }

    Write-Output "Installing $($version) at $($installDir)"

    if($portal){
        Add-AcuSite -n $siteName -v $version -p $installDir -pt
    }
    else{
        Add-AcuSite -n $siteName -v $version -p $installDir
    }

}

InstallSite("24.117.0035")
InstallSite("24.210.0019")

# SQL command to update admin password
$sqlCommand = @"
UPDATE Users 
SET Password = '123456' 
WHERE Username = 'admin'
"@

# Define the server and databases
$sqlServer = "localhost" # Change this to your SQL server name
$databases = @("PackageToolsTest_24_1", "PackageToolsTest_24_2")

# Check if SqlServer module is installed, install if not
if (-not (Get-Module -ListAvailable -Name SqlServer)) {
    Write-Host "SqlServer module not found. Installing..."
    Install-Module -Name SqlServer -Scope CurrentUser -Force
}

# Import the module
Import-Module SqlServer

# Execute SQL command on each database
foreach ($database in $databases) {
    try {
        Write-Host "Updating admin password in database: $database"
        Invoke-Sqlcmd -ServerInstance $sqlServer -Database $database -Query $sqlCommand -ErrorAction Stop
        Write-Host "Password updated successfully in $database" -ForegroundColor Green
    }
    catch {
        Write-Host "Error updating password in $database $_" -ForegroundColor Red
    }
}

Write-Host "Password update operation completed." -ForegroundColor Cyan