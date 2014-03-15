using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

class go_pc : NpcScript
{
    public override async Task Run()
    {
        AddText("Aren't you connected through the Internet Cafe? ");
        AddText("If so, then go in here ... you'll probably head to a familiar place. ");
        AddText("What do you think? Do you want to go in?");
        var yes = await SendYesNo();

        if (yes)
        {
            AddText("Hey, hey ... I don't think you're logging on from the internet cafe. ");
            AddText("You can't enter this place if you are logging on from home ...");
        }
        else
        {
            AddText("You must be busy, huh? ");
            AddText("But if you're loggin on from the internet cafe, then you should try going in. ");
            AddText("You may end up in a strange place once inside.");
        }

        await SendNext();

        Stop();
    }
}