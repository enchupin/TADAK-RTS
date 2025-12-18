using System.Collections.Generic;

public static class BuildingDatabase {
    public static readonly Dictionary<string, BuildingData> Data = new Dictionary<string, BuildingData>()
    {
        // {"ID", new BuildingData(종족, 골드, 나무, 건설시간, 체력, 방어력, 인구제공, 생산가능 유닛 리스트)}
        {
            "Human_Barracks",
            new BuildingData("", Race.Human, 150, 0, 30f, 500f, 5, 0, new List<string>{"Footman", "Archer"})
        },
        {
            "Elf_Tree",
            new BuildingData("", Race.Elf, 200, 100, 60f, 800f, 2, 10, new List<string>{"Wisp", "Ent"})
        },
        {
            "Undead_Grave",
            new BuildingData("", Race.Undead, 100, 50, 20f, 400f, 3, 0, new List<string>{"Skeleton"})
        },
        {
            "Beastman_Den",
            new BuildingData("", Race.Beastman, 120, 20, 25f, 600f, 8, 0, new List<string>{"Wolfman"})
        }
    };

          
    public static BuildingData Get(string id) {
        if (Data.TryGetValue(id, out BuildingData building)) {
            return building;
        }

        UnityEngine.Debug.LogError($"Building ID '{id}' Get Failed Error.");
        return null;
    }
}