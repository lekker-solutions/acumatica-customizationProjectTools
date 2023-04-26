Describe "Invoke-PackageBuild"{
    BeforeAll {
        $modulePath = Join-Path $PSScriptRoot "..\AcuPackageTools\AcuPackageTools.psd1" 
        Import-Module -Name $modulePath
        $url = "https://localhost/23R1"
        $username = "admin"
        $password = "123456"
        $packageName = "TestPackage"
        $packageFolderPath = Join-Path $PSScriptRoot ".\TestResources\TestPackage"
    }

    Context "Build Package From folder"{
        It "Should build the package succesfully"{
            Invoke-PackageBuild -cp $packageFolderPath -pfn TestPackage.zip -v "20.200" -d "Built" -l 1
        }
    }
}

