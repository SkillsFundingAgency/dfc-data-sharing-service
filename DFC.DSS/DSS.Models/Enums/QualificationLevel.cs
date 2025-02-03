using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum QualificationLevel
    {
        [Description("Entry Level")]
        EntryLevel = 0,

        [Description("Level 1")]
        Level1 = 1,

        [Description("Level 2")]
        Level2 = 2,

        [Description("Level 3")]
        Level3 = 3,

        [Description("Level 4")]
        Level4 = 4,

        [Description("Level 5")]
        Level5 = 5,

        [Description("Level 6")]
        Level6 = 6,

        [Description("Level 7")]
        Level7 = 7,

        [Description("Level 8")]
        Level8 = 8,

        [Description("No Qualifications")]
        NoQualifications = 99
    }
}
