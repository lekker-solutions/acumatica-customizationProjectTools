Describe "Invoke-ApiPackage"{
    BeforeAll {
        $modulePath = Join-Path $PSScriptRoot "..\AcuPackageTools\AcuPackageTools.psd1" 
        Import-Module -Name $modulePath
        $url = "https://localhost/23R1"
        $username = "admin"
        $password = "123456"
        $packageName = "TestPackage"
        $packagePath = Join-Path $PSScriptRoot ".\TestResources\TestPackage.zip"
    }

    Context "Upload Package"{
        It "Should import the package succesfully"{
            Invoke-ServicePackageUpload -u $username -p $password -url $url -pn $packageName -pp $packagePath
        }
    }

    Context "Publish Package"{
        It "Should publish the package succesfully"{
            Invoke-ServicePackageUpload -u $username -p $password -url $url -pn $packageName -pp $packagePath
        }
    }

    Context "Unpublish All"{
        It "Should unpublish all packages succesfully"{
            Invoke-ServicePackageUpload -u $username -p $password -url $url -pn $packageName -pp $packagePath
        }
    }
}

