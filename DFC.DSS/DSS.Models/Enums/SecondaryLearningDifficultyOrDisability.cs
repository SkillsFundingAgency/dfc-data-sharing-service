using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum SecondaryLearningDifficultyOrDisability
    {
        [Description("Visual impairment")]
        VisualImpairment = 4,

        [Description("Hearing impairment")]
        HearingImpairment = 5,

        [Description("Disability affecting mobility")]
        DisabilityAffectingMobility = 6,

        [Description("Profound complex disabilities")]
        ProfoundComplexDisabilities = 7,

        [Description("Social and emotional difficulties")]
        SocialAndEmotionalDifficulties = 8,

        [Description("Mental health difficulty")]
        MentalHealthDifficulty = 9,

        [Description("Moderate learning difficulty")]
        ModerateLearningDifficulty = 10,

        [Description("Severe learning difficulty")]
        SevereLearningDifficulty = 11,

        Dyslexia = 12,
        Dyscalculia = 13,

        [Description("Autism spectrum disorder")]
        AutismSpectrumDisorder = 14,

        [Description("Asperger's syndrome")]
        AspergersSyndrome = 15,

        [Description("Temporary disability after illness (for example post viral) or accident")]
        TemporaryDisabilityAfterIllnessOrAccident = 16,

        [Description("Speech, Language and Communication Needs")]
        SpeechLanguageAndCommunicationNeeds = 17,

        [Description("Other physical disability")]
        OtherPhysicalDisability = 93,

        [Description("Other specific learning difficulty (e.g. Dyspraxia)")]
        OtherSpecificLearningDifficulty = 94,

        [Description("Other medical condition (for example epilepsy, asthma, diabetes)")]
        OtherMedicalCondition = 95,

        [Description("Other learning difficulty")]
        OtherLearningDifficulty = 96,

        [Description("Other disability")]
        OtherDisability = 97,

        [Description("Prefer not to say")]
        PreferNotToSay = 98,

        [Description("Not provided")]
        NotProvided = 99
    }
}
