using UnityEngine;
using System.Collections.Generic;

public static class BuildingDataBase {
    private static readonly Dictionary<string, BuildingData> Building_db = new Dictionary<string, BuildingData>();

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
                Building_db.Add(item.ID, new UnitBuildingData(
                    item.ID, item.Type, item.Race, item.Wood, item.Rock,item.MaxHealth, item.BuildTime, item.ProducibleUnits));
            } else if (item.Type == "Resource") {
                Building_db.Add(item.ID, new ResourceBuildingData(
                    item.ID, item.Type, item.Race, item.Wood, item.Rock,
                    item.MaxHealth, item.BuildTime, item.ResourceType));
            }
        }
    }

    public static BuildingData Get(string id) => Building_db.GetValueOrDefault(id);
}