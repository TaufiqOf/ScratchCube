using System.Collections.Generic;

namespace ScratchDocker.Models.Docker;

public class DockerSummaryNetworkSettings
{
    public IDictionary<string, DockerEndpointSettings> Networks { get; set; }
}