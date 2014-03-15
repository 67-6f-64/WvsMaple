using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

class test : NpcScript
{
    public override async Task Run()
    {
        AddText("Press 'Yes' to continue.");
        var yes = await SendYesNo();

        if (yes)
        {
            AddText("Yes!");
        }
        else
        {
            AddText("No!");
        }

        await SendOk();

        Stop();
    }
}
