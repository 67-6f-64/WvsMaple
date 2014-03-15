using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

class get_ticket : NpcScript
{
    public override async Task Run()
    {
        var evt = GetEvent("ShipOssyria");
        var ticket = 4031045;
        var board = 101000301;

        if (evt == null)
        {
            AddText("It looks like there's a problem with the boat to Orbis.");
            await SendOk();
        }
        else if (evt.Get("Board").Equals(true))
        {
            AddText("This will not be a short flight, so if you need to take care of some things, I suggest you do that first before getting on board. ");
            AddText("Do you still wish to board the ship?");
            var yes = await SendYesNo();

            if (yes)
            {
                if (HasItem(ticket))
                {
                    GainItem(ticket, -1);
                    SetField(board);
                }
                else
                {
                    AddText("Oh no ... I don't think you have the ticket with you. ");
                    AddText("I can't let you in without it. ");
                    AddText("Please buy the ticket at the ticketing booth.");
                    await SendNext();
                }
            }
            else
            {
                AddText("You must have some business to take care of here, right?");
                await SendNext();
            }
        }
        else if (evt.Get("Odocked").Equals(true))
        {
            AddText("The ship is getting ready for takeoff. ");
            AddText("I'm sorry, but you'll have to get on the next ride. ");
            AddText("The ride schedule is available through the usher at the ticketing booth.");
            await SendNext();
        }
        else if (evt.Get("Flying").Equals(true) || evt.Get("Direction").Equals("Ellinia"))
        {
            AddText("We will begin boarding 5 minutes before the takeoff. ");
            AddText("Please be patient and wait for a few minutes. ");
            AddText("Be aware that the ship will take off on time, and we stop receiving tickets 1 minute before that, so please make sure to be here on time.");
            await SendNext();
        }

        Stop();
    }
}