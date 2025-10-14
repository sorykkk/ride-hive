using System.ComponentModel;

namespace RideHiveApi.Models.Enums
{
    public enum FuelType
    {
        [Description("Diesel")]
        Diesel,
        [Description("Petrol")]
        Petrol,
        [Description("Electric")]
        Electric,
        [Description("Hybrid")]
        Hybrid,
        [Description("Biodiesel")]
        Biodiesel,
        [Description("Hydrogen")]
        Hydrogen,
        [Description("LPG")]
        LPG,
        [Description("Ethanol")]
        Ethanol,
        [Description("Natural Gas")]
        NaturalGas,
        [Description("Gasoline")]
        Gasoline,
        [Description("Fuel Oil")]
        FuelOil,
        [Description("Liquid fuels")]
        LiquidFuels,
        [Description("Solid fuels")]
        SolidFuels,
        [Description("Kerosene")]
        Kerosene,
        [Description("Methanol")]
        Methanol,
        [Description("E10")]
        E10
    };
}