using DSS.Models.Interfaces;
using DSS.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DSS.ActionPlan.Models
{

    public class ActionPlanPatch : IActionPlan
    {
        [Display(Description = "Unique identifier to the related session resource. " +
                               "This will need to be provided the first time on a Patch Request for V2")]
        [Example(Description = "2730af9c-fc34-4c2b-a905-c4b584b0f379")]
        public Guid? SessionId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time action plan was created.")]
        [Example(Description = "2018-06-20T21:45:00")]
        public DateTime? DateActionPlanCreated { get; set; }

        [Display(Description = "Customer has seen the customer charter.")]
        [Example(Description = "true")]
        public bool? CustomerCharterShownToCustomer { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time the customer was shown the customer charter.")]
        [Example(Description = "2018-06-20T21:45:00")]
        public DateTime? DateAndTimeCharterShown { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time the action plan was sent (or made available) to the customer.")]
        [Example(Description = "2018-06-21T13:32:00")]
        public DateTime? DateActionPlanSentToCustomer { get; set; }

        [Display(Description = "Action Plan Delivery Method reference data.")]
        [Example(Description = "1")]
        public ActionPlanDeliveryMethod? ActionPlanDeliveryMethod { get; set; }

        [Display(Description = "Date and time the customer acknowledged receipt of the action plan.")]
        [Example(Description = "2018-06-22T07:55:00")]
        public DateTime? DateActionPlanAcknowledged { get; set; }

        [Display(Description = "Summary of a customer current situation and how it affects their career.")]
        [StringLength(4000)]
        [Example(Description = "this is some text")]
        public string CurrentSituation { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time of the last modification to the record.")]
        [Example(Description = "2018-06-20T13:45:00")]
        public DateTime? LastModifiedDate { get; set; }

        [StringLength(10, MinimumLength = 10)]
        [Display(Description = "Identifier of the touchpoint who made the last change to the record")]
        [Example(Description = "0000000001")]

        public string LastModifiedTouchpointId { get; set; }

        [StringLength(50)]
        [Display(Description = "Identifier supplied by the touchpoint to indicate their subcontractor")]
        [Example(Description = "01234567899876543210")]
        public string SubcontractorId { get; set; }

        [Display(Description = "Indentifier of customer satisfaction levels")]
        [Example(Description = "yes/no/not complete")]
        public CustomerSatisfaction? CustomerSatisfaction { get; set; }

        public void SetDefaultValues()
        {
            if (!LastModifiedDate.HasValue)
                LastModifiedDate = DateTime.UtcNow;

            if (!CustomerSatisfaction.HasValue)
                CustomerSatisfaction = null;
        }

        public void SetIds(string touchpointId, string subcontractorId)
        {
            LastModifiedTouchpointId = touchpointId;
            SubcontractorId = subcontractorId;
        }

    }
}
