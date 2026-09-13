using System;
using System.Collections.Generic;

namespace ScratchDocker.Models.Docker;

public class DockerContainer
{
    public string ID { get; set; } = string.Empty;
    public IList<string> Names { get; set; } = new List<string>();
    public string Image { get; set; } = string.Empty;
    public string ImageID { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public IList<DockerPort> Ports { get; set; } = new List<DockerPort>();
    public long SizeRw { get; set; }
    public long SizeRootFs { get; set; }
    public IDictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();
    public string State { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DockerSummaryNetworkSettings NetworkSettings { get; set; } = new DockerSummaryNetworkSettings();
    public IList<DockerMountPoint> Mounts { get; set; } = new List<DockerMountPoint>();
}