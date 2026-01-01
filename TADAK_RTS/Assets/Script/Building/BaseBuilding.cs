// 건물 데이터 클래스
public abstract class BaseBuilding : IOwnable {
    public string ID { get; protected set; }
    public Race Race { get; protected set; }
    public int Wood { get; protected set; }
    public int Rock { get; protected set; }
    public float MaxHealth { get; protected set; }
    public float BuildTime { get; protected set; }

    public string OwnerName { get; set; }

    protected BaseBuilding(BuildingJsonData data) {
        this.ID = data.ID;
        this.Race = data.Race;
        this.Wood = data.Wood;
        this.Rock = data.Rock;
        this.MaxHealth = data.MaxHealth;
        this.BuildTime = data.BuildTime;

        // 추후 this.OwnerName 받아오는 코드 구현

    }







    public bool IsOwnedBy(string username) {

        // 소유자 이름이랑 일치하면 true를 반환하도록 수정 예정
        return true;
    }


}