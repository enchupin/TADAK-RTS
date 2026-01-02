using System.Collections.Generic;


// 유닛 해금 기능을 나타내는 인터페이스
public interface IUnitUnlocker {
    List<string> UnlockedUnitNames { get; }
}


// 유닛 해금 건물 데이터 클래스
public class TechBuildingData : BuildingController, IUnitUnlocker {
    public List<string> UnlockedUnitNames { get; private set; }

}