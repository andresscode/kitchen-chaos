using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject barVisual;
    [SerializeField] private Image barImage;
    [SerializeField] private CuttingCounter cuttingCounter;

    private void Start()
    {
        cuttingCounter.OnProgressChanged += CuttingCounter_OnProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void OnDestroy()
    {
        cuttingCounter.OnProgressChanged -= CuttingCounter_OnProgressChanged;
    }

    private void CuttingCounter_OnProgressChanged(object sender, CuttingCounter.OnProgressChangedEventArgs e)
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
