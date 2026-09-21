namespace ScratchCube.Models;

public class DockerPullProgress
{
    public string? Id { get; init; }
    public string? Status { get; init; }

    public long? Current { get; init; }
    public long? Total { get; init; }

    public double? Percentage { get; init; }

    public string? Error { get; init; }
}