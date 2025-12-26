

public interface ICaptureProcessor {

    const float MaxProgressRate = 100f;
    string CapturingUserName { get; set; }
    float CurrentProgressRate { get; set; }
    void Process(OccupationState state, float captureStat);

    void CancelProcess();
    bool IsCapturable();
}

public class CaptureProcessor : ICaptureProcessor {
    public string CapturingUserName { get; set; }
    public float CurrentProgressRate { get; set; }
    public void Process(OccupationState state, float captureStat) {
        if (!IsCapturable()) // 본인이 점령 가능하면
            return;

    }

    public void CancelProcess() {

    
    
    }

    public bool IsCapturable() {

        // 본인의 유닛만 있을 때 true 아니면 false
        return true;
    }



}