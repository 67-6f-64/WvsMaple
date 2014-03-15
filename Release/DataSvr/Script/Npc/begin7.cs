using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

/*
 * Name: Shanks.
 * Map: Southperry (60000).
 * 
 * Purpose: Paid teleportation service for beginners to Lith Harbor.
 * 
 * Author: Fraysa.
 */

class begin7 : NpcScript
{
    public override async Task Run()
    {
        var recommendationLetter = HasItem(4031801);

        AddText("Take this ship and you'll head off to a bigger continent.");
        AddText(Bold(" For 150 mesos") + ", I'll take you to " + Blue("Victoria Island") + ". ");
        AddText("The thing is, once you leave this place, you can't ever come back. ");
        AddText("What do you think? Do you want to go to Victoria Island?");
        var yes = await SendYesNo();

        if (yes)
        {
            if (GetLevel() >= 7)
            {
                if (recommendationLetter)
                {
                    AddText("Okay, now give me 150 mesos... ");
                    AddText("Hey, what's that? ");
                    AddText("Is that the recommendation letter from Lucas, the chief of Amherst? ");
                    AddText("Hey, you should have told me you had this. ");
                    AddText("I, Shanks, recognize greatness when I see one, and since you have been recommended by Lucas, I see that you have a great, great potential as an adventurer. ");
                    AddText("No way would I charge you for this trip!");
                    await SendNext();

                    AddText("Since you have the recommendation letter, I won't charge you for this. ");
                    AddText("Alright, buckle up, because we're going to head to Victoria Island right now, and it might get a bit turbulent!!");
                    await SendNextPrev();

                    GainItem(4031801, -1);
                    SetField(104000000);
                }
                else if (GainMeso(-150))
                {
                    AddText("Bored of this place? ");
                    AddText("Here... Give me 150 mesos first...");
                    await SendNext();

                    AddText("Awesome! " + Bold("150 mesos") + " accepted! ");
                    AddText("Alright, off to Victoria Island!");
                    await SendNext();

                    SetField(104000000);
                }
                else
                {
                    AddText("What? ");
                    AddText("You're telling me you wanted to go without any money? ");
                    AddText("You're one weirdo...");
                    await SendNextPrev();
                }
            }
            else
            {
                AddText("Let's see... ");
                AddText("I don't think you are strong enough. ");
                AddText("You'll have to be at least " + Blue("Level 7") + " to go to Victoria Island.");
                await SendNext();
            }
        }
        else
        {
            AddText("Hmm... I guess you still have things to do here?");
            await SendNext();
        }

        Stop();
    }
}