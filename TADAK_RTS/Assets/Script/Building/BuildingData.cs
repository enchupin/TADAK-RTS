
public enum Race { Human, Elf, Beastman, Undead }

// 건물 데이터 클래스
public abstract class BuildingData {
    public string ID { get; protected set; }
    public string Type { get; protected set; }
    public Race Race { get; protected set; }
    public int Wood { get; protected set; }
    public int Rock { get; protected set; }
    public float MaxHealth { get; protected set; }
    public float BuildTime { get; protected set; }


}


public class BaseBuildingData : BuildingData {
    public BaseBuildingData(string id, string type, Race race,  int wood, int rock, float hp, float buildTime) {
        ID = id; Type = type; Race = race; Wood = wood; Rock = rock; MaxHealth = hp; BuildTime = buildTime;
    }
}
