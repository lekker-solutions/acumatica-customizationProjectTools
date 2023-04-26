Describe "Invoke-ServicePackageUpload"{
    BeforeAll {
        $modulePath = Join-Path $PSScriptRoot "..\AcuPackageTools\AcuPackageTools.psd1" 
        Import-Module -Name $modulePath
        $url = "https://localhost/23R1"
        $username = "admin"
        $password = "123456"
        $packageName = "TestPackage"
        $packagePath = Join-Path $PSScriptRoot ".\TestResources\TestPackage.zip"
    }

    Context "Upload Package Thru ServiceGate"{
        It "Should import the package succesfully"{
            Invoke-ServicePackageUpload -u $username -p $password -url $url -pn $packageName -pp $packagePath
        }
    }
}

