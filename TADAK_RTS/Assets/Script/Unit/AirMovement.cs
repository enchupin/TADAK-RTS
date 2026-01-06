using UnityEngine;

public class AirMovement : MonoBehaviour, IMovement
{
    private float _moveSpeed;
    private Vector3 _targetPos;
    private bool _isMoving = false;

    [Header("비행 설정")]
    [SerializeField] private float _flyHeight = 1.4f;

    private float _turnSpeed = 10f;

    public void Initialize(float speed)
    {
        _moveSpeed = speed;
    }

    public void MoveTo(Vector3 destination)
    {
        _targetPos = new Vector3(destination.x, _flyHeight, destination.z);
        _isMoving = true;
    }

    public void Stop()
    {
        _isMoving = false;
    }

    private void Update()
    {
        if (!_isMoving) return;

        Vector3 lookTarget = new Vector3(_targetPos.x, transform.position.y, _targetPos.z);

        Vector3 dir = lookTarget - transform.position;
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
        }

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