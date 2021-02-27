using JMW.Extensions.Text;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace JMW.Google.OnHub.Console
{
    using CommandLine;
    using System;

    public class Options
    {
        [Option('t', "target", Default = "192.168.85.1",
            HelpText = "The IP Address of the Google Wifi Router")]
        public string Target { get; set; }

        [Option('j', "json", Default = false,
            HelpText = "Outputs the results as JSON.")]
        public bool AsJson { get; set; }

        [Option('c', "category", Default = Categories.all,
            HelpText = "Specific type of data to return: arp, cam, interface, all")]
        public Categories Category { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            IPAddress target = null;
            Options opts = null;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    if (o.Target == null || !IPAddress.TryParse(o.Target, out target))
                    {
                        Console.WriteLine(
                            "A collection target is required.  Please pass the IPv4 address of your Google Wifi Router using one of the following:");
                        Console.WriteLine(" -t 192.168.84.1");
                        Console.WriteLine(" --target 192.168.84.1");
                        return;
                    }

                    opts = o;
                })
                ;

            if (opts == null)
            {
                return;
            }

            var data = await OnHubApi.GetData(target);
            if (opts.AsJson)
            {
                var jsonOpts = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                };

                switch (opts.Category)
                {
                    case Categories.all:
                        Console.Write(JsonSerializer.Serialize(data, jsonOpts));
                        break;

                    case Categories.arp:
                        Console.Write(JsonSerializer.Serialize(data.ArpCache, jsonOpts));
                        break;

                    case Categories.cam:
                        Console.Write(JsonSerializer.Serialize(data.CamTable, jsonOpts));
                        break;

                    case Categories.ifc:
                        Console.Write(JsonSerializer.Serialize(data.Interfaces, jsonOpts));
                        break;
                }
                return;
            }

            switch (opts.Category)
            {
                case Categories.all:
                    data.ArpCache.WriteAsTable(Console.Write);
                    data.CamTable.WriteAsTable(Console.Write);
                    data.Interfaces.WriteAsTable(Console.Write);
                    break;

                case Categories.arp:
                    data.ArpCache.WriteAsTable(Console.Write);
                    break;

                case Categories.cam:
                    data.CamTable.WriteAsTable(Console.Write);
                    break;

                case Categories.ifc:
                    data.Interfaces.WriteAsTable(Console.Write);
                    break;
            }
        }
    }

    public enum Categories
    {
        all,
        arp,
        cam,
        ifc,
    }
}