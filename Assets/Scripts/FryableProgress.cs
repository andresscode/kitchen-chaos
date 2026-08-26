using UnityEngine;

/// <summary>
/// Remembers how far through its current frying stage a KitchenObject is, so that
/// picking a half-cooked patty up and putting it back down (on this stove or any
/// other) resumes instead of restarting.
///
/// Attach to the prefab of every KitchenObjectSO that appears as the input of a
/// FryingRecipeSO. Objects without it simply never resume.
/// </summary>
public class FryableProgress : MonoBehaviour
{
    private float _progressNormalized;

    /// <summary>
    /// Progress through the current stage, 0 to 1. Stored normalized rather than in
    /// seconds so it stays meaningful if a recipe's fryingTimerMax is retuned.
    /// </summary>
    public float ProgressNormalized
    {
        get => _progressNormalized;
        set => _progressNormalized = Mathf.Clamp01(value);
    }
}
