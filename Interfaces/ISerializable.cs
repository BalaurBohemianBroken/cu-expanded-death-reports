namespace BalaurBohemianBroken.Stats;

public interface ISerializable {
    public string Serialize();
    public void Deserialize(string serialized);
}