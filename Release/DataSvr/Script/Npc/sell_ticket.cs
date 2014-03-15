using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

class sell_ticket : NpcScript
{
    public override async Task Run()
    {
        if (GetLevel() < 10)
        {
            AddText("Your level seems to be too low for this. ");
            AddText("We do not allow anyone below Level 10 to get on this ride, for the sake of safety.");
            await SendNext();
        }
        else
        {
            int item;
            int price;

            if (GetLevel() < 30)
            {
                item = 4031044;
                price = 1000;
            }
            else
            {
                item = 4031045;
                price = 5000;
            }

            AddText("Hello, I'm in charge of selling tickets for the ship ride to Orbis Station of Ossyria. ");
            AddText("The ride to Orbis takes off every 15 minutes, beginning on the hour, and it'll cost you #b" + price + " mesos#k. ");
            AddText("Are you sure you want to purchase #b#t" + item + "##k?");
            var yes = await SendYesNo();

            if (yes)
            {
                if (!HasOpenSlotsFor(item) || !GainMeso(price))
                {
                    GainItem(item);
                }
                else
                {
                    AddText("Are you sure you have #b" + price + " mesos#k? ");
                    AddText("If so, then I urge you to check your etc. inventory, and see if it's full or not.");
                    await SendOk();
                }
            }
            else
            {
                AddText("You must have some business to take care of here, right?");
                await SendNext();
            }
        }

        Stop();
    }
}