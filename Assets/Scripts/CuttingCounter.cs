using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] _cuttingRecipeSOArray;
    private int _cuttingProgress = 0;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                // Player drops what they carry onto this counter.
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _cuttingProgress = 0;
            }

            return;
        }

        if (player.HasKitchenObject())
        {
            Debug.Log($"{name} already holds {GetKitchenObject().GetKitchenObjectSO().objectName}");
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

        _cuttingProgress++;

        if (_cuttingProgress < recipe.cuttingProgressMax)
        {
            return;
        }

        GetKitchenObject().DestroySelf();
        KitchenObject.SpawnKitchenObject(recipe.output, this);
        _cuttingProgress = 0;
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
}
