using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum LengthOfUnemployment
    {
        [Description("Less Than Three Months")]
        LessThanThreeMonths = 1,

        [Description("Three To Five Months")]
        ThreeToFiveMonths = 2,

        [Description("Six To Eleven Months")]
        SixToElevenMonths = 3,

        [Description("Twelve To Twenty Three Months")]
        TwelveToTwentyThreeMonths = 4,

        [Description("Twenty Four To Thirty Five Months")]
        TwentyFourToThirtyFiveMonths = 5,

        [Description("Over Thirty Six Months")]
        OverThirtySixMonths = 6,

        [Description("Prefer Not To Say")]
        PreferNotToSay = 98,

        [Description("Not Known Or Not Provided")]
        NotKnownOrNotProvided = 99
    }
}
