using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum ReasonForNonAttendance
    {
        [Description("Forgot")]
        Forgot = 1,

        [Description("No Longer Needed")]
        NoLongerNeeded = 2,

        [Description("Work Related")]
        WorkRelated = 3,

        [Description("Personal Situation")]
        PersonalSituation = 4,

        [Description("Rescheduled")]
        Rescheduled = 5,

        [Description("Not Known")]
        NotKnown = 99
    }
}
