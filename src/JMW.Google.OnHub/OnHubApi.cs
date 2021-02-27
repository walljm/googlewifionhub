using Diagnosticreport;
using JMW.Google.OnHub.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace JMW.Google.OnHub
{
    public class OnHubApi
    {
        private static readonly HttpClient client = new HttpClient();

        private static readonly Dictionary<string, string> hwTypes = new Dictionary<string, string>
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

        private static readonly Dictionary<string, string> flags = new Dictionary<string, string>
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

        public static async Task<DeviceState> GetData(IPAddress target)
        {
            var dict = await getDiagnosticReport(target);
            var netState = extractDeviceState(dict["netState"]);
            netState.ArpCache = extractArps(dict["/proc/net/arp"]);
            netState.Interfaces = extractInterfaces(dict["/bin/ip -s -d addr"]);
            netState.CamTable = extractMacs(dict["/sbin/brctl showmacs br-lan"]);
            return netState;
        }

        private static async Task<Dictionary<string, string[]>> getDiagnosticReport(IPAddress target)
        {
            client.Timeout = new TimeSpan(0, 5, 0); // 5 min timeout... these things can take a while...
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Host", "localhost");
            var req = await client.GetAsync($"http://{target}/api/v1/diagnostic-report");

            // if there was an error let us know
            if (!req.IsSuccessStatusCode)
            {
                throw new Exception(req.ReasonPhrase);
            }
            
            var stream = await req.Content.ReadAsStreamAsync();
            var compressedBytes = readBytes(stream);
            var decompressedBytes = decompress(compressedBytes);
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

            dict.Add("netState", diag.NetworkState.Split('\n'));
            return dict;
        }

        private static byte[] readBytes(Stream input)
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

        private static byte[] decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        #region Parsing Functions

        private static List<Interface> extractInterfaces(string[] lines)
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

            return records;
        }

        private static List<Arp> extractArps(string[] lines)
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

            return records;
        }

        private static List<Mac> extractMacs(string[] lines)
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

            return records;
        }

        private static DeviceState extractDeviceState(string[] lines)
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
                    ds.InfraState = extractInfraState(tag.Children);
                }
                else if (tag.Name == "network_service_state")
                {
                    ds.NetworkServiceState = extractNetworkServiceState(tag.Children);
                }
            }
            return ds;
        }

        private static InfraState extractInfraState(IEnumerable<Parsing.Junos.Tag> ast)
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

        private static NetworkServiceState extractNetworkServiceState(IEnumerable<Parsing.Junos.Tag> ast)
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

        #endregion Parsing Functions
    }
}