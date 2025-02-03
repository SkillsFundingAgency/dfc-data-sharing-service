using System.ComponentModel;

namespace DSS.Models.Enums
{
    public enum Ethnicity
    {
        [Description("English / Welsh / Scottish / Northern Irish / British")]
        EnglishWelshScottishNorthernIrishBritish = 31,

        Irish = 32,

        [Description("Gypsy or Irish Traveller")]
        GypsyIrishTraveller = 33,

        [Description("Any Other White background")]
        AnyOtherWhiteBackground = 34,

        [Description("White and Black Caribbean")]
        WhiteAndBlackCaribbean = 35,

        [Description("White and Black African")]
        WhiteAndBlackAfrican = 36,

        [Description("White and Asian")]
        WhiteAndAsian = 37,

        [Description("Any Other Mixed / multiple ethnic background")]
        AnyOtherMixedMultipleEthnicBackground = 38,

        Indian = 39,
        Pakistani = 40,
        Bangladeshi = 41,
        Chinese = 42,

        [Description("Any other Asian background")]
        AnyOtherAsianBackground = 43,

        African = 44,
        Caribbean = 45,

        [Description("Any other Black / African / Caribbean background")]
        AnyOtherBlackAfricanCaribbeanBackground = 46,

        Arab = 47,

        [Description("Any other ethnic group")]
        AnyOtherEthnicGroup = 98,

        [Description("Not provided")]
        NotProvided = 99
    }
}
