namespace ScratchDocker.Models.Docker;

public class DockerMountPoint
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public string Driver { get; set; }
    public string Mode { get; set; }
    public bool RW { get; set; }
    public string Propagation { get; set; }
}
