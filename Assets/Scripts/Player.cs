using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter SelectedCounter;
    }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;
    private readonly float rotationSpeed = 10f;
    private Vector2 _inputVector = new();
    private Vector3 _moveDirection = new();
    private Vector3 _lastInteractDirection = new();
    private ClearCounter _selectedCounter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There is more than one Player instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _lastInteractDirection = transform.forward;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnEnable()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void OnDisable()
    {
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleMovement()
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

    private void HandleInteractions()
    {
        if (_moveDirection != Vector3.zero)
        {
            // Keep the last facing direction so we can still interact while standing still.
            _lastInteractDirection = _moveDirection;
        }

        if (Physics.Raycast(
                transform.position + playerRadius * Vector3.up,
                _lastInteractDirection,
                out RaycastHit hit,
                interactDistance,
                countersLayerMask)
            && hit.transform.TryGetComponent(out ClearCounter clearCounter))
        {
            SetSelectedCounter(clearCounter);
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        if (_selectedCounter == selectedCounter)
        {
            return;
        }

        _selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            SelectedCounter = _selectedCounter
        });
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (_selectedCounter != null)
        {
            _selectedCounter.Interact();
        }
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
