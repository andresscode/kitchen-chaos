using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen Chaos/Recipe List")]
public class RecipeListSO: ScriptableObject
{
    public List<RecipeSO> recipeSOList;
}
