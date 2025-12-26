public abstract class BaseUnit : IOwnable {
    public string OwnerName { get; set; }
    public bool IsOwnedBy(string name) {

        return true; // 본인 소유일 때만 true 반환하도록 수정
    }



    // 유닛 관련 프로퍼티 추가

    // 예시
    // float MaxHelath { get; set; }
    // float MaxMana { get; set; }

}