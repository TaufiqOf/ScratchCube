using System.Collections.Generic;

namespace ScratchCube.Models.Docker;

public class DockerSummaryNetworkSettings
{
    public IDictionary<string, DockerEndpointSettings> Networks { get; set; }

    public DockerSummaryNetworkSettings()
    {
        Networks = new Dictionary<string, DockerEndpointSettings>();
    }
}