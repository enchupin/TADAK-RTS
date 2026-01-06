using UnityEngine;
using UnityEngine.AI; 

[RequireComponent(typeof(NavMeshAgent))]
public class GroundMovement : MonoBehaviour, IMovement
{
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(float speed)
    {
        _agent.speed = speed;
        _agent.acceleration = speed * 1.5f; 
        _agent.angularSpeed = 120f; 
    }

    public void MoveTo(Vector3 destination)
    {
        if (_agent.isOnNavMesh)
        { 
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }
    }

    public void Stop()
    {
        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }
}