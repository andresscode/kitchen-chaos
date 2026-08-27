using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single ingredient icon on a plate's icon strip. Spawned by
/// <see cref="PlateIconsUI"/> from a hidden template.
/// </summary>
public class PlateIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.sprite;
    }
}
