using System.Collections.Generic;
using UnityEngine;

public class CaptureProcessor {

    private const float MAX_PROGRESS_RATE = 100f;
    private const float CAPTURE_TIME_SECONDS = 10f; // 점령까지 걸리는 시간
    private const float CAPTURE_SPEED = MAX_PROGRESS_RATE / CAPTURE_TIME_SECONDS; // 초당 점령 수치

    private CapturableMap _capturableMap;
    public CaptureProcessor (CapturableMap capturableMap) { _capturableMap = capturableMap; }
    public OccupationState State {
        get => _capturableMap.State;
        set => _capturableMap.State = value;
    }
    private List<string> _unitsInRange = new List<string>();
    public float ProgressRate;
    public string CapturingUserName;

    public void UpdateCaptureProgress() {
        if (!IsCapturable()) { // 본인이 점령 가능하지 않다면
            return;
        }

        ProgressRate += CAPTURE_SPEED * Time.deltaTime;

        if (ProgressRate >= MAX_PROGRESS_RATE) {
            State = OccupationState.Occupied;
            _capturableMap.OwnerName = CapturingUserName;
        }
    }

    public void CancelProcess() {
        ProgressRate = 0f;
        State = OccupationState.Neutral;
    }

    public bool IsCapturable() {



        // 본인의 유닛만 있을 때 true 아니면 false
        return true;
    }


    public void OnUnitCountChanged() { // 유닛 변경 파악
        string candidate = DetermineCapturer();

        if (candidate != null) {
            // 점령 주체가 바뀌었을 때만 딱 한 번 설정
            CapturingUserName = candidate;
        } else {
            // 아무도 없으면 중단 로직 실행
            CancelProcess();
        }
    }

    string DetermineCapturer() { // 점령 주체 파악

        return null;
    }


}