using System.Collections.Generic;

namespace ScratchDocker.Service;

public class DockerService
{
    public List<DockerInstance> DockerInstances { get; set; }
    public DockerService()
    {
        DockerInstances = new List<DockerInstance>();
    }
    public void AddDockerInstance(string uri)
    {
        var instance = new DockerInstance(uri);
        DockerInstances.Add(instance);
    }
    public void AddDefaultDockerInstance()
    {
        var uri = "unix:///var/run/docker.sock";
        var instance = new DockerInstance(uri);
        DockerInstances.Add(instance);
    }
    public void RemoveDockerInstance(DockerInstance instance)
    {
        DockerInstances.Remove(instance);
    }
    public void ClearDockerInstances()
    {
        DockerInstances.Clear();
    }
    
    public void Connect(DockerInstance instance)
    {
        instance.Connect();
    }
    
}