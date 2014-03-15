using System.Threading.Tasks;
using WvsGame.Maple.Scripting;

class go_xmas : NpcScript
{
    public override async Task Run()
    {
        AddText("Do you want to " + (GetField().MapleID == 101000000 ? "go to" : "get out of") + "Happyville?");
        var yes = await SendYesNo();

        if (yes)
        {
            SetField(GetField().MapleID == 101000000 ? 209000000 : 101000000);
        }

        Stop();
    }
}