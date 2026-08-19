using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private GameObject selectedVisualGameObject;

    private void Start()
    {
        // Subscribe in Start so the Player singleton is guaranteed to be assigned.
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        selectedVisualGameObject.SetActive(e.SelectedCounter == clearCounter);
    }
}
