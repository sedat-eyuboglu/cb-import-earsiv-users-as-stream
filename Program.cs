using System.Diagnostics;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using Couchbase;

var zipFile = args[0];
var cbConnection = args[1];
var cbBucket = args[2];
var cbUser = args[3];
var cbPsw = args[4];
var index = 0;
var consoleWidth = Console.LargestWindowWidth;


var cluster = await Cluster.ConnectAsync(cbConnection, cbUser, cbPsw);
var collection = (await cluster.BucketAsync(cbBucket)).DefaultCollection();

using (var zipArchive = new ZipArchive(File.OpenRead(zipFile)))
{
    using(var xmlStream = zipArchive.Entries.First().Open())
    {
        using(var xReader = XmlReader.Create(xmlStream))
        {
            var sw = new Stopwatch();
            sw.Start();
            while(xReader.Read())
            {
                if (xReader.NodeType != XmlNodeType.Element || xReader.Name.Equals("User") == false)
                {
                    continue;
                }
                var userXml = XElement.ReadFrom(xReader) as XElement;
                if(userXml is not null)
                {
                    var user = new EArsivUser();
                    user.entity_type = nameof(EArsivUser);
                    user.Identifier = userXml.Element(nameof(user.Identifier))?.Value ?? string.Empty;
                    user.Title = userXml.Element(nameof(user.Title))?.Value ?? string.Empty;
                    user.Type = userXml.Element(nameof(user.Type))?.Value ?? string.Empty;
                    user.FirstCreationTime = userXml.Element(nameof(user.FirstCreationTime))?.Value ?? string.Empty;
                    user.AccountType = userXml.Element(nameof(user.AccountType))?.Value ?? string.Empty;
                    var userKey = $"{nameof(EArsivUser)}:{user.Identifier}";
                    var existsResult = await collection.ExistsAsync(userKey);
                    await collection.UpsertAsync(userKey, user);
                    Console.WriteLine($"{(++index).ToString().PadRight(15)}:{user.Title}");
                }
            }
            sw.Stop();
            Console.WriteLine($"Time elapsed: {sw.Elapsed.TotalSeconds}");
        }
    }
}
Console.WriteLine("End");

class EArsivUser
{
    public string Identifier { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string FirstCreationTime { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string entity_type { get; set; } = string.Empty;
}