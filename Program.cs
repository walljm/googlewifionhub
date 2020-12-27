using Diagnosticreport;
using JMW.Extensions.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace onhub
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private static Dictionary<string, string> hwTypes = new Dictionary<string, string>
        {
            {"0x0", "Reserved"},
            {"0x1", "Ethernet"},
            {"0x2", "Experimental Ethernet"},
            {"0x3", "Amateur Radio AX.25"},
            {"0x4", "Proteon ProNET Token Ring"},
            {"0x5", "Chaos"},
            {"0x6", "IEEE 802 Networks"},
            {"0x7", "ARCNET"},
            {"0x8", "Hyperchannel"},
            {"0x9", "Lanstar"},
            {"0x10", "Autonet Short Address"},
            {"0x11", "LocalTalk"},
            {"0x12", "LocalNet (IBM PCNet or SYTEK LocalNET)"},
            {"0x13", "Ultra link"},
            {"0x14", "SMDS"},
            {"0x15", "Frame Relay"},
            {"0x16", "Asynchronous Transmission Mode (ATM)"},
            {"0x17", "HDLC"},
            {"0x18", "Fibre Channel"},
            {"0x19", "Asynchronous Transmission Mode (ATM)"},
            {"0x20", "Serial Line"},
            {"0x21", "Asynchronous Transmission Mode (ATM)"},
            {"0x22", "MIL-STD-188-220"},
            {"0x23", "Metricom"},
            {"0x24", "IEEE 1394.1995"},
            {"0x25", "MAPOS"},
            {"0x26", "Twinaxial"},
            {"0x27", "EUI-64"},
            {"0x28", "HIPARP"},
            {"0x29", "IP and ARP over ISO 7816-3"},
            {"0x30", "ARPSec"},
            {"0x31", "IPsec tunnel"},
            {"0x32", "InfiniBand (TM)"},
            {"0x33", "TIA-102 Project 25 Common Air Interface (CAI)"},
            {"0x34", "Wiegand Interface"},
            {"0x35", "Pure IP"},
            {"0x36", "HW_EXP1"},
            {"0x37", "HFI"},
            {"0x256", "HW_EXP2"},
            {"0x257", "AEthernet"},
            {"0x65535", "Reserved"}
        };

        private static Dictionary<string, string> flags = new Dictionary<string, string>
        {
            {"0x0", "Incomplete"},
            {"0x2", "Complete"},
            {"0x4", "Permanent"},
            {"0x6", "Complete and Manually Set"},
            {"0x8", "Published"},
            {"0x10", "Use Trailers"},
            {"0x20", "Netmask"},
            {"0x40", "Dont Publish"},
        };

        private static void Main(string[] args)
        {
            var t = test();
            t.Wait();

            //var t = doWork();
            //t.Wait();
        }

        private static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray(); 

        private static string getChars(int size)
        {            
            byte[] data = new byte[4*size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

         
            return result.ToString();
        }

        private static async Task test()
        {
            const string REFRESH_TOKEN =
                "1//0d9mgH01EnYUeCgYIARAAGA0SNwF-L9IrkWFdSzH6CKgh_2DjbJWx1GepAxmsTm4sPP0OWJ_mf9lrhvoUX1_nHEc_ZNvsDN6frHY";
            string devicesUrl = "https://googlehomefoyer-pa.googleapis.com/v2/groups/AAAAABlsocQ/stations?prettyPrint=false";
            string systemsUrl = "https://googlehomefoyer-pa.googleapis.com/v2/groups?prettyPrint=false";
        
            var authResponse = await client.PostAsync("https://www.googleapis.com/oauth2/v4/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", "936475272427.apps.googleusercontent.com"},
                {"grant_type", "refresh_token"},
                {"refresh_token", REFRESH_TOKEN}
            }));
            var auth = await authResponse.Content.ReadAsAsync<AuthResp>();


            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(auth.token_type, auth.access_token);
            var tokenResponse = await client.PostAsync("https://oauthaccountmanager.googleapis.com/v1/issuetoken",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"app_id", "com.google.OnHub"},
                    {"client_id", "586698244315-vc96jg3mn4nap78iir799fc2ll3rk18s.apps.googleusercontent.com"},
                    {"hl", "en-US"},
                    {"lib_ver", "3.3"},
                    {"response_type", "token"},
                    {
                        "scope",
                        "https://www.googleapis.com/auth/accesspoints https://www.googleapis.com/auth/clouddevices"
                    }
                }));
            var token = await tokenResponse.Content.ReadAsAsync<TokenResp>();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(auth.token_type, token.token);
            var systemsResponse = await client.GetAsync(systemsUrl);
            var systems = await systemsResponse.Content.ReadAsStringAsync();

            var devicesResponse = await client.GetAsync(devicesUrl);
            var devices = await devicesResponse.Content.ReadAsStringAsync();
        }


        private class TokenResp
        {
            public string issueAdvice { get; set; }
            public string token {get;set;}
            public string expiresIn {get;set;}
            public string id_token {get;set;}
        }
        private class AuthResp
        {
            public string access_token {get;set;}
            public int expires_in {get;set;}
            public string scope {get;set;}
            public string token_type {get;set;}
            public string id_token {get;set;}
        }


        private static async Task test1()
        {
            var clientState = getChars(20) + "_" + getChars(36);
            var deviceChallenge = getChars(76);
            var deviceId = Guid.NewGuid().ToString().ToUpper();

            var httpContent = new StringContent(@"
                {
                    ""external_browser"":true,
                    ""report_user_id"": true,
                    ""system_version"": ""13.4"",
                    ""app_version"": ""2.16.4"",
                    ""user_id"": [],
                    ""safari_authentication_session"": true,
                    ""supported_service"": [],
                    ""request_trigger"": ""ADD_ACCOUNT"",
                    ""lib_ver"": ""3.3"",
                    ""package_name"": ""com.google.OnHub"",
                    ""redirect_uri"": ""com.google.sso.586698244315-vc96jg3mn4nap78iir799fc2ll3rk18s:/authCallback"",
                    ""device_name"": ""walljm-hostname"",
                    ""client_id"": ""586698244315-vc96jg3mn4nap78iir799fc2ll3rk18s.apps.googleusercontent.com"",
                    ""mediator_client_id"": ""936475272427.apps.googleusercontent.com"",
                    ""device_id"": """ + deviceId + @""",
                    ""hl"": ""en-US"",
                    ""device_challenge_request"": """ + deviceChallenge + @""",
                    ""client_state"": """ + clientState + @""",
                }", Encoding.UTF8, "application/json");

            var resp = await client.PostAsync("https://oauthaccountmanager.googleapis.com/v1/authadvice", httpContent);
            var json = await resp.Content.ReadAsStringAsync();
        }

        #region OnHub V1
        private static async Task doWork()
        {
            var dict = await getDiagnosticReport("192.168.1.1");
            var netState = ExtractDeviceState(dict["netState"]);
            netState.ArpCache = ExtractArps(dict["/proc/net/arp"]);
            netState.Interfaces = ExtractInterfaces(dict["/bin/ip -s -d addr"]);
            netState.CamTable = ExtractMacs(dict["/sbin/brctl showmacs br-lan"]);

            write(JsonConvert.SerializeObject(netState, Formatting.Indented));
        }

        private static void printJson(List<Parsing.Junos.Tag> netState)
        {
            write("[\n");
            foreach (var tag in netState)
            {
                write($"{tag.ToJSON()},\n");
            }

            write("]\n");
        }

        private static void write(string str)
        {
            Console.WriteLine(str);
            System.IO.File.AppendAllText("onhub.diag", str);
        }

        private static async Task<Dictionary<string, string[]>> getDiagnosticReport(string host)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Host", "localhost");
            var req = await client.GetAsync($"http://{host}/api/v1/diagnostic-report");
            var stream = await req.Content.ReadAsStreamAsync();

            var compressedBytes = ReadBytes(stream);
            var decompressedBytes = Decompress(compressedBytes);
            //System.IO.File.WriteAllBytes("diagnosticreport", decompressedBytes);
            var diag = DiagnosticReport.Parser.ParseFrom(decompressedBytes);

            var dict = new Dictionary<string, string[]>();
            foreach (var cmd in diag.CommandOutputs)
            {
                dict.Add(cmd.Command, cmd.Output.Split('\n').Select(l => l.TrimEnd('\r')).ToArray());
            }

            foreach (var file in diag.Files)
            {
                if (file.Path.StartsWith("/var/log/"))
                {
                    continue;
                }

                dict.Add(file.Path, file.Content.ToStringUtf8().Split('\n').Select(l => l.TrimEnd('\r')).ToArray());
            }

            //dict.Add("config", diag.Config.Split('\n'));
            //dict.Add("netConfig", diag.NetworkConfig.Split('\n'));
            dict.Add("netState", diag.NetworkState.Split('\n'));

            //foreach (var kvp in dict)
            //{
            //    System.IO.File.AppendAllLines("onhub.diag", new[] { kvp.Key });
            //    System.IO.File.AppendAllLines("onhub.diag", kvp.Value);
            //}
            return dict;
        }

        private static List<Interface> ExtractInterfaces(string[] lines)
        {
            var records = new List<Interface>();
            var ifc = new Interface();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Trim().Length == 0)
                {
                    continue;
                }

                if (!line.StartsWith(" "))
                {
                    ifc = new Interface();
                    ifc.IfIndex = line.ParseToIndexOf(":");
                    ifc.Name = line.If(Extensions.ParseAfterIndexOf_PlusLength, ":").ParseToIndexOf(":");
                    ifc.Info = line.If(Extensions.ParseAfterIndexOf_PlusLength, "<").ParseToIndexOf(">");
                    ifc.MTU = line.If(Extensions.ParseAfterIndexOf_PlusLength, "mtu ").ParseToIndexOf(" ");
                    ifc.Qdisc = line.If(Extensions.ParseAfterIndexOf_PlusLength, "qdisc ").ParseToIndexOf(" ");
                    ifc.State = line.If(Extensions.ParseAfterIndexOf_PlusLength, "state ").ParseToIndexOf(" ");
                    ifc.Group = line.If(Extensions.ParseAfterIndexOf_PlusLength, "group ").ParseToIndexOf(" ");
                    ifc.Qlen = line.If(Extensions.ParseAfterIndexOf_PlusLength, "qlen ").ParseToIndexOf(" ");
                    records.Add(ifc);
                }
                else if (line.Contains("link/"))
                {
                    ifc.Link = line.If(Extensions.ParseAfterIndexOf_PlusLength, "link/").ParseToIndexOf(" ");
                    ifc.MAC = line.Trim().ParseAfterIndexOf_PlusLength(" ").ParseToIndexOf(" ");
                    ifc.BRD = line.If(Extensions.ParseAfterIndexOf_PlusLength, "brd ").ParseToIndexOf(" ");
                    ifc.Promiscuity = line.If(Extensions.ParseAfterIndexOf_PlusLength, "promiscuity ")
                        .ParseToIndexOf(" ");
                    ifc.NumTxQueues = line.If(Extensions.ParseAfterIndexOf_PlusLength, "numtxqueues ")
                        .ParseToIndexOf(" ");
                    ifc.NumRxQueues = line.If(Extensions.ParseAfterIndexOf_PlusLength, "numrxqueues ")
                        .ParseToIndexOf(" ");
                }
                else if (line.Contains("inet "))
                {
                    ifc.Inet = line.If(Extensions.ParseAfterIndexOf_PlusLength, "inet ").ParseToIndexOf(" ");
                    ifc.InetScope = line.If(Extensions.ParseAfterIndexOf_PlusLength, "scope ").Trim();
                    line = lines[++i];
                    ifc.InetValidLifetime = line.If(Extensions.ParseAfterIndexOf_PlusLength, "valid_lft ")
                        .ParseToIndexOf(" ");
                    ifc.InetPreferredLifetime = line.If(Extensions.ParseAfterIndexOf_PlusLength, "preferred_lft ")
                        .ParseToIndexOf(" ");
                }
                else if (line.Contains("inet6 "))
                {
                    ifc.Inet6 = line.If(Extensions.ParseAfterIndexOf_PlusLength, "inet6 ").ParseToIndexOf(" ");
                    ifc.Inet6Scope = line.If(Extensions.ParseAfterIndexOf_PlusLength, "scope ").Trim();
                    line = lines[++i];
                    ifc.Inet6ValidLifetime = line.If(Extensions.ParseAfterIndexOf_PlusLength, "valid_lft ")
                        .ParseToIndexOf(" ");
                    ifc.Inet6PreferredLifetime = line.If(Extensions.ParseAfterIndexOf_PlusLength, "preferred_lft ")
                        .ParseToIndexOf(" ");
                }
                else if (line.Contains("RX: "))
                {
                    line = lines[++i];
                    var fields = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    ifc.RxBytes = fields[0];
                    ifc.RxPackets = fields[1];
                    ifc.RxErrors = fields[2];
                    ifc.RxDropped = fields[3];
                    ifc.RxOverrun = fields[4];
                    ifc.RxMcast = fields[5];
                }
                else if (line.Contains("TX: "))
                {
                    line = lines[++i];
                    var fields = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    ifc.TxBytes = fields[0];
                    ifc.TxPackets = fields[1];
                    ifc.TxErrors = fields[2];
                    ifc.TxDropped = fields[3];
                    ifc.TxOverrun = fields[4];
                    ifc.TxMcast = fields[5];
                }
                else if (line.Contains("bridge  "))
                {
                    ifc.StpForwardDelay = line.If(Extensions.ParseAfterIndexOf_PlusLength, "forward_delay ")
                        .ParseToIndexOf(" ");
                    ifc.StpHelloTime = line.If(Extensions.ParseAfterIndexOf_PlusLength, "hello_time ")
                        .ParseToIndexOf(" ");
                    ifc.StpMaxAge = line.If(Extensions.ParseAfterIndexOf_PlusLength, "max_age ").ParseToIndexOf(" ");
                    ifc.NumTxQueues = line.If(Extensions.ParseAfterIndexOf_PlusLength, "numtxqueues ")
                        .ParseToIndexOf(" ");
                    ifc.NumRxQueues = line.If(Extensions.ParseAfterIndexOf_PlusLength, "numrxqueues ")
                        .ParseToIndexOf(" ");
                }
                else if (line.Contains("bridge_slave  "))
                {
                    ifc.State = line.If(Extensions.ParseAfterIndexOf_PlusLength, "state ").ParseToIndexOf(" ");
                    ifc.StpPriority = line.If(Extensions.ParseAfterIndexOf_PlusLength, "priority ").ParseToIndexOf(" ");
                    ifc.StpCost = line.If(Extensions.ParseAfterIndexOf_PlusLength, "cost ").ParseToIndexOf(" ");
                    ifc.StpHairpin = line.If(Extensions.ParseAfterIndexOf_PlusLength, "hairpin ").ParseToIndexOf(" ");
                    ifc.StpGuard = line.If(Extensions.ParseAfterIndexOf_PlusLength, "guard ").ParseToIndexOf(" ");
                    ifc.StpRootBlock = line.If(Extensions.ParseAfterIndexOf_PlusLength, "root_block ")
                        .ParseToIndexOf(" ");
                    ifc.StpFastLeave = line.If(Extensions.ParseAfterIndexOf_PlusLength, "fast_leave ")
                        .ParseToIndexOf(" ");
                    ifc.StpLearning = line.If(Extensions.ParseAfterIndexOf_PlusLength, "learning ").ParseToIndexOf(" ");
                    ifc.StpFlood = line.If(Extensions.ParseAfterIndexOf_PlusLength, "flood ").ParseToIndexOf(" ");
                    ifc.StpMcastFastLeave = line.If(Extensions.ParseAfterIndexOf_PlusLength, "mcast_fast_leave ")
                        .ParseToIndexOf(" ");
                    ifc.NumTxQueues = line.If(Extensions.ParseAfterIndexOf_PlusLength, "numtxqueues ")
                        .ParseToIndexOf(" ");
                    ifc.NumRxQueues = line.If(Extensions.ParseAfterIndexOf_PlusLength, "numrxqueues ")
                        .ParseToIndexOf(" ");
                }
            }

            records.WriteAsTable(l => Console.Write(l));
            return records;
        }

        private static List<Arp> ExtractArps(string[] lines)
        {
            var records = new List<Arp>();
            foreach (var line in lines.Skip(1))
            {
                if (line.Trim().Length == 0) continue;
                var fields = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                records.Add(new Arp
                {
                    IpAddress = fields[0],
                    HwType = hwTypes.ContainsKey(fields[1]) ? hwTypes[fields[1]] : "Unassigned",
                    Flags = flags.ContainsKey(fields[2]) ? flags[fields[2]] : "Unknown",
                    HwAddress = fields[3],
                    Mask = fields[4],
                    Device = fields[5]
                });
            }

            records.WriteAsTable(l => Console.Write(l));
            return records;
        }

        private static List<Mac> ExtractMacs(string[] lines)
        {
            var records = new List<Mac>();
            foreach (var line in lines.Skip(1))
            {
                if (line.Trim().Length == 0) continue;
                var fields = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                records.Add(new Mac
                {
                    IfIndex = fields[0],
                    HwAddress = fields[1],
                    IsLocal = fields[2],
                    Age = fields[3]
                });
            }

            records.WriteAsTable(l => Console.Write(l));
            return records;
        }

        private static byte[] ReadBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        private static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        private static DeviceState ExtractDeviceState(string[] lines)
        {
            var parsed = new Parsing.Junos.Parser().Parse(string.Join('\n', lines));
            //printJson(parsed);
            var ds = new DeviceState();
            foreach (var tag in parsed)
            {
                if (tag.Name == "state_seq_no")
                {
                    ds.StateSeqNo = tag.Value;
                }
                else if (tag.Name == "version")
                {
                    ds.Version = tag.Value;
                }
                else if (tag.Name == "timestamp")
                {
                    ds.TimeStampSeconds = tag.Children.FirstOrDefault()?.Value ?? string.Empty;
                }
                else if (tag.Name == "infra_state")
                {
                    ds.InfraState = ExtractInfraState(tag.Children);
                }
                else if (tag.Name == "network_service_state")
                {
                    ds.NetworkServiceState = ExtractNetworkServiceState(tag.Children);
                }
            }
            return ds;
        }

        private static InfraState ExtractInfraState(IEnumerable<Parsing.Junos.Tag> ast)
        {
            var infra = new InfraState();
            foreach (var tag in ast)
            {
                if (tag.Name == "infra_feature")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "background_data_collection")
                        {
                            infra.BackgroundDataCollection = p.Value;
                        }
                        else if (p.Name == "anonymous_metrics_collection")
                        {
                            infra.AnonymousMetricsCollection = p.Value;
                        }
                    }
                }
                else if (tag.Name == "lighting")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "intensity")
                        {
                            infra.LightingIntensity = p.Value;
                        }
                        else if (p.Name == "auto_intensity")
                        {
                            infra.LightingAutoIntensity = p.Value;
                        }
                    }
                }
                else if (tag.Name == "developer_mode")
                {
                    infra.DeveloperMode = tag.Value;
                }
                else if (tag.Name == "wan_speed_test_results")
                {
                    var sp = new SpeedTestResults();
                    infra.WanSpeedTestResults = sp;
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "date_time_seconds_since_epoch")
                        {
                            sp.DateTimeSecondsSinceEpoch = p.Value;
                        }
                        else if (p.Name == "download_speed_bytes_per_second")
                        {
                            sp.DownloadSpeedBytesPerSecond = p.Value;
                        }
                        else if (p.Name == "upload_speed_bytes_per_second")
                        {
                            sp.UploadSpeedBytesPerSecond = p.Value;
                        }
                        else if (p.Name == "total_bytes_downloaded")
                        {
                            sp.TotalBytesDownloaded = p.Value;
                        }
                        else if (p.Name == "total_bytes_uploaded")
                        {
                            sp.TotalBytesUploaded = p.Value;
                        }
                    }
                }
                else if (tag.Name == "auto_update_channel")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "channel")
                        {
                            infra.AutoUpdateChannel = p.Value;
                        }
                        else if (p.Name == "new_version")
                        {
                            infra.AutoUpdateChannelNewVersion = p.Value;
                        }
                        else if (p.Name == "status")
                        {
                            infra.AutoUpdateChannelStatus = p.Value;
                        }
                    }
                }
                else if (tag.Name == "device_mode")
                {
                    infra.DeveloperMode = tag.Children.FirstOrDefault()?.Value ?? string.Empty;
                }
                else if (tag.Name == "isp_configuration")
                {
                    infra.IspConfigurationType = tag.Children.FirstOrDefault()?.Value ?? string.Empty;
                }
                else if (tag.Name == "device_info")
                {
                    infra.FirmwareVersion = tag.Children.FirstOrDefault()?.Value ?? string.Empty;
                }
                else if (tag.Name == "setup_state")
                {
                    infra.SetupState = tag.Value;
                }
                else if (tag.Name == "experiment_state")
                {
                    infra.ExperimentStateIds = tag.Children.FirstOrDefault().Children.Select(o => o.Value).ToList() ??
                                               new List<string>();
                }
            }

            return infra;
        }

        private static NetworkServiceState ExtractNetworkServiceState(IEnumerable<Parsing.Junos.Tag> ast)
        {
            var result = new NetworkServiceState();
            foreach (var tag in ast)
            {
                if (tag.Name == "network_service_feature")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "upnp")
                        {
                            result.RouterConfig.UpnpEnabled = p.Value;
                        }
                        else if (p.Name == "bridge_mode")
                        {
                            result.RouterConfig.BridgeModeEnabled = p.Value;
                        }
                        else if (p.Name == "traffic_acceleration")
                        {
                            result.RouterConfig.TrafficAcceleration = p.Value;
                        }
                    }
                }
                else if (tag.Name == "dns")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "dns_type")
                        {
                            result.LAN.DnsType = p.Value;
                        }
                        else if (p.Name == "servers")
                        {
                            foreach (var server in p.Children.Select(o => o.Value))
                            {
                                result.LAN.IPv4DnsServers.Add(server);
                            }
                        }
                    }
                }
                else if (tag.Name == "ipv6_state")
                {
                    if (result.IPv6 == null)
                    {
                        result.IPv6 = new IPv6();
                    }
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "non_temporary_address_received")
                        {
                            result.IPv6.NonTemporaryAddressReceived = p.Value;
                        }
                        else if (p.Name == "prefix_delegation_received")
                        {
                            result.IPv6.PrefixDelegationReceived = p.Value;
                        }
                        else if (p.Name == "allow_on_ap")
                        {
                            result.IPv6.AllowOnAp = p.Value;
                        }
                        else if (p.Name == "ap_status")
                        {
                            result.IPv6.ApStatus = p.Value;
                        }
                    }
                }
                else if (tag.Name == "ipv6_config")
                {
                    if (result.IPv6 == null)
                    {
                        result.IPv6 = new IPv6();
                    }
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "enabled")
                        {
                            result.IPv6.Enabled = p.Value;
                        }
                        else if (p.Name == "prefix")
                        {
                            result.IPv6.Prefix = p.Value;
                        }
                        else if (p.Name == "prefix_length")
                        {
                            result.IPv6.PrefixLength = p.Value;
                        }
                        else if (p.Name == "source")
                        {
                            result.IPv6.Source = p.Value;
                        }
                        else if (p.Name == "mode")
                        {
                            result.IPv6.Mode = p.Value;
                        }
                    }
                }
                else if (tag.Name == "router_configuration")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "ip_address")
                        {
                            result.RouterConfig.RouterIpAddress = p.Value;
                        }
                        else if (p.Name == "net_mask")
                        {
                            result.RouterConfig.RouterNetMask = p.Value;
                        }
                    }
                }
                else if (tag.Name == "wan_configuration")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "type")
                        {
                            result.WAN.Type = p.Value;
                        }
                        else if (p.Name == "static_configuration")
                        {
                            foreach (var p1 in p.Children)
                            {
                                if (p1.Name == "ip_address")
                                {
                                    result.WAN.StaticIpAddress = p1.Value;
                                }
                                else if (p1.Name == "netmask")
                                {
                                    result.WAN.StaticNetMask = p1.Value;
                                }
                                else if (p1.Name == "gateway")
                                {
                                    result.WAN.StaticGateway = p1.Value;
                                }
                            }
                        }
                    }
                }
                else if (tag.Name == "wan_state")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "ip_address")
                        {
                            result.WAN.OperIpAddress = p.Value;
                        }
                        else if (p.Name == "gateway_address")
                        {
                            result.WAN.OperGateway = p.Value;
                        }
                        else if (p.Name == "link_speed_mbps")
                        {
                            result.WAN.OperLinkSpeedMbps = p.Value;
                        }
                        else if (p.Name == "primary_wan_interface")
                        {
                            result.WAN.PrimaryWanInterface = p.Value;
                        }
                    }
                }
                else if (tag.Name == "local_network")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "ip_address")
                        {
                            result.LAN.OperIpAddress = p.Value;
                        }
                        else if (p.Name == "netmask")
                        {
                            result.LAN.OperNetMask = p.Value;
                        }
                        else if (p.Name == "pool_begin")
                        {
                            result.LAN.OperDhcpPoolBegin = p.Value;
                        }
                        else if (p.Name == "pool_end")
                        {
                            result.LAN.OperDhcpPoolEnd = p.Value;
                        }
                    }
                }
                else if (tag.Name == "wan_name_servers")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "dns_server")
                        {
                            foreach (var server in p.Children.Select(o => o.Value))
                            {
                                result.WAN.IPv4DnsServers.Add(server);
                            }
                        }
                    }
                }
                else if (tag.Name == "station_state_updates")
                {
                    foreach (var p in tag.Children)
                    {
                        foreach (var p0 in p.Children)
                        {
                            foreach (var p1 in p0.Children)
                            {
                                if (p1.Name == "station_info")
                                {
                                    var st = new StationState();
                                    result.Stations.Add(st);
                                    foreach (var p2 in p1.Children)
                                    {
                                        if (p2.Name == "station_id")
                                        {
                                            st.StationId = p2.Value;
                                        }
                                        else if (p2.Name == "mdns_name")
                                        {
                                            st.MdnsName = p2.Value;
                                        }
                                        else if (p2.Name == "connected")
                                        {
                                            st.Connected = p2.Value;
                                        }
                                        else if (p2.Name == "ip_addresses")
                                        {
                                            st.IpAddresses = p2.Value;
                                        }
                                        else if (p2.Name == "wireless")
                                        {
                                            st.Wireless = p2.Value;
                                        }
                                        else if (p2.Name == "hdcp_hostname")
                                        {
                                            st.DhcpHostname = p2.Value;
                                        }
                                        else if (p2.Name == "wireless_interface")
                                        {
                                            st.WirelessInterface = p2.Value;
                                        }
                                        else if (p2.Name == "taxonomy_ids")
                                        {
                                            if (p2.TagType == Parsing.Junos.TagTypes.Property)
                                            {
                                                st.TaxonomyIds.Add(p2.Value);
                                            }
                                            else if (p2.TagType == Parsing.Junos.TagTypes.Array)
                                            {
                                                st.TaxonomyIds.AddRange(p2.Children.Select(o => o.Value));
                                            }
                                        }
                                        else if (p2.Name == "dns_sd_features")
                                        {
                                            if (p2.TagType == Parsing.Junos.TagTypes.Object)
                                            {
                                                foreach (var f in p2.Children)
                                                {
                                                    if (f.Name == "key")
                                                    {
                                                        st.DnsSdFeatures.Key = f.Value;
                                                    }
                                                    else if (f.Name == "value")
                                                    {
                                                        if (f.Value.StartsWith("md"))
                                                        {
                                                            st.DnsSdFeatures.MD = f.Value.ToString().ParseAfterIndexOf_PlusLength("=");
                                                        }
                                                        else
                                                        if (f.Value.StartsWith("ca"))
                                                        {
                                                            st.DnsSdFeatures.CA = f.Value.ToString().ParseAfterIndexOf_PlusLength("=");
                                                        }
                                                        else
                                                        if (f.Value.StartsWith("fn"))
                                                        {
                                                            st.DnsSdFeatures.FN = f.Value.ToString().ParseAfterIndexOf_PlusLength("=");
                                                        }
                                                    }
                                                }
                                            }
                                            else if (p2.TagType == Parsing.Junos.TagTypes.Array)
                                            {
                                                foreach (var p3 in p2.Children)
                                                {
                                                    foreach (var f in p3.Children)
                                                    {
                                                        if (f.Name == "key")
                                                        {
                                                            st.DnsSdFeatures.Key = f.Value;
                                                        }
                                                        else if (f.Name == "value")
                                                        {
                                                            if (f.Value.StartsWith("md"))
                                                            {
                                                                st.DnsSdFeatures.MD = f.Value.ToString().ParseAfterIndexOf_PlusLength("=");
                                                            }
                                                            else
                                                            if (f.Value.StartsWith("ca"))
                                                            {
                                                                st.DnsSdFeatures.CA = f.Value.ToString().ParseAfterIndexOf_PlusLength("=");
                                                            }
                                                            else
                                                            if (f.Value.StartsWith("fn"))
                                                            {
                                                                st.DnsSdFeatures.FN = f.Value.ToString().ParseAfterIndexOf_PlusLength("=");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else if (p2.Name == "last_seen_seconds_since_epoch")
                                        {
                                            st.LastSeenSecondsSinceEpoch = p2.Value;
                                        }
                                        else if (p2.Name == "oui")
                                        {
                                            st.Oui = p2.Value;
                                        }
                                        else if (p2.Name == "guest")
                                        {
                                            st.Guest = p2.Value;
                                        }
                                        else if (p2.Name == "os_version")
                                        {
                                            st.OsVersion = p2.Value;
                                        }
                                        else if (p2.Name == "device_model")
                                        {
                                            st.DeviceModel = p2.Value;
                                        }
                                        else if (p2.Name == "os_build")
                                        {
                                            st.OsBuild = p2.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (tag.Name == "v6_dns_servers")
                {
                    foreach (var p in tag.Children)
                    {
                        if (p.Name == "dns_server")
                        {
                            foreach (var server in p.Children.Select(o => o.Value))
                            {
                                result.LAN.IPv6DnsServers.Add(server);
                            }
                        }
                    }
                }
            }

            return result;
        }
        
        #endregion
    }

    public class Arp
    {
        public string IpAddress { get; set; }
        public string HwType { get; set; }
        public string Flags { get; set; }
        public string HwAddress { get; set; }
        public string Mask { get; set; }
        public string Device { get; set; }
    }

    public class Mac
    {
        public string IfIndex { get; set; }
        public string HwAddress { get; set; }
        public string IsLocal { get; set; }
        public string Age { get; set; }
    }

    public class Interface
    {
        public string IfIndex { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string State { get; set; }
        public string MAC { get; set; }
        public string BRD { get; set; }

        public string MTU { get; set; }
        public string Qdisc { get; set; }
        public string Group { get; set; }
        public string Qlen { get; set; }
        public string Link { get; set; }
        public string Promiscuity { get; set; }

        public string Inet { get; set; }
        public string InetScope { get; set; }
        public string InetValidLifetime { get; set; }
        public string InetPreferredLifetime { get; set; }

        public string Inet6 { get; set; }
        public string Inet6Scope { get; set; }
        public string Inet6ValidLifetime { get; set; }
        public string Inet6PreferredLifetime { get; set; }

        public string NumRxQueues { get; set; }
        public string RxBytes { get; set; }
        public string RxPackets { get; set; }
        public string RxErrors { get; set; }
        public string RxDropped { get; set; }
        public string RxOverrun { get; set; }
        public string RxMcast { get; set; }

        public string NumTxQueues { get; set; }
        public string TxBytes { get; set; }
        public string TxPackets { get; set; }
        public string TxErrors { get; set; }
        public string TxDropped { get; set; }
        public string TxOverrun { get; set; }
        public string TxMcast { get; set; }

        public string StpPriority { get; set; }
        public string StpCost { get; set; }
        public string StpHairpin { get; set; }
        public string StpGuard { get; set; }
        public string StpRootBlock { get; set; }
        public string StpFastLeave { get; set; }
        public string StpLearning { get; set; }
        public string StpFlood { get; set; }
        public string StpMcastFastLeave { get; set; }
        public string StpForwardDelay { get; set; }
        public string StpHelloTime { get; set; }
        public string StpMaxAge { get; set; }
    }

    public class DeviceState
    {
        public List<Arp> ArpCache { get; set; }
        public List<Mac> CamTable { get; set; }
        public List<Interface> Interfaces { get; set; }
        public string StateSeqNo { get; set; }
        public string Version { get; set; }
        public string TimeStampSeconds { get; set; }
        public InfraState InfraState { get; set; }
        public NetworkServiceState NetworkServiceState { get; set; }
    }

    public class NetworkServiceState
    {
        public IPv6 IPv6 { get; set; }
        public RouterConfig RouterConfig { get; set; } = new RouterConfig();
        public WanStatus WAN { get; set; } = new WanStatus();
        public LanStatus LAN { get; set; } = new LanStatus();
        public List<StationState> Stations { get; set; } = new List<StationState>();
    }

    public class StationState
    {
        public string StationId { get; set; }
        public string MdnsName { get; set; }
        public string Connected { get; set; }
        public string IpAddresses { get; set; }
        public string Wireless { get; set; }
        public string DhcpHostname { get; set; }
        public string WirelessInterface { get; set; }
        public List<string> TaxonomyIds { get; set; } = new List<string>();
        public DnsSdFeature DnsSdFeatures { get; set; } = new DnsSdFeature();
        public string LastSeenSecondsSinceEpoch { get; set; }
        public string Oui { get; set; }
        public string Guest { get; set; }
        public string OsVersion { get; set; }
        public string OsBuild { get; set; }
        public string DeviceModel { get; set; }
    }

    public class DnsSdFeature
    {
        public string FN { get; set; }
        public string MD { get; set; }
        public string CA { get; set; }
        public string Key { get; set; }
    }

    public class LanStatus
    {
        public string OperIpAddress { get; set; }
        public string OperNetMask { get; set; }
        public string OperDhcpPoolBegin { get; set; }
        public string OperDhcpPoolEnd { get; set; }
        public string DnsType { get; set; }
        public HashSet<string> IPv4DnsServers { get; set; } = new HashSet<string>();
        public HashSet<string> IPv6DnsServers { get; set; } = new HashSet<string>();
    }

    public class WanStatus
    {
        public string Type { get; set; }
        public string StaticIpAddress { get; set; }
        public string StaticNetMask { get; set; }
        public string StaticGateway { get; set; }
        public string PppoeUsername { get; set; }

        public string OperIpAddress { get; set; }
        public string OperGateway { get; set; }
        public string OperLinkSpeedMbps { get; set; }
        public string PrimaryWanInterface { get; set; }
        public HashSet<string> IPv4DnsServers { get; set; } = new HashSet<string>();
    }

    public class RouterConfig
    {
        public string UpnpEnabled { get; set; }
        public string BridgeModeEnabled { get; set; }
        public string TrafficAcceleration { get; set; }
        public string RouterIpAddress { get; set; }
        public string RouterNetMask { get; set; }
    }

    public class IPv6
    {
        public string NonTemporaryAddressReceived { get; set; }
        public string PrefixDelegationReceived { get; set; }
        public string AllowOnAp { get; set; }
        public string ApStatus { get; set; }
        public string Enabled { get; set; }
        public string Prefix { get; set; }
        public string PrefixLength { get; set; }
        public string Source { get; set; }
        public string Mode { get; set; }
    }

    public class InfraState
    {
        public string BackgroundDataCollection { get; set; }
        public string AnonymousMetricsCollection { get; set; }
        public string LightingIntensity { get; set; }
        public string LightingAutoIntensity { get; set; }
        public string DeveloperMode { get; set; }
        public SpeedTestResults WanSpeedTestResults { get; set; }
        public string AutoUpdateChannel { get; set; }
        public string AutoUpdateChannelNewVersion { get; set; }
        public string AutoUpdateChannelStatus { get; set; }
        public string DeviceMode { get; set; }
        public string IspConfigurationType { get; set; }
        public string FirmwareVersion { get; set; }
        public string SetupState { get; set; }
        public List<string> ExperimentStateIds { get; set; }
    }

    public class SpeedTestResults
    {
        public string DateTimeSecondsSinceEpoch { get; set; }
        public string DownloadSpeedBytesPerSecond { get; set; }
        public string UploadSpeedBytesPerSecond { get; set; }
        public string TotalBytesDownloaded { get; set; }
        public string TotalBytesUploaded { get; set; }
    }
}