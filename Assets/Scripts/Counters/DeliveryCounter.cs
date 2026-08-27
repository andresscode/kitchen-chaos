/// <summary>
/// Where finished plates are handed in. Only plates are accepted; the DeliveryManager
/// decides whether the plate matches a waiting recipe.
/// </summary>
public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            return;
        }

        if (player.GetKitchenObject() is not PlateKitchenObject plateKitchenObject)
        {
            // Loose ingredients cannot be delivered, only plated ones.
            return;
        }

        DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

        plateKitchenObject.DestroySelf();
    }
}
