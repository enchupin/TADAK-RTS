using System.Collections.Generic;
using UnityEngine;

public static class DataManager {
    public const string BUILDING_DATABASE = "BuildingDatabase";
    public const string UNIT_DATABASE = "UnitDatabase";
    public static T LoadJson<T>(string race, string dataType) where T : class {
        // ex) Human_Building_json, Human_Unit_json
        string fileName = $"{race}_{dataType}_json";
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile == null) {
            Debug.LogError($"{fileName} 파일을 찾을 수 없습니다.");
            return null;
        }

        return JsonUtility.FromJson<T>(jsonFile.text);
    }
}

public static class GameDataBase {
    private static readonly Dictionary<string, BuildingJsonData> Building_db = new Dictionary<string, BuildingJsonData>();
    private static readonly Dictionary<string, UnitJsonData> Unit_db = new Dictionary<string, UnitJsonData>();

    public static void Initialize(string selectedRace) { // 어디선가 최초 1회 호출해야함
        Building_db.Clear();
        Unit_db.Clear();

        LoadBuildings(selectedRace);
        LoadUnits(selectedRace);
    }

    private static void LoadBuildings(string race) {
        var wrapper = DataManager.LoadJson<BuildingDataWrapper>(race, DataManager.BUILDING_DATABASE);
        if (wrapper == null || wrapper.buildingDatabase_json == null) return;

        foreach (var item in wrapper.buildingDatabase_json) {
                Building_db.Add(item.ID, item);
        }
    }

    private static void LoadUnits(string race) {
        var wrapper = DataManager.LoadJson<UnitDataWrapper>(race, DataManager.UNIT_DATABASE);
        if (wrapper == null || wrapper.unitDatabase_json == null) return;

        foreach (var item in wrapper.unitDatabase_json) {
            Unit_db.Add(item.ID, item);
        }
    }

    public static BuildingJsonData GetBuilding(string id) {
        if (Building_db == null || Building_db.Count == 0) {
            Debug.LogWarning($"[GameDataBase] Building_db가 비어있습니다! ID: {id}를 찾을 수 없습니다.");
        }

        if (Building_db.TryGetValue(id, out var data)) {
            return data;
        }

        Debug.LogWarning($"[GameDataBase] ID: {id} 에 해당하는 건물 데이터가 DB에 없습니다.");
        return null;
    }


    public static UnitJsonData GetUnit(string id) {
        if (Unit_db == null || Unit_db.Count == 0) {
            Debug.LogError($"[GameDataBase] Unit_db가 비어있습니다! ID: {id}를 찾을 수 없습니다.");
            return null;
        }

        if (Unit_db.TryGetValue(id, out var data)) {
            return data;
        }

        Debug.LogWarning($"[GameDataBase] ID: {id} 에 해당하는 유닛 데이터가 DB에 없습니다.");
        return null;
    }
}