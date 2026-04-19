using Verse;

namespace BloodtypeHardcore
{
    public static class BloodPackUtility
    {
        public static bool TryGetBloodTypeFromThingDef(ThingDef def, out BloodType bloodType)
        {
            bloodType = default;
            if (def == null)
            {
                return false;
            }

            if (def == BloodtypeThingDefOf.BloodPack_O_Positive) { bloodType = BloodType.OPositive; return true; }
            if (def == BloodtypeThingDefOf.BloodPack_O_Negative) { bloodType = BloodType.ONegative; return true; }
            if (def == BloodtypeThingDefOf.BloodPack_A_Positive) { bloodType = BloodType.APositive; return true; }
            if (def == BloodtypeThingDefOf.BloodPack_A_Negative) { bloodType = BloodType.ANegative; return true; }
            if (def == BloodtypeThingDefOf.BloodPack_B_Positive) { bloodType = BloodType.BPositive; return true; }
            if (def == BloodtypeThingDefOf.BloodPack_B_Negative) { bloodType = BloodType.BNegative; return true; }
            if (def == BloodtypeThingDefOf.BloodPack_AB_Positive) { bloodType = BloodType.ABPositive; return true; }
            if (def == BloodtypeThingDefOf.BloodPack_AB_Negative) { bloodType = BloodType.ABNegative; return true; }

            return false;
        }
    }
}
