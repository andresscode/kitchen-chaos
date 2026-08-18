using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField] private InputAction playerAction;

    private void Start()
    {
        playerAction.Enable();
    }

    // Update is called once per frame
    public Vector2 GetInputVectorNormalized()
    {
        return playerAction.ReadValue<Vector2>();
    }
}
