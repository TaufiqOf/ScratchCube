using System.Collections.Generic;
using Docker.DotNet.Models;

namespace ScratchDocker.Models.Docker;

public class DockerEndpointSettings
{
    public EndpointIPAMConfig IPAMConfig { get; set; } = default!;
    public IList<string> Links { get; set; } = new List<string>();
    public IList<string> Aliases { get; set; } = new List<string>();
    public string NetworkID { get; set; } = string.Empty;
    public string EndpointID { get; set; } = string.Empty;
    public string Gateway { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public long IPPrefixLen { get; set; } = 0;
    public string IPv6Gateway { get; set; } = string.Empty;
    public string GlobalIPv6Address { get; set; } = string.Empty;
    public long GlobalIPv6PrefixLen { get; set; } = 0;
    public string MacAddress { get; set; } = string.Empty;
    public IDictionary<string, string> DriverOpts { get; set; } = new Dictionary<string, string>();
}