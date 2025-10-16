using System.ComponentModel;

namespace RideHiveApi.Models.Enums
{
    public enum AccountType
    {
        [Description("Owner")]
        Owner,
        [Description("Client")]
        Client,
    }
}