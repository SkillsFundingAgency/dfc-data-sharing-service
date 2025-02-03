using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum EmploymentHours
    {
        [Description("Less Than Sixteen Hours")]
        LessThanSixteenHours = 1,

        [Description("Sixteen Hours Or More")]
        SixteenHoursOrMore = 2,

        [Description("Prefer Not To Say")]
        PreferNotToSay = 98,

        [Description("Not Known")]
        NotKnown = 99
    }
}
