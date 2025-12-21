Describe "AcuPackage API Tests (24.1)"{
    BeforeAll {
        $url = "https://localhost/PackageToolsTest_24_1"
        $securePassword = ConvertTo-SecureString "123456" -AsPlainText -Force
        $credential = New-Object System.Management.Automation.PSCredential("admin", $securePassword)
        $packageName = "TestPackage"
        $packagePath = Join-Path $PSScriptRoot ".\TestResources\TestPackage.zip"
    }

    Context "Upload Package"{
        It "Should import the package succesfully"{
            Import-AcuPackage -Credential $credential -Url $url -pn $packageName -pp $packagePath -r
        }
    }

    Context "Publish Package"{
        It "Should publish the package succesfully"{
            Publish-AcuPackage -Credential $credential -Url $url -pn $packageName -MergeWithExisting -tm Current
        }
    }

    Context "Unpublish All"{
        It "Should unpublish all packages succesfully"{
            Unpublish-AcuPackages -Credential $credential -Url $url -tm Current -Confirm:$false
        }
    }
}
