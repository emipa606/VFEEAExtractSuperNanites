using Verse;

namespace VFEE_Ancient_ExtractPower;

public class VFEEA_ModSettings : ModSettings
{
    public float extractFailChance = 0.3f;

    public float injectFailChance = 0.4f;

    public float largeWindows = 500f;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref extractFailChance, "extractFailChance");
        Scribe_Values.Look(ref injectFailChance, "injectFailChance", 0.4f);
        Scribe_Values.Look(ref largeWindows, "largeWindows", 500f);
        base.ExposeData();
    }
}