Describe "Invoke-PackageBuild"{
    BeforeAll {
        $modulePath = Join-Path $PSScriptRoot "..\AcuPackageTools\AcuPackageTools.psd1" 
        if(!(Get-Module -ListAvailable -Name $modulePath -ErrorAction SilentlyContinue))
        {
            Import-Module -Name $modulePath
        }

        $packageFolderPath = Join-Path $PSScriptRoot ".\TestResources\TestPackage"
    }

    Context "Build Package From folder"{
        It "Should build the package succesfully"{
            Invoke-PackageBuild -cp $packageFolderPath -pfn TestPackage.zip -v "20.200" -d "Built" -l 1
        }
    }
}

