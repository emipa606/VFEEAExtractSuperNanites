using System.Collections.Generic;
using Verse.AI;

namespace VFEE_Ancient_ExtractPower;

public class JobDriver_ExtractActiveSN : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_General.Wait(300).FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate
            {
                var building_PowerInjector = (Building_PowerInjector)job.GetTarget(TargetIndex.A).Thing;
                building_PowerInjector.DropActiveSN();
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}