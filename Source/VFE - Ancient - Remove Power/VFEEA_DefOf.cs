using RimWorld;
using Verse;

namespace VFEE_Ancient_ExtractPower;

[DefOf]
public static class VFEEA_DefOf
{
    public static JobDef VFEEA_InsertSN;

    public static JobDef VFEEA_ExtractActiveSN;

    public static ThingDef VFEEA_PowerInjector;

    static VFEEA_DefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(VFEEA_DefOf));
    }
}