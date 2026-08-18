using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    private readonly float rotationSpeed = 10f;
    private Vector2 _inputVector = new();
    private Vector3 _moveDirection = new();

    private void Update()
    {
        _inputVector = gameInput.GetInputVectorNormalized();

        _moveDirection = new(_inputVector.x, 0f, _inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = CanMove(_moveDirection, moveDistance);

        if (!canMove)
        {
            // Blocked head on, try sliding along the X axis only.
            Vector3 moveDirectionX = new Vector3(_moveDirection.x, 0f, 0f).normalized;
            canMove = moveDirectionX != Vector3.zero && CanMove(moveDirectionX, moveDistance);

            if (canMove)
            {
                _moveDirection = moveDirectionX;
            }
            else
            {
                // Still blocked, try sliding along the Z axis only.
                Vector3 moveDirectionZ = new Vector3(0f, 0f, _moveDirection.z).normalized;
                canMove = moveDirectionZ != Vector3.zero && CanMove(moveDirectionZ, moveDistance);

                if (canMove)
                {
                    _moveDirection = moveDirectionZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDistance * _moveDirection;
        }

        if (_moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * rotationSpeed);
        }

        _inputVector = new();
    }

    private bool CanMove(Vector3 moveDirection, float moveDistance)
    {
        return !Physics.CapsuleCast(
            transform.position + playerRadius * Vector3.up,
            transform.position + playerHeight * Vector3.up,
            playerRadius,
            moveDirection,
            moveDistance);
    }

    public bool IsWalking()
    {
        return _moveDirection != Vector3.zero;
    }
}
