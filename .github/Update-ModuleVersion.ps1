# Update-ModuleVersion.ps1
# This script updates the ModuleVersion in the module manifest with the GitHub tag

param(
    [Parameter(Mandatory=$true)]
    [string]$GitHubTag,
    
    [Parameter(Mandatory=$false)]
    [string]$ManifestPath = ".\AcuPackageTools\AcuPackageTools.psd1"
)

# Ensure the manifest file exists
if (-not (Test-Path -Path $ManifestPath)) {
    throw "Module manifest not found at path: $ManifestPath"
}

# Extract version from tag (remove 'v' prefix if present)
$version = $GitHubTag -replace '^v', ''

# Validate version format (should be in the format x.y.z or x.y.z-suffix)
if (-not ($version -match '^\d+\.\d+\.\d+(-\w+)?$')) {
    throw "Invalid version format: $version. Expected format: x.y.z or x.y.z-suffix"
}

Write-Host "Updating module version to $version"

# Read the manifest content
$manifestContent = Get-Content -Path $ManifestPath -Raw

# Update the ModuleVersion
$updatedContent = $manifestContent -replace "ModuleVersion\s*=\s*['`"](.+?)['`"]", "ModuleVersion = '$version'"

# Check if a replacement was made
if ($updatedContent -eq $manifestContent) {
    Write-Warning "No changes were made to the manifest. ModuleVersion pattern might not match."
    exit 1
}

# Write the updated content back to the file
Set-Content -Path $ManifestPath -Value $updatedContent

# Verify the change
$newManifest = Import-PowerShellDataFile -Path $ManifestPath
Write-Host "Module version updated to: $($newManifest.ModuleVersion)" -ForegroundColor Green

# Check if there's a prerelease portion in the version
$prereleaseMatch = $version -match '-(.+)$'
if ($prereleaseMatch) {
    $prerelease = $matches[1]
    Write-Host "Detected prerelease suffix: $prerelease"
    
    # Update prerelease tag if it exists in the manifest
    $updatedContent = $updatedContent -replace "(Prerelease\s*=\s*)(['`"].+?['`"]|)", "`$1'$prerelease'"
    
    # Write the updated content back to the file
    Set-Content -Path $ManifestPath -Value $updatedContent
    
    # Verify the prerelease change
    $newManifest = Import-PowerShellDataFile -Path $ManifestPath
    if ($newManifest.PrivateData.PSData.Prerelease -eq $prerelease) {
        Write-Host "Prerelease tag updated to: $prerelease" -ForegroundColor Green
    }
}
