using UnityEngine;

public class ClearCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject _kitchenObject;

    public void Interact(Player player)
    {
        if (_kitchenObject == null)
        {
            if (player.HasKitchenObject())
            {
                // Player drops what they carry onto this counter.
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
                kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
            }

            return;
        }

        if (player.HasKitchenObject())
        {
            Debug.Log($"{name} already holds {_kitchenObject.GetKitchenObjectSO().objectName}");
            return;
        }

        // Player picks up what sits on this counter.
        _kitchenObject.SetKitchenObjectParent(player);
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
