using Verse;

namespace VFEE_Ancient_ExtractPower;

public class ThingComp_powerDef : ThingComp
{
    public CompProperties_powerDef Props => props as CompProperties_powerDef;

    public string VFEEA_powerDef => Props.VFEEA_powerDef;
}