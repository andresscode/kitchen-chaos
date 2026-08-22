using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen Chaos/Cutting Recipe")]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
}
