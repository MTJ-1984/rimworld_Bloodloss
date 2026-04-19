using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BloodtypeHardcore
{
    public class BloodTypeGameComponent : GameComponent
    {
        private Dictionary<int, BloodType> bloodTypeByPawnId = new Dictionary<int, BloodType>();
        private List<int> scribedPawnIds = new List<int>();
        private List<BloodType> scribedBloodTypes = new List<BloodType>();

        public BloodTypeGameComponent(Game game)
        {
        }

        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                WriteScribeBuffers();
            }

            Scribe_Collections.Look(ref scribedPawnIds, "pawnIds", LookMode.Value);
            Scribe_Collections.Look(ref scribedBloodTypes, "bloodTypes", LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ReadScribeBuffers();
                EnsureMarkersOnKnownPawns();
            }
        }

        public override void StartedNewGame()
        {
            EnsureMarkersOnKnownPawns();
        }

        public override void LoadedGame()
        {
            EnsureMarkersOnKnownPawns();
        }

        public bool TryGet(Pawn pawn, out BloodType bloodType)
        {
            bloodType = default;
            if (pawn == null)
            {
                return false;
            }

            return bloodTypeByPawnId.TryGetValue(pawn.thingIDNumber, out bloodType);
        }

        public BloodType GetOrAssign(Pawn pawn)
        {
            if (pawn == null)
            {
                return BloodType.OPositive;
            }

            EnsureDefaultBloodProfileGene(pawn);

            int pawnId = pawn.thingIDNumber;
            if (bloodTypeByPawnId.TryGetValue(pawnId, out BloodType existingType))
            {
                EnsureMarkerHediff(pawn, existingType);
                return existingType;
            }

            BloodType assignedType = AssignDeterministicBloodType(pawn);
            bloodTypeByPawnId[pawnId] = assignedType;
            EnsureMarkerHediff(pawn, assignedType);
            return assignedType;
        }

        public void Set(Pawn pawn, BloodType bloodType)
        {
            if (pawn == null)
            {
                return;
            }

            bloodTypeByPawnId[pawn.thingIDNumber] = bloodType;
            EnsureMarkerHediff(pawn, bloodType);
        }

        private BloodType AssignDeterministicBloodType(Pawn pawn)
        {
            uint bucket = StableBucket(pawn);

            if (bucket < 37u)
            {
                return BloodType.OPositive;
            }

            if (bucket < 73u)
            {
                return BloodType.APositive;
            }

            if (bucket < 81u)
            {
                return BloodType.BPositive;
            }

            if (bucket < 88u)
            {
                return BloodType.ONegative;
            }

            if (bucket < 94u)
            {
                return BloodType.ANegative;
            }

            if (bucket < 97u)
            {
                return BloodType.ABPositive;
            }

            if (bucket < 99u)
            {
                return BloodType.BNegative;
            }

            return BloodType.ABNegative;
        }

        private static uint StableBucket(Pawn pawn)
        {
            unchecked
            {
                uint hash = 2166136261;
                hash = (hash ^ (uint)pawn.thingIDNumber) * 16777619;

                if (pawn.kindDef != null)
                {
                    hash = (hash ^ (uint)pawn.kindDef.shortHash) * 16777619;
                }

                return hash % 100u;
            }
        }

        private void WriteScribeBuffers()
        {
            scribedPawnIds.Clear();
            scribedBloodTypes.Clear();

            foreach (KeyValuePair<int, BloodType> kvp in bloodTypeByPawnId)
            {
                scribedPawnIds.Add(kvp.Key);
                scribedBloodTypes.Add(kvp.Value);
            }
        }

        private void ReadScribeBuffers()
        {
            bloodTypeByPawnId.Clear();

            if (scribedPawnIds == null || scribedBloodTypes == null)
            {
                return;
            }

            int count = scribedPawnIds.Count < scribedBloodTypes.Count ? scribedPawnIds.Count : scribedBloodTypes.Count;
            for (int i = 0; i < count; i++)
            {
                bloodTypeByPawnId[scribedPawnIds[i]] = scribedBloodTypes[i];
            }
        }

        private void EnsureMarkersOnKnownPawns()
        {
            List<Pawn> pawns = PawnsFinder.AllMapsWorldAndTemporary_Alive;
            for (int i = 0; i < pawns.Count; i++)
            {
                Pawn pawn = pawns[i];
                if (pawn == null)
                {
                    continue;
                }

                if (!BloodProfileResolver.UsesHumanTyping(pawn))
                {
                    continue;
                }

                GetOrAssign(pawn);
            }
        }

        private static void EnsureMarkerHediff(Pawn pawn, BloodType bloodType)
        {
            if (pawn?.health == null)
            {
                return;
            }

            if (!BloodProfileResolver.UsesHumanTyping(pawn))
            {
                RemoveAllMarkers(pawn);
                return;
            }

            HediffDef desired = MarkerForType(bloodType);
            if (desired == null)
            {
                return;
            }

            if (pawn.health.hediffSet.GetFirstHediffOfDef(desired) == null)
            {
                pawn.health.AddHediff(HediffMaker.MakeHediff(desired, pawn));
            }

            RemoveAllMarkersExcept(pawn, desired);
        }

        private static void RemoveAllMarkers(Pawn pawn)
        {
            RemoveAllMarkersExcept(pawn, null);
        }

        private static void RemoveAllMarkersExcept(Pawn pawn, HediffDef keep)
        {
            List<Hediff> toRemove = new List<Hediff>();
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
            {
                Hediff hediff = pawn.health.hediffSet.hediffs[i];
                if (!IsMarker(hediff?.def))
                {
                    continue;
                }

                if (keep != null && hediff.def == keep)
                {
                    continue;
                }

                toRemove.Add(hediff);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                pawn.health.RemoveHediff(toRemove[i]);
            }
        }

        private static bool IsMarker(HediffDef def)
        {
            return def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_O_Positive ||
                   def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_O_Negative ||
                   def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_A_Positive ||
                   def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_A_Negative ||
                   def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_B_Positive ||
                   def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_B_Negative ||
                   def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_AB_Positive ||
                   def == BloodtypeMarkerHediffDefOf.BloodtypeMarker_AB_Negative;
        }

        private static HediffDef MarkerForType(BloodType bloodType)
        {
            switch (bloodType)
            {
                case BloodType.OPositive:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_O_Positive;
                case BloodType.ONegative:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_O_Negative;
                case BloodType.APositive:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_A_Positive;
                case BloodType.ANegative:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_A_Negative;
                case BloodType.BPositive:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_B_Positive;
                case BloodType.BNegative:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_B_Negative;
                case BloodType.ABPositive:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_AB_Positive;
                case BloodType.ABNegative:
                    return BloodtypeMarkerHediffDefOf.BloodtypeMarker_AB_Negative;
                default:
                    return null;
            }
        }

        private static void EnsureDefaultBloodProfileGene(Pawn pawn)
        {
            if (pawn?.genes == null || BloodtypeGeneDefOf.BloodtypeHumanProfile == null)
            {
                return;
            }

            if (pawn.genes.HasActiveGene(BloodtypeGeneDefOf.BloodtypeHumanProfile))
            {
                return;
            }

            if (BloodtypeGeneDefOf.BloodtypeNoBloodProfile != null &&
                pawn.genes.HasActiveGene(BloodtypeGeneDefOf.BloodtypeNoBloodProfile))
            {
                return;
            }

            if (BloodTypeService.GetBloodFamily(pawn) != BloodFamily.Human)
            {
                return;
            }

            if (!BloodTypeService.CanUseHumanTypedBlood(pawn))
            {
                return;
            }

            pawn.genes.AddGene(BloodtypeGeneDefOf.BloodtypeHumanProfile, false);
        }
    }
}
