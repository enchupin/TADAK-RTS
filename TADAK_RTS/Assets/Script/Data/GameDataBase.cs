using System.Collections.Generic;
using UnityEngine;

public static class DataManager {
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
    private static readonly Dictionary<string, BaseBuilding> Building_db = new Dictionary<string, BaseBuilding>();
    private static readonly Dictionary<string, UnitJsonData> Unit_db = new Dictionary<string, UnitJsonData>();

    public static void Initialize(string selectedRace) {
        Building_db.Clear();
        Unit_db.Clear();

        LoadBuildings(selectedRace);
        LoadUnits(selectedRace);
    }

    private static void LoadBuildings(string race) {
        var wrapper = DataManager.LoadJson<BuildingDataWrapper>(race, "BuildingDatabase");
        if (wrapper == null || wrapper.buildingDatabase_json == null) return;

        foreach (var item in wrapper.buildingDatabase_json) {
            // 특수 건물 분기 로직 -> 추후 변경 예정
            if (item.Type == "Unit") {
                Building_db.Add(item.ID, new UnitBuildingData(
                    item.ID, item.Type, item.Race, item.Wood, item.Rock,
                    item.MaxHealth, item.BuildTime, item.ProducibleUnits));
            } else if (item.Type == "Resource") {
                Building_db.Add(item.ID, new ResourceBuildingData(
                    item.ID, item.Type, item.Race, item.Wood, item.Rock,
                    item.MaxHealth, item.BuildTime, item.ResourceType));
            }
        }
    }

    private static void LoadUnits(string race) {
        var wrapper = DataManager.LoadJson<UnitDataWrapper>(race, "UnitDatabase");
        if (wrapper == null || wrapper.unitDatabase_json == null) return;

        foreach (var item in wrapper.unitDatabase_json) {
            Unit_db.Add(item.ID, item);
        }
    }

    public static BaseBuilding GetBuilding(string id) => Building_db.GetValueOrDefault(id);
    public static UnitJsonData GetUnit(string id) => Unit_db.GetValueOrDefault(id);
}