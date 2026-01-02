using UnityEngine;

public class AirMovement : MonoBehaviour, IMovement
{
    private float _moveSpeed;
    private Vector3 _targetPos;
    private bool _isMoving = false;

    [Header("공중 유닛 설정")]
    [SerializeField] private float _flyHeight = 6.0f; 

    public void Initialize(float speed)
    {
        _moveSpeed = speed;
    }

    public void MoveTo(Vector3 destination)
    {
        _targetPos = new Vector3(destination.x, _flyHeight, destination.z);
        _isMoving = true;

        transform.LookAt(_targetPos);
    }

    public void Stop()
    {
        _isMoving = false;
    }

    private void Update()
    {
        if (!_isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPos,
            _moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _targetPos) < 0.1f)
        {
            Stop();
        }
    }
}