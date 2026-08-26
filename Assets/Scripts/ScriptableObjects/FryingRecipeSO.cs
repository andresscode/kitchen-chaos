using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen Chaos/Frying Recipe")]
public class FryingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax;
}
