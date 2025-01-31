using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum ActionType
    {
        [Description("Skills Health Check")]
        SkillsHealthCheck = 1,

        [Description("Create or update CV")]
        CreateOrUpdateCV = 2,

        [Description("Interview Skills Workshop")]
        InterviewSkillsWorkshop = 3,

        [Description("Search For Vacancy")]
        SearchForVacancy = 4,

        [Description("Enrol On A Course")]
        EnrolOnACourse = 5,

        [Description("Careers Management Workshop")]
        CareersManagementWorkshop = 6,

        [Description("Apply For Apprenticeship")]
        ApplyForApprenticeship = 7,

        [Description("Apply For Traineeship")]
        ApplyForTraineeship = 8,

        [Description("Attend Skills Fair Or Skills Show")]
        AttendSkillsFairOrSkillsShow = 9,

        Volunteer = 10,

        [Description("Use National Careers Service Website")]
        UseNationalCareersServiceWebsite = 11,

        [Description("Use External Digital Services")]
        UseExternalDigitalServices = 12,

        [Description("Book Follow Up Appointment")]
        BookFollowUpAppointment = 13,

        [Description("Use Social Media")]
        UseSocialMedia = 14,

        [Description("Discovery Your Skills And Careers")]
        DiscoveryYourSkillsAndCareers = 15,

        [Description("Find A Course")]
        FindACourse = 16,

        Other = 99
    }
}
