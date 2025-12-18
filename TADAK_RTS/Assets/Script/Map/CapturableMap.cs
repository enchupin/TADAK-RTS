using UnityEngine;
public interface ICapturable {
    CaptureState CurrentState { get; }
    void UpdateCaptureProgress(string capturerUsername, float amount);
    bool IsOccupiedBy(string username);
}

public class CapturableMap : Map, ICapturable {
    public CaptureState CurrentState { get; private set; } = new CaptureState();

    // 로직 클래스 래핑
    private CaptureProcessor _processor = new CaptureProcessor();

    public bool IsOccupiedBy(string username) {
        return CurrentState.State == OccupationState.Occupied && CurrentState.Owner == username;
    }

    public void UpdateCaptureProgress(string capturerUsername, float amount) {
        _processor.Process(CurrentState, capturerUsername, amount);
    }


}