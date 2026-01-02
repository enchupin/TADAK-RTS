
// 자원 건물 인터페이스
public interface IResourceGenerator {
    string ResourceType { get; }
}
// 자원 채취 건물 데이터 클래스
public class ResourceBuildingData : BuildingController, IResourceGenerator {
    public string ResourceType { get; private set; }



}