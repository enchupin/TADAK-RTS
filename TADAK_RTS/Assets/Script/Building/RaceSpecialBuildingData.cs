
// 종족 특수 기능을 나타내는 인터페이스
public interface IRacialSpecialty {
    string SpecialEffectDescription { get; }
    float EffectValue { get; }
}

// 종족 특수 건물 데이터 클래스
public class RaceSpecialBuildingData : BaseBuilding, IRacialSpecialty {
    public string SpecialEffectDescription { get; private set; }
    public float EffectValue { get; private set; }

    public RaceSpecialBuildingData(BuildingJsonData data) : base(data) {
        

    }


}