Describe "Invoke-ServicePackage"{
    BeforeAll {
        $url = "https://localhost/23R1"
        $username = "admin"
        $password = "123456"
        $packageName = "TestPackage"
        $packagePath = Join-Path $PSScriptRoot ".\TestResources\TestPackage.zip"
    }

    Context "Upload Package"{
        It "Should import the package succesfully"{
            {Invoke-ServicePackageUpload -u $username -p $password -url $url -pn $packageName -pp $packagePath -r } | Should -Not -Throw
        } 
    }

    Context "Publish Package"{
        It "Should publish the package succesfully"{
            {Invoke-ServicePackagePublish -u $username -p $password -url $url -pn $packageName -m} | Should -Not -Throw
        } 
    }

    Context "Unpublish All"{
        It "Should unpublish all packages succesfully"{
            {Invoke-ServicePackageUnpublishAll -u $username -p $password -url $url} | Should -Not -Throw
        } 
    }
}

