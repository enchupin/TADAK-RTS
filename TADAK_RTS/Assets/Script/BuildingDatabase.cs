using System.Collections.Generic;

public static class BuildingDatabase {
    private static readonly Dictionary<string, BuildingData> _db = new Dictionary<string, BuildingData>();

    static BuildingDatabase() {
        // 아이디, 종족, int 소모 골드, float 체력, float 방어력, int 인구수 공급량, List<string> 생산 가능 유닛
        Add("Human_Barracks", Race.Human, 150, 500f, 0f, 0, null);
        Add("Elf_Tree", Race.Elf, 200, 800f, 0f, 0, null);
        Add("Orc_Den", Race.Beastman, 120, 600f, 0f, 0, null);





    }

    private static void Add(string id, Race race, int gold, float hp, float armor, float supplyprovided, List<string> producibleUnits) {
        _db.Add(id, new BaseBuildingData(id, race, gold, hp, armor, supplyprovided, producibleUnits));
    }

    public static BuildingData Get(string id) {
        return _db.TryGetValue(id, out var data) ? data : null;
    }
}