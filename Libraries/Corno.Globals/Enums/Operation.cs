namespace Corno.Globals.Enums;

public enum Operation
{
    None,

    // Label
    PartLabel,

    // Packing
    Packing,
    WardrobePacking,
    ShutterPacking,
    CarcassPacking,
    PackFreeNoWeight,
    PackFreeWeight,
    PackBomWeight,
    DimensionPacking,
    DimensionVisionPacking,

    // Warehouse
    Loading,
    Unloading,
    PalletInFg,
    RackInFg,
    RackOutFg,
    Dispatch,

    // Inward
    Grn,
    Inward,
    Qc,

    // Location
    LocationTransfer,
}