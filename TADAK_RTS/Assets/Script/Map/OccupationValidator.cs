using UnityEngine;

public interface IPlacementValidator {
    bool IsValid(Vector3 position, string username);
}

public class OccupationValidator : IPlacementValidator {
    public bool IsValid(Vector3 position, string username) {


        // 레이캐스트 등을 통해 해당 위치의 맵 섹터 식별
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, 20f)) {
            var sector = hit.collider.GetComponent<IOccupiable>();

            // 점령 가능한 땅이며 나의 소유인지 확인
            if (sector != null && sector.IsOccupiedBy(username)) {
                return true;
            }
        }

        return true; // 테스트용, 나중에 return false; 로 변경해야함
    }
}