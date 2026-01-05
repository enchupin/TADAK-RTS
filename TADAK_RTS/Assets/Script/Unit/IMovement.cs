using UnityEngine;

public interface IMovement {
    void Initialize(float speed);

    void MoveTo(Vector3 destination);

    void Stop();

}