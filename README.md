# acumatica-customizationProjectTools
Powershell Cmdlets for Managing Acumatica customization packages. 

Extracted out of Gabriel Michauds [AcumaticaCIDemo](https://github.com/VelixoSolutions/AcumaticaCIDemo/) so that it may be used in a public Powershell Module. Many thanks for taking the time to investigate how to build a customization package without starting up the website.

# Cmdlets

## Invoke-PackageBuild
### Parameters:
 - CustomizationPath (-cp)
    - The input path of the customization, where all the customization files reside
 - PackageFileName (-pfn)
    - The output filename of the package, provide this value without an extension 
 - Description (-d)
    - The Description that is to be written to the package that is visible in the customization packages list view
 - Level (-l)
    - The level of the customization that is to be written to the output package

### Example
Invoke-PackageBuild -cp .\SomeCustomization -pfn SomeCustomization -d "Some Customization" -l 100
<br>
<br>
<br>

## Invoke-{Api/Service}PackageUpload
### Parameters:
 - PackageName (-pn)
    - The name of the package as it appears in the customization project list view
 - PackagePath (-pfn)
    - The path to the .zip folder of the package
 - Url (-url)
    - The URL of the Acumatica instance to publish to
 - Username (-u)
    - The username to use to authenticate to the Acumatica Site
- Password (-p)
    -  The password to use to authenticate to the Acumatica Site
- RestAPI (-api)
    -  Upload and Publish the Customization using the REST API instead of the ServiceGate
### Example
Invoke-ApiPackagePublish -pn SomeCustomization -pfn .\SomeCustomization.zip -url "https://somesite.acumatica.com" -u "admin" -p "setup"

## Invoke-{Api/Service}PackagePublish
### Parameters:
 - PackageName (-pn)
    - The name of the package as it appears in the customization project list view
 - PackageFileName (-pfn)
    - The path to the .zip folder of the package
 - Url (-url)
    - The URL of the Acumatica instance to publish to
 - Username (-u)
    - The username to use to authenticate to the Acumatica Site
- Password (-p)
    -  The password to use to authenticate to the Acumatica Site
- RestAPI (-api)
    -  Upload and Publish the Customization using the REST API instead of the ServiceGate
### Example
Invoke-ApiPackagePublish -pn SomeCustomization -pfn .\SomeCustomization.zip -url "https://somesite.acumatica.com" -u "admin" -p "setup"

