using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    public static class BloodProfileResolver
    {
        public static BloodFamily ResolveFamily(Pawn pawn)
        {
            if (pawn == null)
            {
                return BloodFamily.NoBlood;
            }

            if (pawn.RaceProps?.IsMechanoid == true)
            {
                return BloodFamily.NoBlood;
            }

            if (TryResolveFromGene(pawn, out BloodFamily geneFamily))
            {
                return geneFamily;
            }

            BloodtypeRaceExtension raceExtension = pawn.def?.GetModExtension<BloodtypeRaceExtension>();
            if (raceExtension != null && raceExtension.overrideBloodFamily)
            {
                return raceExtension.bloodFamily;
            }

            return pawn.RaceProps?.Humanlike == true ? BloodFamily.Human : BloodFamily.NoBlood;
        }

        public static bool UsesHumanTyping(Pawn pawn)
        {
            if (ResolveFamily(pawn) != BloodFamily.Human)
            {
                return false;
            }

            if (TryResolveHumanTypingFromGene(pawn, out bool geneUsesHumanTyping))
            {
                return geneUsesHumanTyping;
            }

            BloodtypeRaceExtension raceExtension = pawn.def?.GetModExtension<BloodtypeRaceExtension>();
            if (raceExtension != null)
            {
                return raceExtension.usesHumanBloodTyping;
            }

            return true;
        }

        private static bool TryResolveFromGene(Pawn pawn, out BloodFamily bloodFamily)
        {
            bloodFamily = default;
            if (pawn.genes == null)
            {
                return false;
            }

            for (int i = 0; i < pawn.genes.GenesListForReading.Count; i++)
            {
                Gene gene = pawn.genes.GenesListForReading[i];
                BloodtypeGeneExtension extension = gene?.def?.GetModExtension<BloodtypeGeneExtension>();
                if (extension != null && extension.overrideBloodFamily)
                {
                    bloodFamily = extension.bloodFamily;
                    return true;
                }
            }

            return false;
        }

        private static bool TryResolveHumanTypingFromGene(Pawn pawn, out bool usesHumanTyping)
        {
            usesHumanTyping = true;
            if (pawn.genes == null)
            {
                return false;
            }

            for (int i = 0; i < pawn.genes.GenesListForReading.Count; i++)
            {
                Gene gene = pawn.genes.GenesListForReading[i];
                BloodtypeGeneExtension extension = gene?.def?.GetModExtension<BloodtypeGeneExtension>();
                if (extension != null)
                {
                    usesHumanTyping = extension.usesHumanBloodTyping;
                    return true;
                }
            }

            return false;
        }
    }
}
