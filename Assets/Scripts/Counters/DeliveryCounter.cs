public class DeliveryCounter : BaseCounter
{

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject() && player.GetKitchenObject() is PlateKitchenObject)
        {
            player.GetKitchenObject().DestroySelf();
        }
    }
}