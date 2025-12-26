using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class OccupyProcessor {

    private const float MAX_PROGRESS_RATE = 100f;
    private const float OCCUPY_TIME_SECONDS = 15f; // 점령까지 걸리는 시간
    private const float OCCUPY_SPEED = MAX_PROGRESS_RATE / OCCUPY_TIME_SECONDS; // 초당 점령 수치

    private OccupiableMap _occupiableMap;
    
    public OccupancyState State {
        get => _occupiableMap.State;
        set => _occupiableMap.State = value;
    }
    public float ProgressRate;
    public string OccupyingUserName;

    private UnitTracker unitTracker = new UnitTracker();
    public IUnitMeasurable UnitRegistry => unitTracker;

    public OccupyProcessor(OccupiableMap occupiableMap) {
        _occupiableMap = occupiableMap;
        unitTracker.OnRegistryChanged += HandleUnitCountChanged;
    }
    private Coroutine occupyRoutine;


    private void HandleUnitCountChanged() { // 맵의 유닛 수 변경

        string candidate = GetSingleWorkerOwner();

        if (candidate != null) { // 점령 진행
            if (OccupyingUserName != candidate) {
                OccupyingUserName = candidate;
                ProgressRate = 0f;
            }

            // 점령 루프가 실행 중이 아니라면 시작
            if (occupyRoutine == null && State != OccupancyState.Occupied) {
                occupyRoutine = _occupiableMap.StartCoroutine(OccupyRoutine());
            }
        } else { // 점령 중단
            StopOccupyRoutine();

            // 일꾼이 아예 없으면 초기화
            if (!unitTracker.UnitsInRange.Any(u => u is IWorkerUnit)) {
                CancelProcess();
            }
        }
    }

    private string GetSingleWorkerOwner() {
        var workers = unitTracker.UnitsInRange.Where(u => u is IWorkerUnit).ToList();
        var owners = workers.Select(u => u.OwnerName).Distinct().ToList();
        return (owners.Count == 1) ? owners[0] : null;
    }

    private IEnumerator OccupyRoutine() {

        while (ProgressRate < MAX_PROGRESS_RATE) {
            State = OccupancyState.Occupying;
            ProgressRate += OCCUPY_SPEED * Time.deltaTime;

            // 점령 완료 시
            if (ProgressRate >= MAX_PROGRESS_RATE) {
                ProgressRate = MAX_PROGRESS_RATE;
                State = OccupancyState.Occupied;
                _occupiableMap.OwnerName = OccupyingUserName;
                break;
            }
            yield return null; // 다음 프레임까지 대기
        }

        occupyRoutine = null;
    }

    private void StopOccupyRoutine() {
        if (occupyRoutine != null) {
            _occupiableMap.StopCoroutine(occupyRoutine);
            occupyRoutine = null;
        }
    }

    public void CancelProcess() {
        ProgressRate = 0f;
        OccupyingUserName = null;
        State = OccupancyState.Neutral;
    }

}