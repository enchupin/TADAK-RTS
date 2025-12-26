using UnityEngine;

public struct UnitSummary {
    public string Owner;
    public string UnitName;
    public int Count;
}

public abstract class Map : MonoBehaviour {
    [Header("Sector Info")]
    protected string sectorName;
    protected int sectorID;
    protected UnitTracker unitTracker = new UnitTracker();

    public IUnitMeasurable UnitRegistry => unitTracker;
}
