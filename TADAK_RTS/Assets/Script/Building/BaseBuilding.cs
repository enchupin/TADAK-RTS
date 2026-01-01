// 건물 데이터 클래스
public abstract class BaseBuilding {
    public string ID;
    public string OwnerID;
    public Race Race;
    public int Wood;
    public int Rock;
    public float MaxHP;
    public float BuildTime;





    protected BaseBuilding(BuildingJsonData data) {
        this.ID = data.ID;
        this.Race = data.Race;
        this.Wood = data.Wood;
        this.Rock = data.Rock;
        this.MaxHP = data.MaxHealth;
        this.BuildTime = data.BuildTime;

        // 추후 this.OwnerName 받아오는 코드 구현

    }




}