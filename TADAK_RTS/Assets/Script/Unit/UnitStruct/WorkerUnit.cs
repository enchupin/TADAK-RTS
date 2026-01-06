using UnityEngine;

public interface IWorkerUnit
{ // 전투 유닛 특성
    void CommandBuild(string buildingID, Vector3 targetPos);

}
public class WorkerUnit : UnitController, IWorkerUnit
{
    private bool _isWorking = false;  
    private Vector3 _buildPos;      
    private string _buildingID;        
    private float _buildRange = 1.5f; 


    private void Update()
    {
        if (_isWorking)
        {
            float distance = Vector3.Distance(transform.position, _buildPos);

            if (distance <= _buildRange)
            {
                CompleteBuild();
            }
        }
    }

    public void CommandBuild(string buildingID, Vector3 targetPos)
    {
        _buildingID = buildingID;
        _buildPos = targetPos;
        _isWorking = true;

        CommandMove(targetPos);

        Debug.Log($"[일꾼] {_buildingID} 건설 명령 수신! 이동 시작...");
    }

    // 도착했을 때 실행
    private void CompleteBuild()
    {
        _isWorking = false; 

        CommandMove(transform.position);

        Debug.Log("[일꾼] 도착! 건설 시작.");

        BuildModeManager.Instance.SpawnBuildingReal(_buildingID, _buildPos);
    }

}
