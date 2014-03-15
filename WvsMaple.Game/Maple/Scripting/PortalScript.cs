using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Maple.Characters;

namespace WvsGame.Maple.Scripting
{
    public abstract class PortalScript
    {
        public Character Character { get; private set; }

        public void Initiate(Character character)
        {
            this.Character = character;
        }

        public abstract void Run();

        public void SetField(int fieldId, byte portalId = 0)
        {
            this.Character.SetField(fieldId, portalId);
        }

        public void SetField(int fieldId, string portalLabel)
        {
            this.Character.SetField(fieldId, this.Character.Field.Portals[portalLabel].ID);
        }

        public void SaveLocation(string label)
        {
            this.Character.SavedLocations.Insert(label, this.Character.Field.MapleID);
        }

        public int GetLocation(string label)
        {
            return this.Character.SavedLocations.Get(label);
        }

        public void DeleteLocation(string label)
        {
            this.Character.SavedLocations.Delete(label);
        }

        public void InFreeMarket()
        {
            this.SaveLocation("Free Market");
            this.SetField(910000000, "out00");
        }

        public void OutFreeMarket()
        {
            int destinationId = this.GetLocation("Free Market");

            if (destinationId == 0)
            {
                this.Character.Notify("An error has occured going through the portal.");
                return;
            }

            this.SetField(destinationId, "market00");
            this.DeleteLocation("Free Market");

        }
    }
}
