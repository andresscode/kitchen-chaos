using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;

    [SerializeField] private InputAction playerAction;
    [SerializeField] private InputAction interactAction;

    private void OnEnable()
    {
        playerAction.Enable();

        interactAction.performed += InteractAction_performed;
        interactAction.Enable();
    }

    private void OnDisable()
    {
        playerAction.Disable();

        interactAction.performed -= InteractAction_performed;
        interactAction.Disable();
    }

    private void InteractAction_performed(InputAction.CallbackContext context)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVectorNormalized()
    {
        return playerAction.ReadValue<Vector2>();
    }
}
