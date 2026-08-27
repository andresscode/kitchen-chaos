using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows one icon per ingredient sitting on the plate. The prefab holds a single
/// hidden icon template that gets cloned for every ingredient added.
/// </summary>
public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        // The template is only a blueprint, it should never be visible itself.
        iconTemplate.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        // The plate may already hold ingredients (re-enabled, or picked up mid-game),
        // and no event would fire to tell us about those.
        UpdateVisual();
    }

    private void OnDisable()
    {
        plateKitchenObject.OnIngredientAdded -= PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child == iconTemplate)
            {
                continue;
            }

            Destroy(child.gameObject);
        }

        List<KitchenObjectSO> kitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();

        foreach (KitchenObjectSO kitchenObjectSO in kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}
