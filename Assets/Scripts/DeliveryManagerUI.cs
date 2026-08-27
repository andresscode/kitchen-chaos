using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The on-screen order board: one entry per recipe waiting at the DeliveryManager.
/// The prefab holds a single hidden entry template that gets cloned for every recipe.
/// </summary>
public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake()
    {
        // The template is only a blueprint, it should never be visible itself.
        recipeTemplate.gameObject.SetActive(false);
    }

    // Subscribed in Start rather than OnEnable: DeliveryManager.Instance is assigned in
    // its own Awake, and Unity does not guarantee that runs before this object's OnEnable.
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        // Recipes may already be waiting by the time the UI comes up, and no event
        // would fire to tell us about those.
        UpdateVisual();
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance == null)
        {
            // The manager was already torn down with the scene.
            return;
        }

        DeliveryManager.Instance.OnRecipeSpawned -= DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted -= DeliveryManager_OnRecipeCompleted;
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate)
            {
                continue;
            }

            Destroy(child.gameObject);
        }

        List<RecipeSO> waitingRecipeSOList = DeliveryManager.Instance.GetWaitingRecipeSOList();

        foreach (RecipeSO recipeSO in waitingRecipeSOList)
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
