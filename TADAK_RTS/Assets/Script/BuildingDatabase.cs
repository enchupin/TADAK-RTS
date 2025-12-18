using System.Collections.Generic;

public static class BuildingDatabase {
    private static readonly Dictionary<string, BuildingData> _db = new Dictionary<string, BuildingData>();

    static BuildingDatabase() {
        // 아이디, 종족, int 소모 목재, int 소모 석재, float 체력, 건설시간
        Add("Human_Barracks", Race.Human, 150, 150, 500f, 15f);
        Add("Elf_Tree", Race.Elf, 200, 200, 800f, 0f);
        Add("Orc_Den", Race.Beastman, 120, 120, 600f, 0f);





    }

    private static void Add(string id, Race race, int wood, int rock, float hp, float buildTime) {
        _db.Add(id, new BaseBuildingData(id, race, wood, rock, hp, buildTime));
    }

    public static BuildingData Get(string id) {
        return _db.TryGetValue(id, out var data) ? data : null;
    }
}