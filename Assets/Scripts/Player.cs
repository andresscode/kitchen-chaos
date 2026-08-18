using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    private readonly float rotationSpeed = 10f;
    private Vector2 _inputVector = new();
    private Vector3 _moveDirection = new();

    private void Update()
    {
        _inputVector = gameInput.GetInputVectorNormalized();

        _moveDirection = new(_inputVector.x, 0f, _inputVector.y);

        transform.position += moveSpeed * Time.deltaTime * _moveDirection;
        if (_moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * rotationSpeed);
        }

        _inputVector = new();
    }

    public bool IsWalking()
    {
        return _moveDirection != Vector3.zero;
    }
}
