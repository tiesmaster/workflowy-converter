using System.Text;
using System.Text.Json;
using System.Xml;

using TextCopy;

namespace Tiesmaster.Workflowy.Converter;

internal class Program
{
    private static void Main(string[] args)
    {
        var workflowyBackupFilename = args[0];
        var targetId = args.Length > 1 ? Guid.Parse(args[1]) : Guid.Empty;

        using var inputFile = File.OpenRead(workflowyBackupFilename);

        var rootNode = JsonSerializer.Deserialize<WorkflowyNode>(inputFile)!;

        if (targetId != Guid.Empty)
        {
            rootNode = rootNode.GetNodeBydId(targetId);
        }

        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);

        using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
        {
            Indent = true,
        });

        var root = new RootNode(rootNode);

        root.WriteTo(xmlWriter);

        xmlWriter.Flush();

        var s = Encoding.UTF8.GetString(ms.ToArray());

        ClipboardService.SetText(s);

        Console.WriteLine(s);
    }
}