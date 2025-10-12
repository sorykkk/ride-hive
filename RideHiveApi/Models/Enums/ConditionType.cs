using System.ComponentModel;
namespace RideHiveApi.Models.Enums
{
    public enum ConditionType
    {
        [Description("Brand new")]
        BrandNew,
        [Description("Like new")]
        LikeNew,
        [Description("Used")]
        Used,
        [Description("Damaged")]
        Damaged
    };
}