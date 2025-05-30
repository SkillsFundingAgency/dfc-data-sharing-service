using DSS.ActionPlan.Models;
using System.ComponentModel.DataAnnotations;
using DSS.Models.Interfaces;

namespace DSS.ActionPlans.Interfaces
{
    public interface IValidate
    {
        List<ValidationResult> ValidateResource(IActionPlan resource, DateTime? dateAndTimeSessionCreated);
    }
}