using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum ReasonForTermination
    {
        [Description("Customer Choice")]
        CustomerChoice = 1,

        [Description("Deceased")]
        Deceased = 2,

        [Description("Duplicate")]
        Duplicate = 3,

        [Description("Other")]
        Other = 99
    }
}
