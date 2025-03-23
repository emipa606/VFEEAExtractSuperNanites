using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

internal class Middle_Fail_Inject_Berserk : Middle_Fail_Inject
{
    protected override string Label => "VFEAncients.Berserk".Translate();

    public override void RunOnPawn(Pawn pawn, PowerDef power)
    {
        VFEA_DefOf.Berserk.Worker.TryStart(pawn, "VFEAncients.ExperimentFailed".Translate(), false);
        SendLetter("VFEEA_ExtractPWR.InjectMiddleFailed.Berserk".Translate(power.label), pawn);
    }
}