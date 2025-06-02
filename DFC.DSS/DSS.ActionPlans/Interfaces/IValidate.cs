using DSS.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DSS.ActionPlans.Interfaces
{
    public interface IValidate
    {
        List<ValidationResult> ValidateResource(IActionPlan resource, DateTime? dateAndTimeSessionCreated);
    }
}