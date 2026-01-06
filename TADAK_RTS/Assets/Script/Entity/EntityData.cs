using UnityEngine;

public abstract class EntityData : ScriptableObject {
    public string Name;
    public int MaxHealth;
    public Sprite Icon;
    [TextArea] public string Description;
}

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Data/Building")]
public class BuildingDataSO : EntityData {
    public GameObject Prefab;
    public int ConstructionCost;
}