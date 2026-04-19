# Bloodtype Hardcore (RimWorld Mod)

Initial M1 foundation for a bloodtype-focused hardcore medical mod.

## Implemented in M1

- Persistent per-pawn blood type assignment.
- Deterministic assignment for new pawns.
- Blood type compatibility helpers (ABO + Rh).
- Mod settings scaffold for:
  - Blood recovery speed multiplier
  - Reaction severity multiplier
  - Allow compatible non-identical transfusions

## Implemented in M2

- Vanilla `ExtractHemogenPack` is patched to become typed blood extraction.
- New typed blood pack items for all ABO/Rh groups:
  - `BloodPack_O_Positive`, `BloodPack_O_Negative`
  - `BloodPack_A_Positive`, `BloodPack_A_Negative`
  - `BloodPack_B_Positive`, `BloodPack_B_Negative`
  - `BloodPack_AB_Positive`, `BloodPack_AB_Negative`
- Custom surgery worker that:
  - checks blood-loss safety threshold,
  - applies additional blood loss on extraction,
  - spawns blood pack matching donor's assigned blood type.

## Implemented in M3

- Vanilla `BloodTransfusion` is patched to use typed blood logic.
- Custom transfusion worker that applies outcomes by compatibility:
  - Exact match: strong blood loss recovery.
  - Compatible non-identical: partial recovery + mild reaction.
  - Incompatible (or compatible disabled in settings): minimal recovery + severe reaction.
- New hediff: `Bloodtype_TransfusionReaction` with staged severity and gradual recovery over time.

## Implemented in M3.5

- Added blood family model:
  - `Human`
  - `NoBlood`
- Added guardrails for extraction/transfusion:
  - Rejects mechanoids and other `NoBlood` pawns.
  - Rejects unsupported non-human blood families for typed human blood packs.
- Added extension hooks for mod interoperability:
  - `BloodtypeRaceExtension` (on race defs)
  - `BloodtypeGeneExtension` (on gene defs)

These extensions can override blood family and whether a pawn uses human ABO/Rh typing, enabling compatibility with alien race and gene-transfer mods.

## Balance Pass

All major extraction/transfusion values are now exposed in mod settings with guardrail clamps:

- Extraction
  - Blood loss added per extraction
  - Maximum blood loss threshold allowed for extraction
- Transfusion
  - Exact-match recovery
  - Compatible recovery
  - Incompatible recovery
  - Compatible base reaction severity
  - Incompatible base reaction severity
- Global multipliers
  - Blood recovery speed multiplier
  - Reaction severity multiplier

Balance presets are included:

- Easy
- Normal
- Hard
- Ultra Brutal

Ultra Brutal includes optional critical mode:

- Any non-exact typed transfusion causes a catastrophic crisis.
- Crisis progresses to lethal in about 6 in-game hours if untreated.
- New operation: `stabilize transfusion crisis` (repeatable attempts).
- This operation requires `MedicineUltratech`.
- Rescue chance uses surgery-quality factors and is scaled by a configurable `ultraBrutalRescueScale`.
- With base game max conditions and default scale, effective rescue chance is about 40%.
- Successful rescue applies a 3-day clinical coma.

## Structure

- `About/` mod metadata.
- `Languages/English/Keyed/` setting labels.
- `Source/BloodtypeHardcore/` C# source.
- `Assemblies/` build output target.

## Build Notes

The project targets `net472` and expects the RimWorld managed DLL path via:

- MSBuild property: `RimWorldManagedDir`
- Or environment variable: `RIMWORLD_MANAGED_DIR`

Example:

```powershell
dotnet build .\Source\BloodtypeHardcore\BloodtypeHardcore.csproj -p:RimWorldManagedDir="C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed"
```
