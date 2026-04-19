using UnityEngine;
using Verse;

namespace BloodtypeHardcore
{
    public class BloodtypeMod : Mod
    {
        public static BloodtypeModSettings Settings;

        public BloodtypeMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<BloodtypeModSettings>();
            Settings.Clamp();
        }

        public override string SettingsCategory()
        {
            return "Bloodtype Hardcore";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("BloodtypeHardcore.Settings.Section.Presets".Translate());
            listing.Label("BloodtypeHardcore.Settings.CurrentPreset".Translate(Settings.selectedPreset));
            if (listing.ButtonText("BloodtypeHardcore.Settings.Preset.Easy".Translate()))
            {
                Settings.ApplyPresetEasy();
            }
            if (listing.ButtonText("BloodtypeHardcore.Settings.Preset.Normal".Translate()))
            {
                Settings.ApplyPresetNormal();
            }
            if (listing.ButtonText("BloodtypeHardcore.Settings.Preset.Hard".Translate()))
            {
                Settings.ApplyPresetHard();
            }
            if (listing.ButtonText("BloodtypeHardcore.Settings.Preset.UltraBrutal".Translate()))
            {
                Settings.ApplyPresetUltraBrutal();
            }
            listing.GapLine();

            listing.Label("BloodtypeHardcore.Settings.Section.Global".Translate());
            listing.Gap(4f);
            listing.Label("BloodtypeHardcore.Settings.BloodRecoveryMultiplier".Translate(Settings.bloodRecoveryMultiplier.ToStringPercent()));
            Settings.bloodRecoveryMultiplier = MarkCustomIfChanged(Settings.bloodRecoveryMultiplier, listing.Slider(Settings.bloodRecoveryMultiplier, 0.05f, 2.0f));
            listing.GapLine();

            listing.Label("BloodtypeHardcore.Settings.ReactionSeverityMultiplier".Translate(Settings.reactionSeverityMultiplier.ToString("0.00")));
            Settings.reactionSeverityMultiplier = MarkCustomIfChanged(Settings.reactionSeverityMultiplier, listing.Slider(Settings.reactionSeverityMultiplier, 0.1f, 3.0f));
            listing.GapLine();

            bool currentAllowCompatible = Settings.allowCompatibleNonIdenticalTransfusions;
            listing.CheckboxLabeled(
                "BloodtypeHardcore.Settings.AllowCompatibleNonIdentical".Translate(),
                ref Settings.allowCompatibleNonIdenticalTransfusions,
                "BloodtypeHardcore.Settings.AllowCompatibleNonIdentical.Desc".Translate());
            if (currentAllowCompatible != Settings.allowCompatibleNonIdenticalTransfusions)
            {
                MarkCustomPreset();
            }
            listing.GapLine();

            listing.Label("BloodtypeHardcore.Settings.Section.Extraction".Translate());
            listing.Gap(4f);
            listing.Label("BloodtypeHardcore.Settings.ExtractBloodLossAdded".Translate(Settings.extractBloodLossAdded.ToStringPercent()));
            Settings.extractBloodLossAdded = MarkCustomIfChanged(Settings.extractBloodLossAdded, listing.Slider(Settings.extractBloodLossAdded, 0.01f, 0.80f));
            listing.Label("BloodtypeHardcore.Settings.ExtractMaxAllowedBloodLoss".Translate(Settings.extractMaxAllowedBloodLoss.ToStringPercent()));
            Settings.extractMaxAllowedBloodLoss = MarkCustomIfChanged(Settings.extractMaxAllowedBloodLoss, listing.Slider(Settings.extractMaxAllowedBloodLoss, 0.05f, 0.80f));
            listing.GapLine();

            listing.Label("BloodtypeHardcore.Settings.Section.Transfusion".Translate());
            listing.Gap(4f);
            listing.Label("BloodtypeHardcore.Settings.ExactMatchBloodLossRecovery".Translate(Settings.exactMatchBloodLossRecovery.ToStringPercent()));
            Settings.exactMatchBloodLossRecovery = MarkCustomIfChanged(Settings.exactMatchBloodLossRecovery, listing.Slider(Settings.exactMatchBloodLossRecovery, 0.01f, 0.80f));
            listing.Label("BloodtypeHardcore.Settings.CompatibleBloodLossRecovery".Translate(Settings.compatibleBloodLossRecovery.ToStringPercent()));
            Settings.compatibleBloodLossRecovery = MarkCustomIfChanged(Settings.compatibleBloodLossRecovery, listing.Slider(Settings.compatibleBloodLossRecovery, 0.0f, 0.80f));
            listing.Label("BloodtypeHardcore.Settings.IncompatibleBloodLossRecovery".Translate(Settings.incompatibleBloodLossRecovery.ToStringPercent()));
            Settings.incompatibleBloodLossRecovery = MarkCustomIfChanged(Settings.incompatibleBloodLossRecovery, listing.Slider(Settings.incompatibleBloodLossRecovery, 0.0f, 0.30f));
            listing.Gap(4f);
            listing.Label("BloodtypeHardcore.Settings.CompatibleReactionSeverity".Translate(Settings.compatibleReactionSeverity.ToStringPercent()));
            Settings.compatibleReactionSeverity = MarkCustomIfChanged(Settings.compatibleReactionSeverity, listing.Slider(Settings.compatibleReactionSeverity, 0.0f, 1.0f));
            listing.Label("BloodtypeHardcore.Settings.IncompatibleReactionSeverity".Translate(Settings.incompatibleReactionSeverity.ToStringPercent()));
            Settings.incompatibleReactionSeverity = MarkCustomIfChanged(Settings.incompatibleReactionSeverity, listing.Slider(Settings.incompatibleReactionSeverity, 0.0f, 1.0f));
            bool currentUltra = Settings.ultraBrutalCriticalMode;
            listing.CheckboxLabeled(
                "BloodtypeHardcore.Settings.UltraBrutalCriticalMode".Translate(),
                ref Settings.ultraBrutalCriticalMode,
                "BloodtypeHardcore.Settings.UltraBrutalCriticalMode.Desc".Translate());
            if (currentUltra != Settings.ultraBrutalCriticalMode)
            {
                MarkCustomPreset();
            }
            listing.Label("BloodtypeHardcore.Settings.UltraBrutalRescueScale".Translate(Settings.ultraBrutalRescueScale.ToStringPercent()));
            Settings.ultraBrutalRescueScale = MarkCustomIfChanged(Settings.ultraBrutalRescueScale, listing.Slider(Settings.ultraBrutalRescueScale, 0.05f, 1.0f));

            listing.End();
            Settings.Clamp();
        }

        private static float MarkCustomIfChanged(float previous, float current)
        {
            if (!Mathf.Approximately(previous, current))
            {
                MarkCustomPreset();
            }

            return current;
        }

        private static void MarkCustomPreset()
        {
            if (Settings.selectedPreset != "Custom")
            {
                Settings.selectedPreset = "Custom";
            }
        }
    }
}
