Describe "New-AcuPackage" {
    BeforeAll {
        $packageFolderPath = Join-Path $PSScriptRoot ".\TestResources\TestPackage"
        $outputFile = "TestPackage.zip"
    }

    AfterAll {
        Remove-Item $outputFile -ErrorAction SilentlyContinue
    }

    Context "Build Package From folder" {
        It "Should build the package successfully" {
            { New-AcuPackage -cp $packageFolderPath -pfn $outputFile -v "20.200" -d "Built" -l 1 } | Should -Not -Throw
        }

        It "Should produce an output file" {
            $outputFile | Should -Exist
        }
    }
}
