using Verse;

namespace BloodtypeHardcore
{
    public static class BloodTypeService
    {
        public static BloodFamily GetBloodFamily(Pawn pawn)
        {
            return BloodProfileResolver.ResolveFamily(pawn);
        }

        public static bool CanUseHumanTypedBlood(Pawn pawn)
        {
            return BloodProfileResolver.UsesHumanTyping(pawn);
        }

        public static BloodType GetOrAssign(Pawn pawn)
        {
            if (!CanUseHumanTypedBlood(pawn))
            {
                return BloodType.OPositive;
            }

            BloodTypeGameComponent component = Current.Game?.GetComponent<BloodTypeGameComponent>();
            if (component == null)
            {
                return BloodType.OPositive;
            }

            return component.GetOrAssign(pawn);
        }

        public static bool TryGet(Pawn pawn, out BloodType bloodType)
        {
            if (!CanUseHumanTypedBlood(pawn))
            {
                bloodType = default;
                return false;
            }

            BloodTypeGameComponent component = Current.Game?.GetComponent<BloodTypeGameComponent>();
            if (component == null)
            {
                bloodType = default;
                return false;
            }

            return component.TryGet(pawn, out bloodType);
        }

        public static void Set(Pawn pawn, BloodType bloodType)
        {
            if (!CanUseHumanTypedBlood(pawn))
            {
                return;
            }

            BloodTypeGameComponent component = Current.Game?.GetComponent<BloodTypeGameComponent>();
            component?.Set(pawn, bloodType);
        }

        public static string GetLabel(Pawn pawn)
        {
            return CanUseHumanTypedBlood(pawn)
                ? BloodTypeCompatibility.ToLabel(GetOrAssign(pawn))
                : "N/A";
        }
    }
}
