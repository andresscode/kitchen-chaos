using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;

    private Vector2 _inputVector = new();
    private Vector3 _moveDirection = new();

    private void Update()
    {
        if (Input.GetKey(KeyCode.W)) _inputVector.y += 1;
        if (Input.GetKey(KeyCode.S)) _inputVector.y -= 1;
        if (Input.GetKey(KeyCode.A)) _inputVector.x -= 1;
        if (Input.GetKey(KeyCode.D)) _inputVector.x += 1;

        _inputVector = _inputVector.normalized;

        _moveDirection = new (_inputVector.x, 0f, _inputVector.y); 
        transform.position += moveSpeed * Time.deltaTime * _moveDirection;

        _inputVector = new();
    }
}
