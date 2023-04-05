using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO.Compression;
using System.ServiceModel;
using System.Xml;

namespace Velixo.Common.CustomizationPackageTools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(Environment.CommandLine);

            var rootCommand = new RootCommand();
            var buildCommand = new Command("build")
            {
                new Option<string>("--customizationpath", "The folder containing the customization source code (_project folder).") { IsRequired = true},
                new Option<string>("--packagefilename", "The name of the customization package.") { IsRequired = true},
                new Option<string>("--description", "The description of the customization project.") { IsRequired = true},
                new Option<int>("--level", "The number representing the level that is used to resolve conflicts that arise if multiple modifications of the same items of the website are merged. Defaults to 0."),
            };
            rootCommand.Add(buildCommand);

            var publishCommand = new Command("publish")
            {
                new Option<string>("--packagefilename", "The name of the customization package file.") { IsRequired = true},
                new Option<string>("--packagename", "The name of the customization.") { IsRequired = true},
                new Option<string>("--url", "The root URL of the Acumatica website where the customization should be published.") { IsRequired = true},
                new Option<string>("--username", "The username to connect.") { IsRequired = true},
                new Option<string>("--password", "The password to conect.") { IsRequired = true},

            };
            rootCommand.Add(publishCommand);

            buildCommand.Handler = CommandHandler.Create((string customizationPath, string packageFilename, string description, int level) =>
            {
                Console.WriteLine($"Generating customization package {packageFilename}...");
                PackageBuilder.BuildCustomizationPackage(customizationPath, packageFilename, description, level, "20.202");
                Console.WriteLine("Done!");
            });

            publishCommand.Handler = CommandHandler.Create(async (string packageFilename, string packageName, string url, string username, string password) => 
            {
                Console.WriteLine($"Publishing customization package {packageFilename} to {url}...");
                await PackagePublisher.PublishCustomizationPackage(packageFilename, packageName, url, username, password);
                Console.WriteLine("Done!");

            });

            await rootCommand.InvokeAsync(args);
        }



    }
}
