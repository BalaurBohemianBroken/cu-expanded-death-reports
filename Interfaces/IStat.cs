namespace BalaurBohemianBroken.Stats;

public interface IStat : ISerializable {
    // Should assign to a static variable that can be accessed from patches.
    // This is the version of the IStat that is currently tracking data.
    // I'd prefer this to be an abstract static to remove the middleman,
    // but game's runtime doesn't allow it I don't think.
    public IStat runningInstance { get; set; }
    // TODO: delete this. supersceded by noteworthiness.
    public int priority { get; }
    public string name { get; }
    // TODO: What this field should be identified as on the report.
    public string fieldName { get; }

    // Noteworthiness guide:
    // > 1 - Not noteworthy. Do not show.
    // 1 - Something happened here, but barely.
    // 3 - It happened a lot, but it could still happen in any run.
    // 5 - This stands out. It probably has a footnote to comment on it.
    // 10 - This is unique. It probably has a major note to comment on it.
    public float Noteworthiness();

    public string GetStatReadout(bool color);

    public void Reset();
}