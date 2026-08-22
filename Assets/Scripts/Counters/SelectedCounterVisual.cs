using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounterVisual : MonoBehaviour
{
    [FormerlySerializedAs("clearCounter")]
    [SerializeField] private BaseCounter baseCounter;

    // Some counters (the container counter, for one) are built from several meshes, so the
    // highlight has to be toggled on every one of them at once.
    [FormerlySerializedAs("selectedVisualGameObject")]
    [SerializeField] private GameObject[] selectedVisualGameObjects;

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
        SetSelectedVisualsActive(e.SelectedCounter == baseCounter);
    }

    private void SetSelectedVisualsActive(bool isActive)
    {
        foreach (GameObject selectedVisualGameObject in selectedVisualGameObjects)
        {
            selectedVisualGameObject.SetActive(isActive);
        }
    }
}
