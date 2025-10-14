using System.ComponentModel;
namespace RideHiveApi.Models.Enums
{
    public enum DriveTrainLayoutType
    {
        [Description("Front-Wheel")]
        FWD,
        [Description("Rear-Wheel")]
        RWD,
        [Description("All-Wheel")]
        AWD,
        [Description("Four-Wheel")]
        WD4x4
    };
}