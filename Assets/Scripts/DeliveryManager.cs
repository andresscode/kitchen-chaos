using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps the list of recipes currently waiting to be delivered: it queues a new random
/// one every few seconds up to a maximum, and checks plates handed in at the
/// DeliveryCounter against that list.
/// </summary>
public class DeliveryManager : MonoBehaviour
{
    /// <summary>The single manager living in the scene.</summary>
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [SerializeField] private int waitingRecipesMax = 4;

    private readonly List<RecipeSO> _waitingRecipeSOList = new();
    private float _spawnRecipeTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_waitingRecipeSOList.Count >= waitingRecipesMax)
        {
            // The order board is full, so the timer stays where it is until one is delivered.
            return;
        }

        _spawnRecipeTimer -= Time.deltaTime;

        if (_spawnRecipeTimer > 0f)
        {
            return;
        }

        _spawnRecipeTimer = spawnRecipeTimerMax;

        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
        _waitingRecipeSOList.Add(waitingRecipeSO);

        Debug.Log($"New waiting recipe: {waitingRecipeSO.recipeName}");
    }

    /// <summary>
    /// Checks a delivered plate against every waiting recipe. The first match is
    /// completed and removed from the list.
    /// </summary>
    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < _waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = _waitingRecipeSOList[i];

            if (!MatchesRecipe(waitingRecipeSO, plateKitchenObject))
            {
                continue;
            }

            _waitingRecipeSOList.RemoveAt(i);

            Debug.Log($"Delivered the correct recipe: {waitingRecipeSO.recipeName}");
            return;
        }

        Debug.Log("Delivered a plate nobody ordered");
    }

    /// <summary>
    /// True when the plate holds exactly the recipe's ingredients, no more and no fewer.
    /// Order does not matter, and a plate never holds the same ingredient twice.
    /// </summary>
    private static bool MatchesRecipe(RecipeSO recipeSO, PlateKitchenObject plateKitchenObject)
    {
        List<KitchenObjectSO> plateKitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();

        if (recipeSO.kitchenObjectSOList.Count != plateKitchenObjectSOList.Count)
        {
            return false;
        }

        foreach (KitchenObjectSO recipeKitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            if (!plateKitchenObjectSOList.Contains(recipeKitchenObjectSO))
            {
                return false;
            }
        }

        return true;
    }
}
