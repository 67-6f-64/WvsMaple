using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

class goOutWaitingRoom : NpcScript
{
    public override async Task Run()
    {
        AddText("Do you want to leave the waiting room? You can, but the ticket is NOT refundable. Are you sure you still want to leave this room?");
        var yes = await SendYesNo();

        if (yes)
        {
            SetField(101000300);
        }
        else
        {
            AddText("You'll get to your destination in a few. Go ahead and talk to other people, and before you know it, you'll be there already.");
            await SendNext();
        }
    }
}