# Bloodtype Hardcore (RimWorld Mod)

Hardcore medical realism mod focused on typed blood extraction, transfusion compatibility, and crisis management.

## Current Feature Set

### Blood typing and compatibility
- Persistent per-pawn ABO + Rh typing.
- Compatibility logic for exact, compatible non-identical, and incompatible transfusions.
- Blood type markers are hidden by default (logic still active).

### Extraction and blood packs
- Vanilla `ExtractHemogenPack` is repurposed into typed blood extraction.
- Typed blood packs:
  - `BloodPack_O_Positive`, `BloodPack_O_Negative`
  - `BloodPack_A_Positive`, `BloodPack_A_Negative`
  - `BloodPack_B_Positive`, `BloodPack_B_Negative`
  - `BloodPack_AB_Positive`, `BloodPack_AB_Negative`
  - `BloodPack_Unknown` (icon/def support)
- Extraction respects blood-loss safety checks and applies extraction blood-loss cost.
- Blood packs have custom blood-bag icons with type overlays.

### Transfusion behavior
- Vanilla `BloodTransfusion` uses typed packs instead of generic hemogen packs.
- Outcome model:
  - Exact match: best recovery.
  - Compatible non-identical: reduced recovery + reaction risk.
  - Incompatible: minimal recovery + severe reaction risk.
- Hemogen restoration remains supported for hemogenic pawns.

### Crisis system and stabilization
- Ultra Brutal mode can trigger catastrophic crisis on bad transfusions.
- Crisis is now phased (sentinel -> acute -> terminal severity thresholds).
- Dedicated `stabilize transfusion crisis` operation (separate from transfusion).
- Stabilization is auto-repeat QoL:
  - Failed attempts automatically queue another attempt.
- Stabilization uses surgery factors + medicine potency + stat-driven resilience:
  - `BloodFiltration`
  - `BloodPumping`
  - `ImmunityGainSpeed`
- Successful stabilization applies post-crisis consequences:
  - Immediate blood-loss penalty
  - New recovery hediff: `Bloodtype_PostCrisisInjury`

### Forced blood-pack administration (QoL)
- Blood-pack force use is a real doctor job, not instant effect.
- Flow:
  - Select doctor-capable colonist
  - Right-click blood pack
  - Choose `Inject blood pack into...`
  - Target pawn (self-target allowed)

### Blood family interoperability
- Blood family model includes:
  - `Human`
  - `NoBlood`
- Guardrails reject no-blood physiologies (for example mechanoids).
- Extension hooks for mod interoperability:
  - `BloodtypeRaceExtension`
  - `BloodtypeGeneExtension`

## Balance and Presets

Included presets:
- Easy
- Normal
- Hard
- Ultra Brutal

Configurable values include:
- Blood recovery multiplier
- Reaction severity multiplier
- Compatible transfusion toggle
- Extraction blood-loss added
- Extraction max allowed blood loss
- Exact/compatible/incompatible recovery
- Compatible/incompatible reaction severity
- Ultra brutal rescue scale
- Phased crisis tuning:
  - Crisis severity growth rate
  - Stabilization success severity drop
  - Stabilization failure severity gain
  - Post-crisis blood-loss penalty

## Known Next Steps
- Delayed-onset crisis branch is planned but not yet implemented.
- Further stage-specific balancing is ongoing.

## Build

Project target: `net472`

Provide RimWorld managed path via:
- `RimWorldManagedDir` MSBuild property, or
- `RIMWORLD_MANAGED_DIR` environment variable.

Example:

```powershell
dotnet build .\Source\BloodtypeHardcore\BloodtypeHardcore.csproj -p:RimWorldManagedDir="C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed"
```
