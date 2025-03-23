using UnityEngine;
using Verse;

namespace VFEE_Ancient_ExtractPower;

public class VFEEA_Mod : Mod
{
    private int extractFC;

    private int injectFC;

    private int largeWindows;

    private readonly VFEEA_ModSettings settings;

    public VFEEA_Mod(ModContentPack content) : base(content)
    {
        settings = GetSettings<VFEEA_ModSettings>();
        extractFC = (int)(settings.extractFailChance * 100f);
        injectFC = (int)(settings.injectFailChance * 100f);
        largeWindows = (int)settings.largeWindows;
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.Label("VFEEA_ExtractPWR.ModSetting.Desc".Translate());
        listing_Standard.Label("VFEEA_ExtractPWR.ModSetting.ExtractFailChance".Translate() + " " +
                               extractFC.ToString() + " %");
        extractFC = (int)listing_Standard.Slider(extractFC, 0f, 100f);
        settings.extractFailChance = extractFC / 100f;
        listing_Standard.Label("VFEEA_ExtractPWR.ModSetting.InjectFailChance".Translate() + " " + injectFC.ToString() +
                               " %");
        injectFC = (int)listing_Standard.Slider(injectFC, 0f, 100f);
        settings.injectFailChance = injectFC / 100f;
        listing_Standard.Label("VFEEA_ExtractPWR.ModSetting.LargeExtractWindows".Translate() + " " +
                               largeWindows.ToString() + " px");
        largeWindows = (int)listing_Standard.Slider(largeWindows, 500f, 1900f);
        settings.largeWindows = largeWindows;
        listing_Standard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "VFEEA_ExtractPWR.ModSetting.ModName".Translate();
    }
}