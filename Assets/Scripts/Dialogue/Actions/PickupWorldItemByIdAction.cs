using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Pickup World Item By ID")]
public class PickupWorldItemByIdAction : DialogueAction
{
    public string worldItemId;

    public override void Execute()
    {
        var item = WorldItemRegistry.Instance.Get(worldItemId);

        if (item != null)
        {
            item.Pickup();
        }
    }
}