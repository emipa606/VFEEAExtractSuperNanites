using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Fail_Inject_Death : Fail_Inject
{
    public override string Label => "VFEAncients.Death".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        pawn.Kill(null);
        SendLetter("VFEEA_ExtractPWR.InjectFailed.Death".Translate(power.label), pawn.Corpse);
    }
}