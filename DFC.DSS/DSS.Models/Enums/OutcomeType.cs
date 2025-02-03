using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum OutcomeType
    {
        [Description("Customer Satisfaction")]
        CustomerSatisfaction = 1,

        [Description("Career Management")]
        CareersManagement = 2,

        [Description("Sustainable Employment")]
        SustainableEmployment = 3,

        [Description("Accredited Learning")]
        AccreditedLearning = 4,

        [Description("Career Progression")]
        CareerProgression = 5
    }
}
