using System;
using System.Collections.Generic;

namespace BalaurBohemianBroken.Stats;

public interface IStat : ISerializable {
    // These are the instances of IStat that interact with patches.
    public static Dictionary<Type, IStat> runningRegister = new();
    public static T Get<T>() where T : IStat {
        return (T)runningRegister[typeof(T)];
    }
    
    public string name { get; }
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