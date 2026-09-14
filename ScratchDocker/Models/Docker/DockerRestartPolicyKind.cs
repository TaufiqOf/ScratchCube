using System.Runtime.Serialization;

namespace ScratchDocker.Models.Docker;

public enum DockerRestartPolicyKind
{
    [EnumMember(Value = "")]
    Undefined,

    [EnumMember(Value = "no")]
    No,

    [EnumMember(Value = "always")]
    Always,

    [EnumMember(Value = "on-failure")]
    OnFailure,

    [EnumMember(Value = "unless-stopped")]
    UnlessStopped
}