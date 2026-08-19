using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject _kitchenObject;

    public void Interact()
    {
        if (_kitchenObject != null)
        {
            Debug.Log($"{name} already holds {_kitchenObject.GetKitchenObjectSO().objectName}");
            return;
        }

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
        kitchenObjectTransform.localPosition = Vector3.zero;
        _kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
    }
}
