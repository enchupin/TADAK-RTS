public enum OccupationState { Neutral, Capturing, Occupied }

public class CaptureState {
    public string Owner { get; set; } = "";
    public float CaptureProgress { get; set; } = 0f;
    public OccupationState State { get; set; } = OccupationState.Neutral;
}

