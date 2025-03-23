using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEE_Ancient_ExtractPower;

[StaticConstructorOnStartup]
public class Command_SetSuperNanites(Building_PowerInjector building, bool removeActiveSn = false)
    : Command
{
    public readonly bool allowRemovesuperNanites = removeActiveSn;

    public readonly Building_PowerInjector building = building;

    public readonly Map map = building.Map;

    public List<Thing> superNanites;

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        if (building.InjectorState == Enum_InjectorState.Inactive)
        {
            var list = new List<FloatMenuOption>();
            var hashSet = new HashSet<ThingDef>();
            hashSet.AddRange(from x in map.listerThings.AllThings
                where x.TryGetComp<ThingComp_powerDef>() != null
                select x.def);
            using (var enumerator = hashSet.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var SN = enumerator.Current;
                    if (SN != null)
                    {
                        list.Add(new FloatMenuOption(SN.LabelCap, delegate { InsertSN(SN); },
                            MenuOptionPriority.Default,
                            null, null, 29f));
                    }
                }
            }

            if (allowRemovesuperNanites && building.ActiveSN != null)
            {
                list.Add(new FloatMenuOption("VFEEA_ExtractPWR.RemoveCurrentSN".Translate(),
                    RemoveActiveSN, MenuOptionPriority.Default, null, null, 29f));
            }

            if (list.Count == 0)
            {
                list.Add(new FloatMenuOption("None".Translate(), null, MenuOptionPriority.Default, null, null, 29f));
            }

            Find.WindowStack.Add(new FloatMenu(list));
        }
        else
        {
            Messages.Message("VFEEA_ExtractPWR.SNCurrentlyUsed".Translate(), MessageTypeDefOf.CautionInput);
        }
    }

    private void InsertSN(ThingDef SN)
    {
        building.activeSNToBeProcessed = SN;
    }

    private void RemoveActiveSN()
    {
        building.activeSNToBeProcessed = null;
        building.removeActiveSN = true;
    }
}