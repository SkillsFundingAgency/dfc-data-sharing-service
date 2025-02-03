using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum CustomerSatisfaction
    {
        [Description("Yes")]
        Satisfied = 1,

        [Description("No")]
        NotSatisfied = 2,

        [Description("N/A")]
        NotAvailable = 99
    }
}
