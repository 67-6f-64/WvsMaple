using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

/*
 * Name: Jane.
 * Location: Victoria Island: Lith Harbor (104000000).
 * 
 * Purpose: Sells potions after completing the pre-quests:
 *          Jane and the Wild Boar (2010),
 *          Jane's First Challenge (2011),
 *          Jane's Second Challenge (2012),
 *          Jane's Final Challenge (2013).
 * 
 * Author: Fraysa.
 */

class jane : NpcScript
{
    // NOTE: MapleID, Cost, Recovery Amount.

    private const int[,] items = new int[,] {
        {2000002, 310, 300},
        {2022003, 1060, 1000},
        {202200, 1600, 800},
        {200100, 3120, 1000}
    };

    public override async Task Run()
    {
        if (HasQuestCompleted(2013))
        {
            AddText("It's you... thanks to you I was able to get a lot done. ");
            AddText("Nowadays I've been making a bunch of items. ");
            AddText("If you need anything let me know.");
            await SendNext();

            AddText("Which item would you like to buy?");

            string[] selections = new string[] { };
            for (int i = 0; i < items.Length; i++)
            {
                selections[i] = "#z" + items[i, 0] + "# (Price : " + items[i, 1] + " mesos)";
            }

            var choice = await SendMenu(selections);

            var mapleId = items[choice, 0];
            var cost = items[choice, 1];
            var recovery = items[choice, 2];

            AddText("You want #b#t" + mapleId + "##k? #t" + mapleId + "# allows you to recover " + recovery + " " + "TODO" + ". ");
            AddText("How many would you like to buy?");
            var count = await SendGetNumber(1, 1, 100);
            var totalCost = cost * count;

            AddText("Will you purchase #r" + count + "#k #b#t" + mapleId + "#(s)#k? ");
            AddText("#t" + mapleId + "# costs " + cost + " mesos for one, so the total comes out to be #r" + totalCost + "#k mesos.");
            var answer = await SendYesNo();

            if (answer)
            {
                if (HasOpenSlotsFor(mapleId) && GainMeso(-totalCost))
                {
                    GainItem(mapleId, count);

                    AddText("Thank you for coming. ");
                    AddText("Stuff here can always be made so if you need something, please come again.");
                    await SendNext();
                }
                else
                {
                    AddText("Are you lacking mesos by any chance? ");
                    AddText("Please check and see if you have an empty slot available at your etc. inventory, and if you have atleast #r" + totalCost + "#k mesos with you.");
                    await SendNext();
                }
            }
            else
            {
                AddText("I still have quite a few of the materials you got me before. ");
                AddText("The items are all there so take your time choosing.");
                await SendNext();
            }
        }
        else if (HasQuestCompleted(2010))
        {
            AddText("You don't seem strong enough to be able to purchase my potion...");
            await SendNext();
        }
        else
        {
            AddText("My dream is to travel everywhere, much like you. ");
            AddText("My father, however, does not allow me to do it, because he thinks it's very dangerous. ");
            AddText("He may say yes, though, if I show him some sort of a proof that I'm not the weak girl that he thinks I am...");
            await SendOk();
        }

        Stop();
    }
}