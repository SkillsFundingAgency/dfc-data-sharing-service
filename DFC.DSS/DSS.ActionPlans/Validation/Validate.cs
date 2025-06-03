using DSS.ActionPlans.Interfaces;
using DSS.Models.Enums;
using DSS.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DSS.ActionPlan.Validation
{
    public class Validate : IValidate
    {
        public List<ValidationResult> ValidateResource(IActionPlan resource, DateTime? dateAndTimeSessionCreated)
        {
            var context = new ValidationContext(resource, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(resource, context, results, true);
            ValidateActionPlanRules(resource, results, dateAndTimeSessionCreated);

            return results;
        }

        private void ValidateActionPlanRules(IActionPlan actionPlanResource, List<ValidationResult> results, DateTime? dateAndTimeSessionCreated)
        {
            if (actionPlanResource == null)
                return;

            if (actionPlanResource.DateActionPlanCreated.HasValue)
            {
                if (actionPlanResource.DateActionPlanCreated.Value > DateTime.UtcNow)
                    results.Add(new ValidationResult("Date ActionPlan Created must be less the current date/time", new[] { "DateActionPlanCreated" }));

                if (dateAndTimeSessionCreated.HasValue)
                {
                    if (!(actionPlanResource.DateActionPlanCreated.Value >= dateAndTimeSessionCreated.Value))
                        results.Add(new ValidationResult("Date ActionPlan Created must be greater than Date And Time Session Created", new[] { "DateActionPlanCreated" }));
                }
            }

            if (actionPlanResource.DateAndTimeCharterShown.HasValue)
            {
                if (actionPlanResource.DateAndTimeCharterShown.Value > DateTime.UtcNow)
                    results.Add(new ValidationResult("Date And Time Charter Shown must be less the current date/time", new[] { "DateAndTimeCharterShown" }));
            }

            if (actionPlanResource.DateActionPlanSentToCustomer.HasValue)
            {
                if (actionPlanResource.DateActionPlanSentToCustomer.Value > DateTime.UtcNow)
                    results.Add(new ValidationResult("Date ActionPlan Sent To Customer must be less the current date/time", new[] { "DateActionPlanSentToCustomer" }));

                if (!(actionPlanResource.DateActionPlanSentToCustomer.Value >= actionPlanResource.DateActionPlanCreated.GetValueOrDefault()))
                    results.Add(new ValidationResult("Date ActionPlan Sent To Customer must be greater than Date Action Plan Created", new[] { "DateActionPlanSentToCustomer" }));
            }

            if (actionPlanResource.DateActionPlanAcknowledged.HasValue)
            {
                if (actionPlanResource.DateActionPlanAcknowledged.Value > DateTime.UtcNow)
                    results.Add(new ValidationResult("Date ActionPlan Acknowledged must be less the current date/time", new[] { "DateActionPlanAcknowledged" }));

                if (!(actionPlanResource.DateActionPlanAcknowledged.Value >= actionPlanResource.DateActionPlanCreated.GetValueOrDefault()))
                    results.Add(new ValidationResult("Date ActionPlan Acknowledged must be greater than Date Action Plan Created", new[] { "DateActionPlanAcknowledged" }));
            }

            if (actionPlanResource.LastModifiedDate.HasValue && actionPlanResource.LastModifiedDate.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Last Modified Date must be less the current date/time", new[] { "LastModifiedDate" }));

            if (actionPlanResource.ActionPlanDeliveryMethod.HasValue && !Enum.IsDefined(typeof(ActionPlanDeliveryMethod), actionPlanResource.ActionPlanDeliveryMethod.Value))
                results.Add(new ValidationResult("Please supply a valid Action Plan Delivery Method", new[] { "ActionPlanDeliveryMethod" }));

            if (actionPlanResource.CustomerSatisfaction.HasValue && !Enum.IsDefined(typeof(CustomerSatisfaction), actionPlanResource.CustomerSatisfaction.Value))
                results.Add(new ValidationResult("Please supply a valid Customer Satisfaction result", new[] { "CustomerSatisfaction" }));

        }
    }
}