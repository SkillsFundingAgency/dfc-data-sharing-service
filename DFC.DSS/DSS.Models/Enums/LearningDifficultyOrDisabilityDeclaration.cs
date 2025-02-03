using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum LearningDifficultyOrDisabilityDeclaration
    {
        [Description("Customer considers themselves to have a learning difficulty and/or health problem")]
        CustomerConsidersThemselvesToHaveALearningDifficultyAndOrHealthProblem = 1,

        [Description("Customer does not consider themselves to have a learning difficulty and/or health problem")]
        CustomerDoesNotConsiderThemselvesToHaveALearningDifficultyAndOrHealthProblem = 2,

        [Description("Not provided by the customer")]
        NotProvidedByTheCustomer = 9
    }
}
