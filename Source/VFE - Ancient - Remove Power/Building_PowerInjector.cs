using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEE_Ancient_ExtractPower;

public class Building_PowerInjector : Building, IThingHolder
{
    public ThingDef activeSNToBeProcessed;

    public Enum_InjectorState InjectorState;

    protected ThingOwner innerContainer;

    public bool removeActiveSN;

    public Building_PowerInjector()
    {
        innerContainer = new ThingOwner<Thing>(this, false);
    }

    public Thing ActiveSN
    {
        get { return innerContainer.FirstOrDefault(x => x.TryGetComp<ThingComp_powerDef>() != null); }
    }

    public Thing InnerThing
    {
        get { return innerContainer.FirstOrDefault(x => x.TryGetComp<ThingComp_powerDef>() == null); }
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (Faction != Faction.OfPlayer)
        {
            yield break;
        }

        if (activeSNToBeProcessed == null && ActiveSN == null)
        {
            var command_Action3 = new Command_SetSuperNanites(this)
            {
                defaultLabel = "VFEEA_ExtractPWR.Insert".Translate(),
                defaultDesc = "VFEEA_ExtractPWR.Insert.Text".Translate(),
                hotKey = KeyBindingDefOf.Misc8,
                icon = ContentFinder<Texture2D>.Get("UI/Icons/Empowered_SuperNanites_Grayed")
            };
            if (InjectorState > Enum_InjectorState.Inactive)
            {
                command_Action3.Disable("VFEEA_ExtractPWR.SNCurrentlyUsed".Translate());
            }

            yield return command_Action3;
        }

        if (ActiveSN != null)
        {
            var command_Action4 = new Command_SetSuperNanites(this, true)
            {
                defaultLabel = ActiveSN.LabelCap,
                defaultDesc = "VFEEA_ExtractPWR.Insert".Translate(),
                hotKey = KeyBindingDefOf.Misc8,
                icon = ContentFinder<Texture2D>.Get("UI/Icons/Empowered_SuperNanites")
            };
            if (InjectorState > Enum_InjectorState.Inactive)
            {
                command_Action4.Disable("VFEEA_ExtractPWR.SNCurrentlyUsed".Translate());
            }

            yield return command_Action4;
        }
        else
        {
            if (activeSNToBeProcessed == null)
            {
                yield break;
            }

            var command_Action5 = new Command_SetSuperNanites(this, true)
            {
                defaultLabel = activeSNToBeProcessed.LabelCap,
                defaultDesc = "VFEEA_ExtractPWR.Insert".Translate() + activeSNToBeProcessed.LabelCap,
                hotKey = KeyBindingDefOf.Misc8,
                icon = ContentFinder<Texture2D>.Get("UI/Icons/Empowered_SuperNanites")
            };
            if (InjectorState > Enum_InjectorState.Inactive)
            {
                command_Action5.Disable("VFEEA_ExtractPWR.SNCurrentlyUsed".Translate());
            }

            yield return command_Action5;
        }
    }

    public override string GetInspectString()
    {
        string result;
        switch (InjectorState)
        {
            case Enum_InjectorState.Inactive:
                result = "VFEEA_ExtractPWR.Enum_InjectorState.Inactive".Translate(base.GetInspectString());
                break;
            case Enum_InjectorState.ToBeRemoved:
                result = "VFEEA_ExtractPWR.Enum_InjectorState.ToBeRemoved".Translate(base.GetInspectString());
                break;
            case Enum_InjectorState.ToBeInserted:
                result = "VFEEA_ExtractPWR.Enum_InjectorState.ToBeInserted".Translate(base.GetInspectString());
                break;
            case Enum_InjectorState.Injecting:
                result = "VFEEA_ExtractPWR.Enum_InjectorState.Injecting".Translate(base.GetInspectString());
                break;
            default:
                result = "VFEEA_ExtractPWR.Enum_InjectorState.default".Translate(base.GetInspectString());
                break;
        }

        return result;
    }

    public bool isEmpty()
    {
        return !innerContainer.Any();
    }

    public void DropActiveSN()
    {
        innerContainer.TryDrop(ActiveSN, InteractionCell, Map, ThingPlaceMode.Near, 1, out _);
        removeActiveSN = false;
    }

    public void ConsumeActiveSN()
    {
        innerContainer.RemoveAll(x => x != null);
        removeActiveSN = false;
    }

    public void AcceptSN(Thing thing)
    {
        if (ActiveSN != null)
        {
            DropActiveSN();
        }

        innerContainer.TryAddOrTransfer(thing);
        activeSNToBeProcessed = null;
        removeActiveSN = false;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref innerContainer, "VFEEA_innerContainer", this);
        Scribe_Defs.Look(ref activeSNToBeProcessed, "VFEEA_activeSNToBeProcessed");
        Scribe_Values.Look(ref InjectorState, "VFEEA_InjectorState");
        Scribe_Values.Look(ref removeActiveSN, "VFEEA_removeActiveSN");
    }
}