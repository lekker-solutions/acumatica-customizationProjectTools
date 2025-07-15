# <b>acumatica-customizationProjectTools</b>
A Powershell 7 Module for Building, Uploading and Publishing Customization Packages to Acumatica ERP. Uses code for building customization packages from the CustomizationProjectTools command line application from Gabriel Michauds excellent [Acumatica CI/CD Demo](https://github.com/VelixoSolutions/AcumaticaCIDemo)

## <u><b>Package Build</b></u>

### <u>Invoke-PackageBuild</u>
Wraps a directory saved from an Acumatica ERP instance into a zip that can be uploaded into an instance. Does not spin up an entire app domain like PX.Commandline.exe does, much faster! Does not need an existing Acumatica site!
- Parameters
    - CustomizationPath (-cp) [string]
        - The path to the customization directory that was saved using the source control drop down
    - Package File Name (-pfn)[string]
        - The output path and file name of the generated .zip folder
    - Product Version (-v) [string]
        - The Acumatica Site version in format ##.### or 22.202 for a 22R2 instance build 2
    - Description (-d) [string]
        - Appends a description to the package to be displayed in the customization project list
    - Level (-l) [int]
        - Adds a level to the package so that it can be published in the right order
- Example
   - Invoke-PackageBuild -cp $packageFolderPath -pfn TestPackage.zip -v "20.200" -d "Built" -l 1
    
## <u><b>Package Management</b></u>
Use Invoke-Api... for using the REST API introduced in 22R2, or Invoke-Service... to use the ServiceGate endpoint.


### <u>Invoke-ApiPackageUpload</u>

Upload a customization package to an Acumatica ERP Instance. 
- Parameters
    - URL [string]
        - The URL Of the Acumatica Instance
    - Username (-u) [string]
        - The username to authenticate with
    - Password (-p) [string]
        - The password to authenticate with
    - Tenant (-t) [string]
        - The tenant to authenticate to
    - PackgeName (-pn) [string]
        - The name of the package as it is saved in Acuamtica ERP
    - PackagePath (-pp) [string]
        - The path to the .zip folder to be uploaded
    - ReplacePackage (-r) [switch]
        - If this is not set and the package exists, it will throw an error
    - Description (-d) [string]
        - Appends a description to the package to be displayed in the customization project list
    - Level (-l) [int]
        - Adds a level to the package so that it can be published in the right order
- Example
   - Invoke-ApiPackageUpload -u $username -p $password -url $url -pn $packageName -pp $packagePath -r

### <u>Invoke-ApiPackagePublish</u>
Publish a number of uploaded packages to an Acuamtica ERP Instance
- Parameters
    - URL
        - The URL Of the Acumatica Instance
    - Username (-u) [string]
        - The username to authenticate with
    - Password (-p) [string]
        - The password to authenticate with
    - Tenant (-t) [string]
        - The tenant to authenticate to
    - ProjectNames (-pn) [string]
        - The names of the projects that are to be published
    - TenantMode (-tm) {api only} [enum]
        - The tenant mode to use (Current,All,List)
    - MergeWithExisting {api only} [switch]
    - OnlyValidate {api only} [switch]
    - DbUpdateOnly {api only} [switch]
    - ExecuteAllScripts {api only}  [switch]
    - TenantLoginNames {api only} [string[]]
        - The tenant names to publish to if List is used as a tenant mode
- Example
   - Invoke-ServicePackagePublish -u $username -p $password -url $url -pn $packageName -m

### <u>Invoke-ApiPackageUnPublishAll</u>
Unpublish all packages from an Acumatica ERP Instance
- Parameters
    - URL
        - The URL Of the Acumatica Instance
    - Username (-u)
        - The username to authenticate with
    - Password (-p)
        - The password to authenticate with
    - Tenant (-t) [string]
        - The tenant to authenticate to
    - TenantMode (-tm) {api only} [enum]
        - The tenant mode to use (Current,All,List)
    - TenantLoginNames {api only} [string[]]
        - The tenant names to publish to if List is used as a tenant mode
- Example
   - Invoke-ApiPackageUnpublishAll -u $username -p $password -url $url -tm Current