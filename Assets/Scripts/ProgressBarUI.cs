using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject barVisual;
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressGameObject;

    private IHasProgress _hasProgress;

    private void Start()
    {
        _hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

        if (_hasProgress == null)
        {
            Debug.LogError($"{hasProgressGameObject.name} does not implement IHasProgress.");
            return;
        }

        _hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void OnDestroy()
    {
        if (_hasProgress != null)
        {
            _hasProgress.OnProgressChanged -= HasProgress_OnProgressChanged;
        }
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0 || e.progressNormalized == 1f)
        {
            Hide();
            return;
        }

        Show();
    }

    private void Hide()
    {
        barVisual.SetActive(false);
    }

    private void Show()
    {
        barVisual.SetActive(true);
    }
}
