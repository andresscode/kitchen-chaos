using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter SelectedCounter;
    }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    private const float InputDeadzone = 0.1f;
    private const float SlideDeadzone = 0.3f;
    private readonly float rotationSpeed = 10f;
    private Vector2 _inputVector = new();
    private Vector3 _moveDirection = new();
    private Vector3 _lastInteractDirection = new();
    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

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
        _inputVector = gameInput.GetInputVector();

        // The keyboard composite always reports a unit vector, but a gamepad stick reports
        // any magnitude between 0 and 1. Keep that magnitude as a speed multiplier and work
        // with a unit direction, so the slide-along-a-counter logic below can swap the
        // direction without also changing how fast we travel.
        float inputMagnitude = Mathf.Clamp01(_inputVector.magnitude);

        if (inputMagnitude < InputDeadzone)
        {
            _moveDirection = Vector3.zero;
            return;
        }

        _moveDirection = new Vector3(_inputVector.x, 0f, _inputVector.y).normalized;

        float moveDistance = moveSpeed * inputMagnitude * Time.deltaTime;
        bool canMove = CanMove(_moveDirection, moveDistance);

        if (!canMove)
        {
            // Blocked head on, try sliding along the X axis only. Ignore a barely tilted axis
            // so a near-vertical stick push does not snap into a full speed sideways slide.
            Vector3 moveDirectionX = Mathf.Abs(_moveDirection.x) > SlideDeadzone
                ? new Vector3(Mathf.Sign(_moveDirection.x), 0f, 0f)
                : Vector3.zero;
            canMove = moveDirectionX != Vector3.zero && CanMove(moveDirectionX, moveDistance);

            if (canMove)
            {
                _moveDirection = moveDirectionX;
            }
            else
            {
                // Still blocked, try sliding along the Z axis only.
                Vector3 moveDirectionZ = Mathf.Abs(_moveDirection.z) > SlideDeadzone
                    ? new Vector3(0f, 0f, Mathf.Sign(_moveDirection.z))
                    : Vector3.zero;
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

        transform.forward = Vector3.Slerp(transform.forward, _moveDirection, Time.deltaTime * rotationSpeed);
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
            && hit.transform.TryGetComponent(out BaseCounter baseCounter))
        {
            SetSelectedCounter(baseCounter);
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
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
            _selectedCounter.Interact(this);
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

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }
}
