using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Middle_Fail_Inject_Death : Middle_Fail_Extract
{
    protected override string Label => "VFEAncients.Death".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        pawn.Kill(null);
        SendLetter("VFEEA_ExtractPWR.ExperimentMiddleFailed.Death".Translate(power.label), pawn.Corpse);
    }
}