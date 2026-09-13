namespace ScratchDocker.Models.Docker;

public class DockerPort
{
    public string IP { get; set; }
    public ushort PrivatePort { get; set; }
    public ushort PublicPort { get; set; }
    public string Type { get; set; }
}