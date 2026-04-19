namespace Censeq.FeatureManagement;

public static class FeatureDefinitionRecordConsts
{
    public static int MaxNameLength { get; set; } = 128;
    public static int MaxDisplayNameLength { get; set; } = 256;
    public static int MaxLocalizationKeyLength { get; set; } = 512;
    public static int MaxDescriptionLength { get; set; } = 256;
    public static int MaxDefaultValueLength { get; set; } = 256;
    public static int MaxAllowedProvidersLength { get; set; } = 256;
    public static int MaxValueTypeLength { get; set; } = 2048;
}
