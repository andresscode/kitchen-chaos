using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen Chaos/Recipe")]
public class RecipeSO: ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;
}
