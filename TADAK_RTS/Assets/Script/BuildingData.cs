using System.Collections.Generic;

public enum Race { Human, Elf, Beastman, Undead }

public class BuildingData {
    public string Name;
    public Race Race;
    public int Gold;
    public int Wood;
    public float BuildTime;
    public float MaxHealth;
    public int Armor;
    public int SupplyProvided;
    public List<string> ProducibleUnits;

    // »ý¼ºÀÚ
    public BuildingData(string name, Race race, int gold, int wood, float time, float health, int armor, int supply, List<string> units) {
        Name = name;
        Race = race;
        Gold = gold;
        Wood = wood;
        BuildTime = time;
        MaxHealth = health;
        Armor = armor;
        SupplyProvided = supply;
        ProducibleUnits = units;
    }
}