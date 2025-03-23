using RimWorld;
using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

public abstract class Middle_Fail_Inject
{
    protected abstract string Label { get; }

    public abstract void RunOnPawn(Pawn pawn, PowerDef power);

    protected virtual void SendLetter(string letterText, LookTargets targets = null)
    {
        Find.LetterStack.ReceiveLetter("VFEEA_ExtractPWR.ExperimentMiddleFailed".Translate() + ": " + Label, letterText,
            LetterDefOf.NegativeEvent, targets);
    }
}