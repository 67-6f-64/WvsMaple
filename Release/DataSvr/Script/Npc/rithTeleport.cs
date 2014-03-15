using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

/*
 * Name: Phil.
 * Location: Victoria Island: Lith Harbor (104000000).
 * 
 * Purpose: Beginner advices & Teleportation service to various towns in Victoria Island.
 * 
 * Author: Fraysa.
 */

class rith_teleport : NpcScript
{
    public override async Task Run()
    {
        int[,] fields = new int[,] {
            {102000000, 1200},
            {101000000, 1200},
            {100000000, 800},
            {103000000, 1000}
        };

        AddText("Do you wanna head over to some other town? ");
        AddText("With a little money involved, I can make it happen. ");
        AddText("It's a tad expensive, but I run a special 90% discount for beginners.");
        await SendNext();

        AddText("It's understable that you may be confused about this place if this is your first time around. ");
        AddText("If you got any questions about this place, fire away.");
        var choice = await SendMenu("What kind of towns are here in Victoria Island?",
            "Please take me somewhere else.");

        if (choice == 0)
        {
            string[] choices = new string[] {
                MapRef(104000000),
                MapRef(102000000),
                MapRef(101000000),
                MapRef(100000000),
                MapRef(103000000),
                MapRef(105040300)
            };

            AddText("There are 6 big towns here in Victoria Island. ");
            AddText("Which of those do you want to know more of?");
            choice = await SendMenu(choices);

            switch (choice)
            {
                case 0:

                    AddText("The town you are at is Lith Harbor! ");
                    AddText("Alright I'll explain to you more about " + Blue(MapRef(104000000)) + ". ");
                    AddText("It's the place you landed on Victoria Island by riding The Victoria. ");
                    AddText("That's " + MapRef(104000000) + ". ");
                    AddText("A lot of beginners who just got here from Maple Island start their journey here.");
                    await SendNext();

                    AddText("It's a quiet town with the wide body of water on the back of it, thanks to the fact that the harbor is located at the west end of the island. ");
                    AddText("Most of the people here are, or used to be fishermen, so they may look intimidating, but if you strike up a conversation with them, they'll be friendly to you.");
                    await SendNextPrev();

                    AddText("Around town lies a beautiful prairie. ");
                    AddText("Most of the monsters there are small and gentle, perfect for beginners. ");
                    AddText("If you haven't chosen your job yet, this is a good place to boost up your level.");
                    await SendNextPrev();

                    break;

                case 1:

                    AddText("Alright I'll explain to you more about " + Blue(MapRef(102000000)) + ". ");
                    AddText("It's a warrior-town located at the northern-most part of Victoria Island, surrounded by rocky mountains. ");
                    AddText("With an unfriendly atmosphere, only the strong survives there.");
                    await SendNext();

                    AddText("Around the highland you'll find a really skinny tree, a wild hog running around the place, and monkeys that live all over the island. ");
                    AddText("There's also a deep valley, and when you go deep into it, you'll find a humongous dragon with the power to match his size. ");
                    AddText("Better go in there very carefully, or don't go at all.");
                    await SendNextPrev();

                    AddText("If you want to be a the " + Blue("Warrior") + " then find " + Red(NpcRef(1022000)) + ", the chief of " + MapRef(102000000) + ". ");
                    AddText("If you're level 10 or higher, along with a good STR level, he may make you a warrior afterall. ");
                    AddText("If not, better keep training yourself until you reach that level.");
                    await SendNextPrev();

                    break;

                case 2:

                    AddText("Alright I'll explain to you more about " + Blue(MapRef(101000000)) + ". ");
                    AddText("It's a magician-town located at the fart east of Victoria Island, and covered in tall, mystic trees. ");
                    AddText("You'll find some fairies there, too; They don't like humans in general so it'll be best for you to be on their good side and stay quiet.");
                    await SendNext();
                    AddText("Near the forest you'll find green slimes, walking mushrooms, monkeys and zombie monkeys all residing there. ");
                    AddText("Walk deeper into the forest and you'll find witches with the flying broomstick navigating the skies. ");
                    AddText("A word of warning: unless you are really strong, I recommend you don't go near them.");
                    await SendNextPrev();

                    AddText("If you want to be the " + Blue("Magician") + ", search for " + Red(NpcRef(1032001)) + ", the head wizard of " + MapRef(101000000) + ". ");
                    AddText("He may make you a wizard if you're at or above level 8 with a decent amount of INT. ");
                    AddText("If that's not the case, you may have to hunt more and train yourself to get there.");
                    await SendNextPrev();

                    break;

                case 3:

                    AddText("Alright I'll explain to you more about " + Blue(MapRef(100000000)) + ". ");
                    AddText("It's a bowman-town located at the southernmost part of the island, made on a flatland in the mIdst of a deep forest and prairies. ");
                    AddText("The weather's just right, and everything is plentiful around that town, perfect for living. ");
                    AddText("Go check it out.");
                    await SendNext();

                    AddText("Around the prairie you'll find weak monsters such as snails, mushrooms, and pigs. ");
                    AddText("According to what I hear, though, in the deepest part of the Pig Park, which is connected to the town somewhere, you'll find a humongous, powerful mushroom called Mushmom every now and then.");
                    await SendNextPrev();

                    AddText("If you want to be the " + Blue("Bowman") + ", you need to go see " + Red(NpcRef(1012100)) + " at " + MapRef(100000000) + ". ");
                    AddText("With a level at or above 10 and a decent amount of DEX, she may make you be one afterall. ");
                    AddText("If not, go train yourself, make yourself stronger, then try again.");
                    await SendNextPrev();

                    break;

                case 4:

                    AddText("Alright I'll explain to you more about " + Blue(MapRef(103000000)) + ". ");
                    AddText("It's a thief-town located at the northwest part of Victoria Island, and there are buildings up there that have just this strange feeling around them. ");
                    AddText("It's mostly covered in black clouds, but if you can go up to a really high place, you'll be able to see a very beautiful sunset there.");
                    await SendNext();

                    AddText("From " + MapRef(103000000) + ", you can go into several dungeons. ");
                    AddText("You can go to a swamp where alligators and snakes are abound, or hit the subway full of ghosts and bats. ");
                    AddText("At the deepest part of the underground, you'll find Lace, who is just as big and dangerous as a dragon.");
                    await SendNextPrev();

                    AddText("If you want to be the " + Blue("Thief") + ", seek " + Red(NpcRef(1052001)) + ", the heart of darkness of " + MapRef(103000000) + ". ");
                    AddText("He may well make you the thief if you're at or above level 10 with a good amount of DEX. ");
                    AddText("If not, go hunt and train yourself to reach there.");
                    await SendNextPrev();

                    break;

                case 5:

                    AddText("Alright I'll explain to you more about " + Blue(MapRef(105040300)) + ". ");
                    AddText("It's a forest town located at the southeast side of Victoria Island. ");
                    AddText("It's pretty much in between " + MapRef(100000000) + " and the ant-tunnel dungeon. ");
                    AddText("There's a hotel there, so you can rest up after a long day at the dungeon ... it's a quiet town in general.");
                    await SendNext();

                    AddText("In front of the hotel there's an old buddhist monk by the name of " + Red(NpcRef(1061000)) + ". ");
                    AddText("Nobody knows a thing about that monk. ");
                    AddText("Apparently he collects materials from the travellers and create something, but I am not too sure about the details. ");
                    AddText("If you have any business going around that area, please check that out for me.");
                    await SendNextPrev();

                    AddText("From " + MapRef(105040300) + ", head east and you'll find the ant tunnel connected to the deepest part of the Victoria Island. ");
                    AddText("Lots of nasty, powerful monsters abound so if you walk in thinking it's a walk in the park, you'll be coming out as a corpse. ");
                    AddText("You need to fully prepare yourself for a rough ride before going in.");
                    await SendNextPrev();

                    AddText("And this is what I hear ... apparently, at " + MapRef(105040300) + " there's a secret entrance leading you to an unknown place. ");
                    AddText("Apparently, once you move in deep, you'll find a stack of black rocks that actually move around. ");
                    AddText("I want to see that for myself in the near future ...");
                    await SendNextPrev();

                    break;
            }
        }
        else if (choice == 1)
        {
            if (GetJob() == 0)
            {
                AddText("There's a special 90% discount for all beginners. ");
                AddText("Alright, where would you want to go?");
            }
            else
            {
                AddText("Oh you aren't a beginner, huh? ");
                AddText("Then I'm afraid I may have to charge you a full price .");
                AddText("Where would you like to go?");
            }

            string[] choices = new string[] { };

            for (int i = 0; i < fields.Length; i++)
            {
                choices[i] = GenerateChoice(fields[i, 0], GetPrice(fields[i, 1]));
            }

            choice = await SendMenu(choices);

            var fieldId = fields[choice, 0];
            var price = fields[choice, 1];

            AddText("I guess you don't need to be here. ");
            AddText("Do you really want to move to " + Blue(MapRef(fieldId)) + "? Well it'll cost you " + Blue(price + " meso") + ". ");
            AddText("What do you think?");
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
                    AddText("With your abilities, you should have more than that!");
                    await SendOk();
                }
            }
            else
            {
                AddText("There's alot to see in this town too. ");
                AddText("Let me know if you want to go somewhere else.");
                await SendOk();
            }
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