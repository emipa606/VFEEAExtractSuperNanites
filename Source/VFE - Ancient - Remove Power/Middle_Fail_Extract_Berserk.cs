using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Middle_Fail_Extract_Berserk : Middle_Fail_Extract
{
    protected override string Label => "VFEAncients.Berserk".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        VFEA_DefOf.Berserk.Worker.TryStart(pawn, "VFEAncients.ExperimentFailed".Translate(), false);
        SendLetter("VFEEA_ExtractPWR.ExperimentMiddleFailed.Berserk".Translate(power.label), pawn);
    }
}