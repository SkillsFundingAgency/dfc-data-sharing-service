using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum Gender
    {
        [Description("Female")]
        Female = 1,

        [Description("Male")]
        Male = 2,

        [Description("Not Applicable")]
        NotApplicable = 3,

        [Description("Another Gender")]
        AnotherGender = 4,

        [Description("Prefer not to say")]
        PreferNotToSay = 5,

        [Description("Not Provided")]
        NotProvided = 99
    }
}
