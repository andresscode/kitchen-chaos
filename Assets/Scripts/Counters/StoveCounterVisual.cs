using UnityEngine;

/// <summary>
/// Turns the stove's "on" look (burner glow + sizzling particles) on and off as the
/// counter moves between its frying states. Purely cosmetic: it never drives cooking.
/// </summary>
public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject sizzlingParticlesGameObject;

    private void OnEnable()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;

        // The counter may already be cooking when this visual is re-enabled,
        // and no state change would fire to tell us about it.
        SetVisualsActive(IsStoveOn(stoveCounter.GetState()));
    }

    private void OnDisable()
    {
        stoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        SetVisualsActive(IsStoveOn(e.state));
    }

    /// <summary>
    /// The burner shows as lit while something is still changing on it. A burned
    /// object has nowhere left to go, so the stove goes dark again.
    /// </summary>
    private static bool IsStoveOn(StoveCounter.State state)
    {
        return state is StoveCounter.State.Frying or StoveCounter.State.Fried;
    }

    private void SetVisualsActive(bool isOn)
    {
        if (stoveOnGameObject != null)
        {
            stoveOnGameObject.SetActive(isOn);
        }

        if (sizzlingParticlesGameObject != null)
        {
            sizzlingParticlesGameObject.SetActive(isOn);
        }
    }
}
