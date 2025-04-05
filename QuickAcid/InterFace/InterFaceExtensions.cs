namespace QuickAcid.InterFace;


public static class InterFaceExtensions
{
    public static SpecBuilder Spec(string label) => new SpecBuilder(label);
}
