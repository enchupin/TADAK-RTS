public class CaptureProcessor {
    private const float MAXPROGRESS = 100f;

    public void Process(CaptureState state, string capturer, float amount) {
        if (state.State == OccupationState.Occupied && state.Owner == capturer) return;

        // 점령 진행도 증가
        state.CaptureProgress += amount;
        state.State = OccupationState.Capturing;

        // 완료
        if (state.CaptureProgress >= MAXPROGRESS) {
            state.Owner = capturer;
            state.State = OccupationState.Occupied;
            state.CaptureProgress = MAXPROGRESS;
        }
    }
}