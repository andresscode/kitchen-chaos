using System;
using UnityEngine;

/// <summary>
/// An ingredient dispenser: interacting with it spawns a KitchenObject straight into the
/// player's hands. Nothing is ever placed on the counter itself.
/// </summary>
public class ContainerCounter : BaseCounter
{
    /// <summary>Raised when the player successfully takes an object out of this counter.</summary>
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // Hands are full, there is nowhere to put a freshly spawned object.
            return;
        }

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
