using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows the finished-burger pieces sitting on a plate. The prefab holds every
/// ingredient visual already assembled and hidden; this component switches on the
/// one matching each ingredient as it is added.
/// </summary>
public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSOGameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSOGameObject> kitchenObjectSOGameObjectList;

    private void OnEnable()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        // The plate may already hold ingredients (re-enabled, or picked up mid-game),
        // and no event would fire to tell us about those.
        RefreshVisuals();
    }

    private void OnDisable()
    {
        plateKitchenObject.OnIngredientAdded -= PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        SetVisualActive(e.KitchenObjectSO, true);
    }

    private void RefreshVisuals()
    {
        List<KitchenObjectSO> kitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();

        foreach (KitchenObjectSOGameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            if (kitchenObjectSOGameObject.gameObject != null)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(
                    kitchenObjectSOList.Contains(kitchenObjectSOGameObject.kitchenObjectSO));
            }
        }
    }

    private void SetVisualActive(KitchenObjectSO kitchenObjectSO, bool isActive)
    {
        foreach (KitchenObjectSOGameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList)
        {
            if (kitchenObjectSOGameObject.kitchenObjectSO == kitchenObjectSO &&
                kitchenObjectSOGameObject.gameObject != null)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(isActive);
            }
        }
    }
}
