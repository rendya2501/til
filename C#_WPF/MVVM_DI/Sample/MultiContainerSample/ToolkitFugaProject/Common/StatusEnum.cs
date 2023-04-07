using System.ComponentModel;

namespace ToolkitFugaProject.Common;

public enum StatusEnum
{
    [Description("No status yet")]
    None = 0,

    [Description("Waiting to run")]
    Waiting = 1,

    [Description("Run in progress")]
    Running = 2,

    [Description("Paused by user")]
    Paused = 3,

    [Description("Run is a success")]
    Success = 4,

    [Description("Run finished with error")]
    Failure = 5
}
