using System.ComponentModel;

namespace RideHiveApi.Models.Enums
{
    public enum TransmissionType
    {
        [Description("Continuously variable")]
        CVT,
        [Description("Dual Clutch automatic")]
        DCT,
        [Description("Automatic")]
        Automatic,
        [Description("Manual")]
        Manual,
        [Description("Automated manual")]
        AMT,
        [Description("Torque converter")]
        TorqueConverter,
        [Description("Direct-shift gearbox")]
        DSG,
        [Description("Semi-automatic")]
        SemiAutomatic,
        [Description("Tiptronic")]
        Tiptronic
    };
}