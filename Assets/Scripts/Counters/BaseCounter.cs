using UnityEngine;

/// <summary>
/// Shared behaviour for every counter: it can hold a single KitchenObject on its top point.
/// Subclasses decide what interacting with them actually does.
/// </summary>
public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject _kitchenObject;

    public virtual void Interact(Player player)
    {
        Debug.LogError($"{name} does not implement Interact.");
    }

    public virtual void InteractAlternate(Player player)
    {
        Debug.LogError($"{name} does not implement InteractAlternate.");
    }

    /// <summary>
    /// Moves an ingredient onto a plate when exactly one of the two sides (the counter or
    /// the player's hands) holds a plate that accepts the other side's KitchenObject.
    /// The plated object is destroyed; only the plate keeps track of it.
    /// Both sides must be holding something. Returns true when an ingredient was plated.
    /// </summary>
    protected bool TryPlateIngredient(Player player)
    {
        KitchenObject counterKitchenObject = GetKitchenObject();
        KitchenObject playerKitchenObject = player.GetKitchenObject();

        if (playerKitchenObject is PlateKitchenObject playerPlate)
        {
            if (playerPlate.TryAddIngredient(counterKitchenObject.GetKitchenObjectSO()))
            {
                counterKitchenObject.DestroySelf();
                return true;
            }
        }
        else if (counterKitchenObject is PlateKitchenObject counterPlate)
        {
            if (counterPlate.TryAddIngredient(playerKitchenObject.GetKitchenObjectSO()))
            {
                playerKitchenObject.DestroySelf();
                return true;
            }
        }

        return false;
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }
}
