using TextCopy;

namespace Tiesmaster.Workflowy.Converter;

internal static class Program
{
    private static void Main(string[] args)
    {
        var workflowyBackupFilename = args[0];
        var targetId = args.Length > 1 ? Guid.Parse(args[1]) : Guid.Empty;

        using var inputStream = File.OpenRead(workflowyBackupFilename);
        var rootNode = WorkflowyNode.ReadFrom(inputStream);

        if (targetId != Guid.Empty)
        {
            rootNode = rootNode.GetNodeBydId(targetId);
        }

        var opml = rootNode.ToOpmlDocument().ToString();

        Console.WriteLine(opml);
        ClipboardService.SetText(opml);
    }
}