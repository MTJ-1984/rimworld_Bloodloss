using Verse;

namespace BloodtypeHardcore
{
    public class BloodtypeModSettings : ModSettings
    {
        public string selectedPreset = "Custom";
        public float bloodRecoveryMultiplier = 0.35f;
        public float reactionSeverityMultiplier = 1.0f;
        public bool allowCompatibleNonIdenticalTransfusions = true;
        public float extractBloodLossAdded = 0.20f;
        public float extractMaxAllowedBloodLoss = 0.45f;
        public float exactMatchBloodLossRecovery = 0.25f;
        public float compatibleBloodLossRecovery = 0.15f;
        public float incompatibleBloodLossRecovery = 0.03f;
        public float compatibleReactionSeverity = 0.12f;
        public float incompatibleReactionSeverity = 0.45f;
        public bool ultraBrutalCriticalMode;
        public float ultraBrutalRescueScale = 0.41f;
        public bool usePhasedCrisisModel = true;
        public float crisisSeverityPerDay = 10.66f;
        public float stabilizeSuccessSeverityDrop = 0.22f;
        public float stabilizeFailureSeverityGain = 0.09f;
        public float postCrisisBloodLoss = 0.55f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref selectedPreset, "selectedPreset", "Custom");
            Scribe_Values.Look(ref bloodRecoveryMultiplier, "bloodRecoveryMultiplier", 0.35f);
            Scribe_Values.Look(ref reactionSeverityMultiplier, "reactionSeverityMultiplier", 1.0f);
            Scribe_Values.Look(ref allowCompatibleNonIdenticalTransfusions, "allowCompatibleNonIdenticalTransfusions", true);
            Scribe_Values.Look(ref extractBloodLossAdded, "extractBloodLossAdded", 0.20f);
            Scribe_Values.Look(ref extractMaxAllowedBloodLoss, "extractMaxAllowedBloodLoss", 0.45f);
            Scribe_Values.Look(ref exactMatchBloodLossRecovery, "exactMatchBloodLossRecovery", 0.25f);
            Scribe_Values.Look(ref compatibleBloodLossRecovery, "compatibleBloodLossRecovery", 0.15f);
            Scribe_Values.Look(ref incompatibleBloodLossRecovery, "incompatibleBloodLossRecovery", 0.03f);
            Scribe_Values.Look(ref compatibleReactionSeverity, "compatibleReactionSeverity", 0.12f);
            Scribe_Values.Look(ref incompatibleReactionSeverity, "incompatibleReactionSeverity", 0.45f);
            Scribe_Values.Look(ref ultraBrutalCriticalMode, "ultraBrutalCriticalMode", false);
            Scribe_Values.Look(ref ultraBrutalRescueScale, "ultraBrutalRescueScale", 0.41f);
            Scribe_Values.Look(ref usePhasedCrisisModel, "usePhasedCrisisModel", true);
            Scribe_Values.Look(ref crisisSeverityPerDay, "crisisSeverityPerDay", 10.66f);
            Scribe_Values.Look(ref stabilizeSuccessSeverityDrop, "stabilizeSuccessSeverityDrop", 0.22f);
            Scribe_Values.Look(ref stabilizeFailureSeverityGain, "stabilizeFailureSeverityGain", 0.09f);
            Scribe_Values.Look(ref postCrisisBloodLoss, "postCrisisBloodLoss", 0.55f);

            Clamp();
            base.ExposeData();
        }

        public void Clamp()
        {
            bloodRecoveryMultiplier = bloodRecoveryMultiplier < 0.05f ? 0.05f : bloodRecoveryMultiplier;
            bloodRecoveryMultiplier = bloodRecoveryMultiplier > 2.0f ? 2.0f : bloodRecoveryMultiplier;
            reactionSeverityMultiplier = reactionSeverityMultiplier < 0.1f ? 0.1f : reactionSeverityMultiplier;
            reactionSeverityMultiplier = reactionSeverityMultiplier > 3.0f ? 3.0f : reactionSeverityMultiplier;

            extractBloodLossAdded = extractBloodLossAdded < 0.01f ? 0.01f : extractBloodLossAdded;
            extractBloodLossAdded = extractBloodLossAdded > 0.80f ? 0.80f : extractBloodLossAdded;

            extractMaxAllowedBloodLoss = extractMaxAllowedBloodLoss < 0.05f ? 0.05f : extractMaxAllowedBloodLoss;
            extractMaxAllowedBloodLoss = extractMaxAllowedBloodLoss > 0.80f ? 0.80f : extractMaxAllowedBloodLoss;

            exactMatchBloodLossRecovery = exactMatchBloodLossRecovery < 0.01f ? 0.01f : exactMatchBloodLossRecovery;
            exactMatchBloodLossRecovery = exactMatchBloodLossRecovery > 0.80f ? 0.80f : exactMatchBloodLossRecovery;

            compatibleBloodLossRecovery = compatibleBloodLossRecovery < 0.0f ? 0.0f : compatibleBloodLossRecovery;
            compatibleBloodLossRecovery = compatibleBloodLossRecovery > 0.80f ? 0.80f : compatibleBloodLossRecovery;

            incompatibleBloodLossRecovery = incompatibleBloodLossRecovery < 0.0f ? 0.0f : incompatibleBloodLossRecovery;
            incompatibleBloodLossRecovery = incompatibleBloodLossRecovery > 0.30f ? 0.30f : incompatibleBloodLossRecovery;

            compatibleReactionSeverity = compatibleReactionSeverity < 0.0f ? 0.0f : compatibleReactionSeverity;
            compatibleReactionSeverity = compatibleReactionSeverity > 1.0f ? 1.0f : compatibleReactionSeverity;

            incompatibleReactionSeverity = incompatibleReactionSeverity < 0.0f ? 0.0f : incompatibleReactionSeverity;
            incompatibleReactionSeverity = incompatibleReactionSeverity > 1.0f ? 1.0f : incompatibleReactionSeverity;

            if (compatibleBloodLossRecovery > exactMatchBloodLossRecovery)
            {
                compatibleBloodLossRecovery = exactMatchBloodLossRecovery;
            }

            if (incompatibleBloodLossRecovery > compatibleBloodLossRecovery)
            {
                incompatibleBloodLossRecovery = compatibleBloodLossRecovery;
            }

            if (incompatibleReactionSeverity < compatibleReactionSeverity)
            {
                incompatibleReactionSeverity = compatibleReactionSeverity;
            }

            ultraBrutalRescueScale = ultraBrutalRescueScale < 0.05f ? 0.05f : ultraBrutalRescueScale;
            ultraBrutalRescueScale = ultraBrutalRescueScale > 1.0f ? 1.0f : ultraBrutalRescueScale;
            crisisSeverityPerDay = crisisSeverityPerDay < 0.5f ? 0.5f : crisisSeverityPerDay;
            crisisSeverityPerDay = crisisSeverityPerDay > 24f ? 24f : crisisSeverityPerDay;
            stabilizeSuccessSeverityDrop = stabilizeSuccessSeverityDrop < 0.01f ? 0.01f : stabilizeSuccessSeverityDrop;
            stabilizeSuccessSeverityDrop = stabilizeSuccessSeverityDrop > 0.95f ? 0.95f : stabilizeSuccessSeverityDrop;
            stabilizeFailureSeverityGain = stabilizeFailureSeverityGain < 0f ? 0f : stabilizeFailureSeverityGain;
            stabilizeFailureSeverityGain = stabilizeFailureSeverityGain > 0.50f ? 0.50f : stabilizeFailureSeverityGain;
            postCrisisBloodLoss = postCrisisBloodLoss < 0f ? 0f : postCrisisBloodLoss;
            postCrisisBloodLoss = postCrisisBloodLoss > 0.80f ? 0.80f : postCrisisBloodLoss;

            if (string.IsNullOrWhiteSpace(selectedPreset))
            {
                selectedPreset = "Custom";
            }
        }

        public void ApplyPresetEasy()
        {
            selectedPreset = "Easy";
            bloodRecoveryMultiplier = 0.75f;
            reactionSeverityMultiplier = 0.35f;
            allowCompatibleNonIdenticalTransfusions = true;
            extractBloodLossAdded = 0.14f;
            extractMaxAllowedBloodLoss = 0.60f;
            exactMatchBloodLossRecovery = 0.30f;
            compatibleBloodLossRecovery = 0.24f;
            incompatibleBloodLossRecovery = 0.15f;
            compatibleReactionSeverity = 0.02f;
            incompatibleReactionSeverity = 0.08f;
            ultraBrutalCriticalMode = false;
            ultraBrutalRescueScale = 1.10f;
            usePhasedCrisisModel = true;
            crisisSeverityPerDay = 3.0f;
            stabilizeSuccessSeverityDrop = 0.45f;
            stabilizeFailureSeverityGain = 0.03f;
            postCrisisBloodLoss = 0.25f;
            Clamp();
        }

        public void ApplyPresetNormal()
        {
            selectedPreset = "Normal";
            bloodRecoveryMultiplier = 0.35f;
            reactionSeverityMultiplier = 1.0f;
            allowCompatibleNonIdenticalTransfusions = true;
            extractBloodLossAdded = 0.20f;
            extractMaxAllowedBloodLoss = 0.45f;
            exactMatchBloodLossRecovery = 0.25f;
            compatibleBloodLossRecovery = 0.15f;
            incompatibleBloodLossRecovery = 0.03f;
            compatibleReactionSeverity = 0.12f;
            incompatibleReactionSeverity = 0.45f;
            ultraBrutalCriticalMode = false;
            ultraBrutalRescueScale = 0.80f;
            usePhasedCrisisModel = true;
            crisisSeverityPerDay = 4.36f;
            stabilizeSuccessSeverityDrop = 0.35f;
            stabilizeFailureSeverityGain = 0.05f;
            postCrisisBloodLoss = 0.35f;
            Clamp();
        }

        public void ApplyPresetHard()
        {
            selectedPreset = "Hard";
            bloodRecoveryMultiplier = 0.22f;
            reactionSeverityMultiplier = 1.3f;
            allowCompatibleNonIdenticalTransfusions = true;
            extractBloodLossAdded = 0.26f;
            extractMaxAllowedBloodLoss = 0.38f;
            exactMatchBloodLossRecovery = 0.22f;
            compatibleBloodLossRecovery = 0.10f;
            incompatibleBloodLossRecovery = 0.01f;
            compatibleReactionSeverity = 0.20f;
            incompatibleReactionSeverity = 0.70f;
            ultraBrutalCriticalMode = false;
            ultraBrutalRescueScale = 0.58f;
            usePhasedCrisisModel = true;
            crisisSeverityPerDay = 6.85f;
            stabilizeSuccessSeverityDrop = 0.28f;
            stabilizeFailureSeverityGain = 0.07f;
            postCrisisBloodLoss = 0.45f;
            Clamp();
        }

        public void ApplyPresetUltraBrutal()
        {
            selectedPreset = "UltraBrutal";
            bloodRecoveryMultiplier = 0.15f;
            reactionSeverityMultiplier = 1.8f;
            allowCompatibleNonIdenticalTransfusions = false;
            extractBloodLossAdded = 0.30f;
            extractMaxAllowedBloodLoss = 0.35f;
            exactMatchBloodLossRecovery = 0.20f;
            compatibleBloodLossRecovery = 0.05f;
            incompatibleBloodLossRecovery = 0.0f;
            compatibleReactionSeverity = 0.35f;
            incompatibleReactionSeverity = 0.90f;
            ultraBrutalCriticalMode = true;
            ultraBrutalRescueScale = 0.41f;
            usePhasedCrisisModel = true;
            crisisSeverityPerDay = 10.66f;
            stabilizeSuccessSeverityDrop = 0.22f;
            stabilizeFailureSeverityGain = 0.09f;
            postCrisisBloodLoss = 0.55f;
            Clamp();
        }
    }
}
