using WvsGame.Maple.Scripting;

class out00: PortalScript
{
    public override void Run()
    {
        int destinationId = GetLocation("Free Market");

        if (destinationId != 0)
        {
            SetField(destinationId);
            DeleteLocation("Free Market");
        }
    }
}
