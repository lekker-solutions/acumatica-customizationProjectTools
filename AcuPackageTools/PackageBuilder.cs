#region #Copyright

//  ----------------------------------------------------------------------------------
//   COPYRIGHT (c) 2023 CONTOU CONSULTING
//   ALL RIGHTS RESERVED
//   AUTHOR: Kyle Vanderstoep
//   CREATED DATE: 2023/04/05
// ----------------------------------------------------------------------------------

#endregion

using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace AcuPackageTools
{
    public class PackageBuilder
    {
        public static void BuildCustomizationPackage(string customizationPath,
                                                          string packageFilename,
                                                          string description,
                                                          int    level,
                                                          string productVersion)
        {
            // Our poor man's version of PX.CommandLine.exe -- to keep things simple.
            var projectXml        = new XmlDocument();
            var customizationNode = projectXml.CreateElement("Customization");

            customizationNode.SetAttribute("level", level.ToString());
            customizationNode.SetAttribute("description", description);
            customizationNode.SetAttribute("product-version", productVersion);

            // Append all .xml files to project.xml
            foreach (var file in Directory.GetFiles(Path.Combine(customizationPath, "_project"), "*.xml"))
            {
                if (file.EndsWith("ProjectMetadata.xml")) continue;

                Console.WriteLine($"Appending {Path.GetFileName(file)} to customization project.xml...");
                var currentFileXml = new XmlDocument();
                currentFileXml.Load(file);

                if (currentFileXml.DocumentElement == null) throw new Exception("project.xml empty");

                customizationNode.AppendChild(projectXml.ImportNode(currentFileXml.DocumentElement, true));
            }

            //Append other customization assets to zip file
            using (FileStream zipToOpen = new FileStream(packageFilename, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    //Append every other files directly, flattening the file name in the process
                    foreach (var directory in Directory.GetDirectories(customizationPath))
                    {
                        if (directory.EndsWith(@"\_project")) continue;
                        AddAssetsToPackage(archive, directory, customizationPath, customizationNode);
                    }

                    projectXml.AppendChild(customizationNode);
                    ZipArchiveEntry projectFile = archive.CreateEntry("project.xml", CompressionLevel.Optimal);
                    using (StreamWriter writer = new StreamWriter(projectFile.Open()))
                    {
                        projectXml.Save(writer);
                    }
                }
            }
        }

        private static void AddAssetsToPackage(ZipArchive archive, string currentDirectory, string rootDirectory,
                                               XmlElement customizationElement)
        {
            Console.WriteLine($"Processing directory {currentDirectory}...");

            foreach (var file in Directory.GetFiles(currentDirectory))
            {
                string targetZipFileName = file.Substring(rootDirectory.Length);
                Console.WriteLine($"Adding {targetZipFileName} to customization project...");

                archive.CreateEntryFromFile(file, targetZipFileName, CompressionLevel.Optimal);

                //Add reference to customization project as well
                var fileElement = customizationElement.OwnerDocument.CreateElement("File");
                fileElement.SetAttribute("AppRelativePath", targetZipFileName);
                customizationElement.AppendChild(fileElement);
            }

            foreach (var directory in Directory.GetDirectories(currentDirectory))
            {
                AddAssetsToPackage(archive, directory, rootDirectory, customizationElement);
            }
        }
    }
}