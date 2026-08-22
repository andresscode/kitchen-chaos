using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] _cuttingRecipeSOArray;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                // Player drops what they carry onto this counter.
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }

            return;
        }

        if (player.HasKitchenObject())
        {
            Debug.Log($"{name} already holds {GetKitchenObject().GetKitchenObjectSO().objectName}");
            return;
        }

        // Player picks up what sits on this counter.
        GetKitchenObject().SetKitchenObjectParent(player);
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && !player.HasKitchenObject())
        {
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            if (outputKitchenObjectSO == null)
            {
                Debug.Log($"Invalid cutting recipe for input {GetKitchenObject().GetKitchenObjectSO().objectName}");
                return;
            }

            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in _cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }

        return null;
    }
}
