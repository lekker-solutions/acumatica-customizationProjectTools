Describe "New-AcuPackage"{
    BeforeAll {
        $packageFolderPath = Join-Path $PSScriptRoot ".\TestResources\TestPackage"
    }

    Context "Build Package From folder"{
        It "Should build the package succesfully"{
            New-AcuPackage -cp $packageFolderPath -pfn TestPackage.zip -v "20.200" -d "Built" -l 1
        }
    }
}
