using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    [SerializeField] private CuttingRecipeSO[] _cuttingRecipeSOArray;
    private int _cuttingProgress = 0;

    public event EventHandler OnCut;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                // Player drops what they carry onto this counter.
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _cuttingProgress = 0;
                NotifyCuttingProgressChange(0.0f);
            }

            return;
        }

        if (player.HasKitchenObject())
        {
            if (!CanPlayerPickUpKitchenObject() || !TryPlateIngredient(player))
            {
                Debug.Log($"{name} already holds {GetKitchenObject().GetKitchenObjectSO().objectName}");
            }

            return;
        }

        if (CanPlayerPickUpKitchenObject())
        {
            // Player picks up what sits on this counter.
            GetKitchenObject().SetKitchenObjectParent(player);
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (!HasKitchenObject() || player.HasKitchenObject())
        {
            return;
        }

        KitchenObjectSO input = GetKitchenObject().GetKitchenObjectSO();

        if (!TryGetCuttingRecipe(input, out CuttingRecipeSO recipe))
        {
            Debug.Log($"{input.objectName} cannot be cut");
            return;
        }

        if (recipe.cuttingProgressMax < 1)
        {
            Debug.LogError($"{recipe.input.objectName} cuttingProgressMax is less than 1");
            return;
        }

        _cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);

        NotifyCuttingProgressChange((float)_cuttingProgress / recipe.cuttingProgressMax);

        if (_cuttingProgress < recipe.cuttingProgressMax)
        {
            return;
        }

        GetKitchenObject().DestroySelf();
        KitchenObject.SpawnKitchenObject(recipe.output, this);
        _cuttingProgress = 0;
        NotifyCuttingProgressChange(0.0f);
    }

    private bool TryGetCuttingRecipe(KitchenObjectSO input, out CuttingRecipeSO recipe)
    {
        foreach (CuttingRecipeSO candidate in _cuttingRecipeSOArray)
        {
            if (candidate != null && candidate.input == input)
            {
                recipe = candidate;
                return true;
            }
        }

        recipe = null;
        return false;
    }

    private bool CanPlayerPickUpKitchenObject()
    {
        // A cut that is underway must be finished before the object can leave the counter.
        return _cuttingProgress == 0;
    }

    private void NotifyCuttingProgressChange(float newValue)
    {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = newValue
        });
    }
}
