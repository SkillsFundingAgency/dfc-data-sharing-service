using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum CurrentLearningStatus
    {
        [Description("In Learning")]
        InLearning = 1,

        [Description("Not In Learning")]
        NotInLearning = 2,

        [Description("Traineeship")]
        Traineeship = 3,

        [Description("Prefer Not To Say")]
        PreferNotToSay = 98,

        [Description("Not Known")]
        NotKnown = 99
    }
}
