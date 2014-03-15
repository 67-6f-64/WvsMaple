using System.Threading.Tasks;
using WvsGame.Maple.Scripting;
 
class getAboard : NpcScript
{
    public override async Task Run()
    {
        int mapId = -1;
 
        AddText("Orbis Station has lots of platforms available to choose from. ");
        AddText("You need to choose the one that'll take you to the destination of your choice. ");
        AddText("Which platform will you take?");
       
        var choice = await SendMenu("The platform to the ship that heads to Ellinia",
            "The platform to the ship that heads to Ludibrium");
 
        AddText("Even if you took the wrong passage you can get back here using the portal, so no worries.");
 
        if (choice == 0)
        {
            mapId = 200000110;
            AddText(" Will you move to the #bplatform to the ship that heads to Ellinia#k?");
        }
        else if (choice == 1)
        {
            mapId = 200000120;
            AddText(" Will you move to the #bplatform to the ship that heads to Ludibrium#k?");
        }
 
        var answer = await SendYesNo();
 
        if (answer)
        {
            SetField(mapId, "west00");
        }
        else
        {
            AddText("Please make sure you know where you are going and then go to the platform through me. ");
            AddText("The ride is on schedule so you better not miss it!");
 
            await SendNext();
        }
     }
}