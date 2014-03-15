using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WvsGame.Maple.Events.Implementation
{
    class ShipLudibrium : Event
    {
        private const int BoardTime = 5;
        private const int DockTime = 1;
        private const int FlightTime = 15;

        public override string Name
        {
            get { return "ShipLudibrium"; }
        }

        public override bool IsBackground
        {
            get { return true; }
        }

        public override void Initiate()
        {
            this.Schedule();
        }

        private void Schedule()
        {
            
        }

        private void CloseEntry()
        {

        }

        public override void Initiate(Characters.Character participant) { }
    }
}
