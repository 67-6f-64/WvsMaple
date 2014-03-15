using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

/*
 * Name: Regular Cab.
 * Location: Victoria Island: Ellinia (101000000).
 * 
 * Purpose: Teleportation service to various towns in Victoria Island.
 * 
 * Author: Fraysa.
 */

class taxi1 : NpcScript
{
    public override async Task Run()
    {
        int[] fields = new int[]{
            104000000,
            102000000,
            103000000,
            100000000
        };

        int[] prices = new int[] {
            1200,
            1000,
            1200,
            1000
        };

        string[] choices = new string[] { 
            GenerateChoice(104000000, 1200),
            GenerateChoice(102000000, 1000),
            GenerateChoice(103000000, 1200),
            GenerateChoice(100000000, 1000)
        };

        AddText("Hi! I drive the " + NpcRef(1022001) + ". ");
        AddText("If you want to go from town to town safely and fast, then ride our cab. ");
        AddText("We'll gladly take you to your destination with an affordable price.");
        await SendNext();

        if (GetJob() == 0)
        {
            AddText("We have a special 90% discount for beginners. ");
        }

        AddText("Choose your destination, for fees will change from place to place.");
        var choice = await SendMenu(choices);

        var fieldId = fields[choice];
        var price = prices[choice];

        AddText("You don't have anything else to do here, huh? ");
        AddText("Do you really want to go to " + Blue(MapRef(fieldId)) + "? ");
        AddText("It'll cost you " + Blue(price + " mesos") + ".");
        var yes = await SendYesNo();

        if (yes)
        {
            if (GainMeso(-price))
            {
                SetField(fieldId);
            }
            else
            {
                AddText("You don't have enough mesos. ");
                AddText("Sorry to say this, but without them, you won't be able to ride this cab.");
                await SendOk();
            }
        }
        else
        {
            AddText("There's a lot to see in this town, too. ");
            AddText("Come back and find me when you need to go to a different town.");
            await SendOk();
        }

        Stop();
    }

    private string GenerateChoice(int mapId, int price)
    {
        return MapRef(mapId) + "(" + price + " mesos) ";
    }

    private int GetPrice(int basePrice)
    {
        if (GetJob() == 0)
        {
            return basePrice / 10;
        }

        return basePrice;
    }
}