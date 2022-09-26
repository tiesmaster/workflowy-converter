using System.CommandLine;

using TextCopy;

namespace Tiesmaster.Workflowy.Converter.Cli;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var inputArgument = new Argument<FileInfo?>(
            name: "input",
            description: "The path to the Workflowy .backup file");

        var idOption = new Option<Guid?>(
            name: "--id",
            description: "The ID of the node to convert");

        var rootCommand = new RootCommand("Workflowy .backup JSON to OPML converter");
        rootCommand.AddArgument(inputArgument);
        rootCommand.AddOption(idOption);

        rootCommand.SetHandler(
            (inputFile, id) => ConvertJsonToOpmlAsync(inputFile!, id),
            inputArgument,
            idOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task ConvertJsonToOpmlAsync(FileInfo file, Guid? id)
    {
        var workflowyBackupFilename = file.FullName;
        var targetId = id ?? Guid.Empty;

        using var inputStream = File.OpenRead(workflowyBackupFilename);
        var rootNode = await WorkflowyNode.ReadFromAsync(inputStream);

        if (targetId != Guid.Empty)
        {
            rootNode = rootNode.GetNodeBydId(targetId);
        }

        var opml = rootNode.ToOpmlDocument().ToString();

        Console.WriteLine(opml);
        ClipboardService.SetText(opml);
    }
}