using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum SignpostedToCategory
    {
        [Description("Charity")]
        Charity = 1,

        [Description("Training Provider")]
        TrainingProvider = 3,

        [Description("Apprenticeships")]
        ApprenticeshipService = 6,

        [Description("Specialist Organisation")]
        SpecialistOrganisation = 7,

        JCP = 8,
        Employer = 9,

        [Description("Traineeships")]
        Traineeships = 10,

        [Description("Further Education")]
        FurtherEducation = 11,

        [Description("Higher Education")]
        HigherEducation = 12,

        [Description("Community Centres")]
        CommunityCentres = 13,

        [Description("Not Applicable")]
        NotApplicable = 98,

        Other = 99
    }
}
