using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A plate that collects ingredients. Only the KitchenObjectSOs listed in
/// <see cref="validKitchenObjectSOList"/> are accepted, and each one only once.
/// </summary>
public class PlateKitchenObject : KitchenObject
{
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO KitchenObjectSO;
    }

    /// <summary>Raised when an ingredient is successfully added to the plate.</summary>
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private readonly List<KitchenObjectSO> _kitchenObjectSOList = new();

    /// <summary>
    /// Tries to add an ingredient to the plate. Returns false when the ingredient is
    /// not valid for this plate or is already on it.
    /// </summary>
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // Not something that belongs on a plate (another plate, a raw patty, ...).
            return false;
        }

        if (_kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }

        _kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            KitchenObjectSO = kitchenObjectSO
        });

        return true;
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return _kitchenObjectSOList;
    }
}
