namespace BloodtypeHardcore
{
    public static class BloodTypeCompatibility
    {
        public static bool IsExactMatch(BloodType donor, BloodType recipient)
        {
            return donor == recipient;
        }

        public static bool IsCompatible(BloodType donor, BloodType recipient)
        {
            AboGroup donorAbo = ToAbo(donor);
            AboGroup recipientAbo = ToAbo(recipient);
            bool donorRhPositive = IsRhPositive(donor);
            bool recipientRhPositive = IsRhPositive(recipient);

            bool aboCompatible = donorAbo == AboGroup.O ||
                                 recipientAbo == AboGroup.AB ||
                                 donorAbo == recipientAbo;
            bool rhCompatible = !donorRhPositive || recipientRhPositive;
            return aboCompatible && rhCompatible;
        }

        public static string ToLabel(BloodType bloodType)
        {
            switch (bloodType)
            {
                case BloodType.OPositive:
                    return "O+";
                case BloodType.ONegative:
                    return "O-";
                case BloodType.APositive:
                    return "A+";
                case BloodType.ANegative:
                    return "A-";
                case BloodType.BPositive:
                    return "B+";
                case BloodType.BNegative:
                    return "B-";
                case BloodType.ABPositive:
                    return "AB+";
                case BloodType.ABNegative:
                    return "AB-";
                default:
                    return "Unknown";
            }
        }

        private static AboGroup ToAbo(BloodType bloodType)
        {
            switch (bloodType)
            {
                case BloodType.OPositive:
                case BloodType.ONegative:
                    return AboGroup.O;
                case BloodType.APositive:
                case BloodType.ANegative:
                    return AboGroup.A;
                case BloodType.BPositive:
                case BloodType.BNegative:
                    return AboGroup.B;
                case BloodType.ABPositive:
                case BloodType.ABNegative:
                    return AboGroup.AB;
                default:
                    return AboGroup.O;
            }
        }

        private static bool IsRhPositive(BloodType bloodType)
        {
            switch (bloodType)
            {
                case BloodType.OPositive:
                case BloodType.APositive:
                case BloodType.BPositive:
                case BloodType.ABPositive:
                    return true;
                default:
                    return false;
            }
        }

        private enum AboGroup
        {
            O,
            A,
            B,
            AB
        }
    }
}
