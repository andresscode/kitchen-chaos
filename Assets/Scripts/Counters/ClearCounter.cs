using UnityEngine;

/// <summary>
/// A plain surface: the player can drop a KitchenObject here or pick one back up.
/// It never spawns anything on its own.
/// </summary>
public class ClearCounter : BaseCounter
{
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
            if (!TryPlateIngredient(player))
            {
                Debug.Log($"{name} already holds {GetKitchenObject().GetKitchenObjectSO().objectName}");
            }

            return;
        }

        // Player picks up what sits on this counter.
        GetKitchenObject().SetKitchenObjectParent(player);
    }
}
