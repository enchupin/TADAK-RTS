using NUnit.Framework;
using System.Diagnostics;
using System.Collections.Generic;

public enum Race { Human, Elf, Beastman, Undead }


public abstract class BuildingData {
    public string ID { get; protected set; }
    public Race Race { get; protected set; }
    public int Gold { get; protected set; }
    public float MaxHealth { get; protected set; }
    public float BuildTime {get; protected set; }

    public float Armor {get; protected set; }
    public float SupplyProvided {get; protected set; }
    public List<string> ProducibleUnits { get; protected set; }


}


public class BaseBuildingData : BuildingData {
    public BaseBuildingData(string id, Race race, int gold, float hp, float armor, float supplyprovided, List<string> producibleUnits) {
        ID = id; Race = race; Gold = gold; MaxHealth = hp; Armor = armor; SupplyProvided = supplyprovided; ProducibleUnits = producibleUnits;
    }
}