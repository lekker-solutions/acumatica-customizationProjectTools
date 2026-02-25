Describe "AcuPackage API Tests (24.2)" -Tag 'Integration' {
    BeforeAll {
        $url = "https://localhost/PackageToolsTest_24_2"
        $securePassword = ConvertTo-SecureString "123456" -AsPlainText -Force
        $credential = New-Object System.Management.Automation.PSCredential("admin", $securePassword)
        $packageName = "TestPackage"
        $packagePath = Join-Path $PSScriptRoot ".\TestResources\TestPackage.zip"
    }

    Context "Upload Package"{
        It "Should import the package successfully"{
            Import-AcuPackage -Credential $credential -Url $url -pn $packageName -pp $packagePath -r
        }
    }

    Context "Publish Package"{
        It "Should publish the package successfully"{
            Publish-AcuPackage -Credential $credential -Url $url -pn $packageName -MergeWithExisting -tm Current
        }
    }

    Context "Unpublish All"{
        It "Should unpublish all packages successfully"{
            Unpublish-AcuPackages -Credential $credential -Url $url -tm Current -Confirm:$false
        }
    }
}
