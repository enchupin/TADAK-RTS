
// 자원 건물 인터페이스
public interface IResourceGenerator {
    string ResourceType { get; }
}
// 자원 채취 건물 데이터 클래스
public class ResourceBuildingData : BuildingData, IResourceGenerator {
    public string ResourceType { get; private set; }

    public ResourceBuildingData(string id, string type, Race race, int wood, int rock, float hp, float time, string resType) {
        ID = id; Race = race; Type = type; Wood = wood; Rock = rock; MaxHealth = hp; BuildTime = time;
        ResourceType = resType;
    }
}