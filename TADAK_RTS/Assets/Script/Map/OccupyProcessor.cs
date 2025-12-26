using System.Collections.Generic;
using UnityEngine;

public class OccupyProcessor {

    private const float MAX_PROGRESS_RATE = 100f;
    private const float OCCUPY_TIME_SECONDS = 10f; // 점령까지 걸리는 시간
    private const float OCCUPY_SPEED = MAX_PROGRESS_RATE / OCCUPY_TIME_SECONDS; // 초당 점령 수치

    private OccupiableMap _occupiableMap;
    public OccupyProcessor (OccupiableMap occupiableMap) { _occupiableMap = occupiableMap; }
    public OccupationState State {
        get => _occupiableMap.State;
        set => _occupiableMap.State = value;
    }
    public float ProgressRate;
    public string OccupyingUserName;

    public void UpdateCaptureProgress() {
        if (!IsOccupiable()) { // 본인이 점령 가능하지 않다면
            return;
        }

        ProgressRate += OCCUPY_SPEED * Time.deltaTime;

        if (ProgressRate >= MAX_PROGRESS_RATE) {
            State = OccupationState.Occupied;
            _occupiableMap.OwnerName = OccupyingUserName;
        }
    }

    public void CancelProcess() {
        ProgressRate = 0f;
        State = OccupationState.Neutral;
    }

    public bool IsOccupiable() {



        // 본인의 유닛만 있을 때 true 아니면 false
        return true;
    }


    public void OnUnitCountChanged() { // 유닛 변경 파악
        string candidate = DetermineCapturer();

        if (candidate != null) {
            // 점령 주체가 바뀌었을 때만 딱 한 번 설정
            OccupyingUserName = candidate;
        } else {
            // 아무도 없으면 중단 로직 실행
            CancelProcess();
        }
    }

    string DetermineCapturer() { // 점령 주체 파악

        return null;
    }


}