Describe "Invoke-ApiPackageUpload"{
    BeforeEach {
        $modulePath = Join-Path $PSScriptRoot "..\AcuPackageTools\AcuPackageTools.psd1" 
        Import-Module -Name $modulePath
        $url = "http://localhost/23R1"
        $username = "admin"
        $password = "123456"
    }

    Context "Upload Package Thru Api"{
        It "Should import the package succesfully"{
            $packageName = "TestPackage"
            $packagePath = ".\TestResources\TestPackage.zip"
    
    
            Invoke-ApiPackageUpload -u $username -p $password -url $url -pn $packageName -pp $packagePath -d "Test"
        }
    }
}

