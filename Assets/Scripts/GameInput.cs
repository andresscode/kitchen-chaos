using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    [SerializeField] private InputAction playerAction;
    [SerializeField] private InputAction interactAction;
    [SerializeField] private InputAction interactAlternateAction;

    private void OnEnable()
    {
        playerAction.Enable();

        interactAction.performed += InteractAction_performed;
        interactAction.Enable();

        interactAlternateAction.performed += InteractAlternateAction_performed;
        interactAlternateAction.Enable();
    }

    private void OnDisable()
    {
        playerAction.Disable();

        interactAction.performed -= InteractAction_performed;
        interactAction.Disable();

        interactAlternateAction.performed -= InteractAlternateAction_performed;
        interactAlternateAction.Disable();
    }

    private void InteractAction_performed(InputAction.CallbackContext context)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }


    private void InteractAlternateAction_performed(InputAction.CallbackContext context)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVector()
    {
        return playerAction.ReadValue<Vector2>();
    }
}
