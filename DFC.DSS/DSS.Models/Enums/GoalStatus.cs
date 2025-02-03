using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum GoalStatus
    {
        [Description("In progress")]
        InProgress = 1,
        
        Achieved = 2,
        
        [Description("No longer relevant")]
        NoLongerRelevant = 99
    }
}
