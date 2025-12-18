using UnityEngine;
using System.Collections.Generic;

public static class BuildingDataBase {
    private static readonly Dictionary<string, BuildingData> _db = new Dictionary<string, BuildingData>();

    static BuildingDataBase() {
        LoadDataFromJson();
    }

    private static void LoadDataFromJson() {
        // Resources 폴더에서 BuildingDatabase_json.json 파일을 읽어옴
        TextAsset jsonFile = Resources.Load<TextAsset>("BuildingDatabase_json");
        if (jsonFile == null) return;

        BuildingDataWrapper wrapper = JsonUtility.FromJson<BuildingDataWrapper>(jsonFile.text);

        foreach (var item in wrapper.buildings) {
            if (item.Type == "Unit") {
                _db.Add(item.ID, new UnitBuildingData(
                    item.ID, item.Race, item.Type, item.Wood, item.Rock,item.MaxHealth, item.BuildTime, item.ProducibleUnits));
            } else if (item.Type == "Resource") {
                _db.Add(item.ID, new ResourceBuildingData(
                    item.ID, item.Race, item.Type, item.Wood, item.Rock,
                    item.MaxHealth, item.BuildTime, item.ResourceType, item.AmountPerTick));
            }
        }
    }

    public static BuildingData Get(string id) => _db.GetValueOrDefault(id);
}