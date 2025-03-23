using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using VFEAncients;

namespace VFEE_Ancient_ExtractPower;

[StaticConstructorOnStartup]
public class Dialog_InjectPowers : Window
{
    public static readonly Texture2D SuperpowerBackgroundTex =
        ContentFinder<Texture2D>.Get("Powers/Backgrounds/Background_Power");

    private readonly List<Tuple<PowerDef, Building_PowerInjector>> choices;

    private readonly Action<Tuple<PowerDef, Building_PowerInjector>> onChosen;

    private readonly Pawn pawn;

    public Dialog_InjectPowers(List<Tuple<PowerDef, Building_PowerInjector>> choices, Pawn pawn,
        Action<Tuple<PowerDef, Building_PowerInjector>> onChosen)
    {
        this.choices = choices;
        this.pawn = pawn;
        this.onChosen = onChosen;
        forcePause = true;
    }

    public override Vector2 InitialSize => new Vector2(500f, 230f);

    public override void DoWindowContents(Rect inRect)
    {
        inRect = inRect.ContractedBy(15f, 7f);
        Widgets.Label(inRect.TopPartPixels(60f), "VFEEA_ExtractPWR.InjectChoice".Translate(pawn.NameShortColored));
        inRect.y += 60f;
        foreach (var valueTuple in choices.Zip(Split(inRect, choices.Count, new Vector2(80f, 200f)),
                     (tuple, rect) =>
                         new ValueTuple<PowerDef, Building_PowerInjector, Rect>(tuple.Item1, tuple.Item2, rect)))
        {
            var item = valueTuple.Item1;
            var item2 = valueTuple.Item2;
            var item3 = valueTuple.Item3;
            var rect3 = new Rect(item3.x, item3.y, 80f, 80f);
            var rect2 = new Rect(item3.x + 5f, item3.y + 100f, 70f, 30f);
            GUI.DrawTexture(rect3, SuperpowerBackgroundTex);
            GUI.DrawTexture(rect3, item.Icon);
            TooltipHandler.TipRegion(rect3,
                new TipSignal(string.Format("{0}\n\n{1}\n{2}", item.LabelCap, item.description,
                    item.Worker.EffectString())));
            if (!Widgets.ButtonText(rect2, "VFEAncients.Select".Translate()))
            {
                continue;
            }

            onChosen(new Tuple<PowerDef, Building_PowerInjector>(item, item2));
            Close();
            break;
        }
    }

    private static IEnumerable<Rect> Split(Rect rect, int parts, Vector2 size, bool vertical = false)
    {
        var distance = (vertical ? rect.height : rect.width) / parts;
        var curLoc = new Vector2(rect.x, rect.y);
        var offset = vertical
            ? new Vector2(0f, (distance / 2f) - (size.y / 2f))
            : new Vector2((distance / 2f) - (size.x / 2f), 0f);
        for (var i = 0f; i < (vertical ? rect.height : rect.width); i += distance)
        {
            yield return new Rect(curLoc + offset, size);
            if (vertical)
            {
                curLoc.y += distance;
            }
            else
            {
                curLoc.x += distance;
            }
        }
    }
}