Describe "Invoke-PackageBuild"{
    BeforeAll {
        $packageFolderPath = Join-Path $PSScriptRoot ".\TestResources\TestPackage"
    }

    Context "Build Package From folder"{
        It "Should build the package succesfully"{
            Invoke-PackageBuild -cp $packageFolderPath -pfn TestPackage.zip -v "20.200" -d "Built" -l 1
        }
    }
}

