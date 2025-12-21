# acumatica-customizationProjectTools

A PowerShell Module for Building, Uploading and Publishing Customization Packages to Acumatica ERP.

Uses code for building customization packages from the CustomizationProjectTools command line application from Gabriel Michaud's excellent [Acumatica CI/CD Demo](https://github.com/VelixoSolutions/AcumaticaCIDemo).

## Installation

```powershell
Install-Module -Name AcuPackageTools
```

## Cmdlet Overview

| Cmdlet | Description |
|--------|-------------|
| `New-AcuPackage` | Build a customization package ZIP from a directory |
| `Import-AcuPackage` | Upload a package to an Acumatica instance |
| `Publish-AcuPackage` | Publish uploaded packages to an instance |
| `Unpublish-AcuPackages` | Unpublish all packages from an instance |
| `Remove-AcuPackage` | Delete an unpublished package from an instance |

Discover all cmdlets:
```powershell
Get-Command -Module AcuPackageTools
# or
Get-Command -Noun Acu*
```

---

## Authentication

All API cmdlets use `PSCredential` for secure authentication:

**Interactive:**
```powershell
$cred = Get-Credential -UserName "admin" -Message "Enter Acumatica credentials"
```

**Scripted/Automation:**
```powershell
$securePassword = ConvertTo-SecureString "YourPassword" -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential("admin", $securePassword)
```

**Secure Storage (for CI/CD):**
```powershell
# Save credentials (encrypted to current user)
Get-Credential | Export-Clixml -Path ".\acumatica-cred.xml"

# Load credentials
$cred = Import-Clixml -Path ".\acumatica-cred.xml"
```

---

## Package Build

### New-AcuPackage

Wraps a directory saved from an Acumatica ERP instance into a ZIP that can be uploaded. Does not spin up an entire app domain like PX.Commandline.exe - much faster and does not require an existing Acumatica site.

**Parameters:**

| Parameter | Alias | Type | Required | Description |
|-----------|-------|------|----------|-------------|
| CustomizationPath | -cp | string | Yes | Path to the customization directory saved via source control |
| PackageFileName | -pfn | string | Yes | Output path and filename for the generated .zip |
| ProductVersion | -v | string | Yes | Acumatica version in format `##.###` (e.g., `24.200`) |
| Description | -d | string | No | Description displayed in customization project list |
| Level | -l | int | No | Package level for publish order |

**Example:**
```powershell
New-AcuPackage -CustomizationPath ".\MyCustomization" `
               -PackageFileName ".\Output\MyPackage.zip" `
               -ProductVersion "24.200" `
               -Description "My customization package" `
               -Level 1
```

---

## Package Management (REST API)

These cmdlets use the Customization REST API introduced in Acumatica 22R2.

### Import-AcuPackage

Upload a customization package to an Acumatica ERP instance.

**Parameters:**

| Parameter | Alias | Type | Required | Description |
|-----------|-------|------|----------|-------------|
| Url | | string | Yes | URL of the Acumatica instance |
| Credential | | PSCredential | Yes | Credentials for authentication |
| Tenant | -t | string | No | Tenant to authenticate to |
| PackageName | -pn | string | Yes | Name of the package in Acumatica |
| PackagePath | -pp | string | Yes | Path to the .zip file to upload |
| ReplacePackage | -r | switch | No | Replace if package exists (otherwise throws error) |
| PackageDescr | -d | string | No | Description for the package |
| Level | -lvl | int | No | Package level for publish order |

**Example:**
```powershell
$cred = Get-Credential
Import-AcuPackage -Url "https://acumatica.example.com" `
                  -Credential $cred `
                  -PackageName "MyPackage" `
                  -PackagePath ".\Output\MyPackage.zip" `
                  -ReplacePackage
```

---

### Publish-AcuPackage

Publish uploaded packages to an Acumatica ERP instance. Supports `-WhatIf` and `-Confirm`.

**Parameters:**

| Parameter | Alias | Type | Required | Description |
|-----------|-------|------|----------|-------------|
| Url | | string | Yes | URL of the Acumatica instance |
| Credential | | PSCredential | Yes | Credentials for authentication |
| Tenant | -t | string | No | Tenant to authenticate to |
| ProjectNames | -pn | string[] | Yes | Names of projects to publish |
| TenantMode | -tm | enum | Yes | `Current`, `All`, or `List` |
| MergeWithExisting | | switch | No | Merge with existing customization |
| OnlyValidate | | switch | No | Validate without publishing |
| DbUpdateOnly | | switch | No | Only update database |
| ExecuteAllScripts | | switch | No | Re-execute all scripts |
| TenantLoginNames | -tln | string[] | No | Tenant names when using `List` mode |

**Example:**
```powershell
$cred = Get-Credential
Publish-AcuPackage -Url "https://acumatica.example.com" `
                   -Credential $cred `
                   -ProjectNames @("MyPackage", "AnotherPackage") `
                   -TenantMode Current `
                   -MergeWithExisting

# Preview what would happen
Publish-AcuPackage -Url "https://acumatica.example.com" `
                   -Credential $cred `
                   -ProjectNames @("MyPackage") `
                   -TenantMode Current `
                   -WhatIf
```

---

### Unpublish-AcuPackages

Unpublish all packages from an Acumatica ERP instance. Supports `-WhatIf` and `-Confirm` (high impact - will prompt by default).

> **Note:** The Acumatica API only supports unpublishing ALL packages. You cannot unpublish individual packages.

**Parameters:**

| Parameter | Alias | Type | Required | Description |
|-----------|-------|------|----------|-------------|
| Url | | string | Yes | URL of the Acumatica instance |
| Credential | | PSCredential | Yes | Credentials for authentication |
| Tenant | -t | string | No | Tenant to authenticate to |
| TenantMode | -tm | enum | Yes | `Current`, `All`, or `List` |
| TenantLoginNames | -tln | string[] | No | Tenant names when using `List` mode |

**Example:**
```powershell
$cred = Get-Credential

# Will prompt for confirmation due to high impact
Unpublish-AcuPackages -Url "https://acumatica.example.com" `
                      -Credential $cred `
                      -TenantMode Current

# Skip confirmation (use with caution)
Unpublish-AcuPackages -Url "https://acumatica.example.com" `
                      -Credential $cred `
                      -TenantMode Current `
                      -Confirm:$false
```

---

### Remove-AcuPackage

Delete an unpublished customization project from an Acumatica ERP instance. Supports `-WhatIf` and `-Confirm` (high impact - will prompt by default).

> **Note:** You can only delete packages that are **not currently published**. The API deletes project data and project items from the database, but does not delete files/objects added by the project (site map nodes, reports, etc.).

**Parameters:**

| Parameter | Alias | Type | Required | Description |
|-----------|-------|------|----------|-------------|
| Url | | string | Yes | URL of the Acumatica instance |
| Credential | | PSCredential | Yes | Credentials for authentication |
| Tenant | -t | string | No | Tenant to authenticate to |
| ProjectName | -pn | string | Yes | Name of the project to delete |

**Example:**
```powershell
$cred = Get-Credential

# Will prompt for confirmation due to high impact
Remove-AcuPackage -Url "https://acumatica.example.com" `
                  -Credential $cred `
                  -ProjectName "MyOldPackage"

# Preview what would happen
Remove-AcuPackage -Url "https://acumatica.example.com" `
                  -Credential $cred `
                  -ProjectName "MyOldPackage" `
                  -WhatIf
```

---

## Complete CI/CD Example

```powershell
# Configuration
$acumaticaUrl = "https://acumatica.example.com"
$packageName = "MyCustomization"
$customizationPath = ".\src\MyCustomization"
$outputPath = ".\artifacts\$packageName.zip"
$acumaticaVersion = "24.200"

# Load saved credentials
$cred = Import-Clixml -Path ".\acumatica-cred.xml"

# Build the package
New-AcuPackage -CustomizationPath $customizationPath `
               -PackageFileName $outputPath `
               -ProductVersion $acumaticaVersion `
               -Description "Build $(Get-Date -Format 'yyyy-MM-dd HH:mm')" `
               -Level 1

# Upload to Acumatica
Import-AcuPackage -Url $acumaticaUrl `
                  -Credential $cred `
                  -PackageName $packageName `
                  -PackagePath $outputPath `
                  -ReplacePackage

# Publish
Publish-AcuPackage -Url $acumaticaUrl `
                   -Credential $cred `
                   -ProjectNames @($packageName) `
                   -TenantMode Current `
                   -MergeWithExisting
```

---

## Migration from v0.x

If upgrading from a previous version, note these breaking changes:

| Old Cmdlet | New Cmdlet |
|------------|------------|
| `Invoke-PackageBuild` | `New-AcuPackage` |
| `Invoke-ApiPackageUpload` | `Import-AcuPackage` |
| `Invoke-ApiPackagePublish` | `Publish-AcuPackage` |
| `Invoke-ApiPackageUnpublishAll` | `Unpublish-AcuPackages` |

**Authentication changed** from `-Username`/`-Password` to `-Credential`:

```powershell
# Old (no longer works)
Invoke-ApiPackageUpload -Username "admin" -Password "pass" ...

# New
$cred = Get-Credential
Import-AcuPackage -Credential $cred ...
```

---

## License

MIT License - see [LICENSE](LICENSE) for details.
