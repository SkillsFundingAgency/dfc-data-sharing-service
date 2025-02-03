using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum CurrentEmploymentStatus
    {
        [Description("Apprenticeship")]
        Apprenticeship = 1,

        [Description("Economically Inactive")]
        EconomicallyInactive = 2,

        [Description("Economically Inactive And Voluntary Work")]
        EconomicallyInactiveAndVoluntaryWork = 3,

        [Description("Employed")]
        Employed = 4,

        [Description("Employed And Voluntary Work")]
        EmployedAndVoluntaryWork = 5,

        [Description("Employed At Risk Of Redundancy")]
        EmployedAtRiskOfRedundancy = 6,

        [Description("Retired")]
        Retired = 7,

        [Description("Retired And Voluntary Work")]
        RetiredAndVoluntaryWork = 8,

        [Description("Self Employed")]
        SelfEmployed = 9,

        [Description("Self Employed And Voluntary Work")]
        SelfEmployedAndVoluntaryWork = 10,

        [Description("Unemployed")]
        Unemployed = 11,

        [Description("Unemployed And Voluntary Work")]
        UnemployedAndVoluntaryWork = 12,

        [Description("Unemployed Due To Redundancy")]
        UnemployedDueToRedundancy = 13,

        [Description("NotKnown")]
        NotKnown = 99
    }
}
