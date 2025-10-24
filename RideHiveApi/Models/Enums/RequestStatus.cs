using System.ComponentModel;

namespace RideHiveApi.Models.Enums
{
    public enum RequestStatus
    {
        [Description("Pending approval")]
        Pending,

        [Description("Approved by the provider")]
        Approved,

        [Description("Rejected by the provider")]
        Rejected,

        [Description("Cancelled by the client")]
        Cancelled,

        [Description("Request completed successfully")]
        Completed,

        [Description("Expired (time slot passed)")]
        Expired
    }
}