using Common;
using Common.Threading;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Life;

namespace WvsGame.Maple.Events.Implementation
{
    class ShipOssyria : Event
    {
        private OssyriaBoatDirection Direction = OssyriaBoatDirection.NotSet;

        private const int EntryTime = 5; // Note: Time to accept entries.
        private const int BoardTime = 1; // NOTE: Time until take off.
        private const int FlightTime = 15; // NOTE: Time for each direction.
        private const int InvasionTime = 1; // NOTE: Time to spawn Balrogs.

        private const int Balrog = 8150000;
        private const byte BalrogAmount = 2;

        public override string Name
        {
            get { return "ShipOssyria"; }
        }

        public override bool IsBackground
        {
            get { return true; }
        }

        public override void Initiate()
        {
            this.AddField("ElliniaStation", 101000300);
            this.AddField("ElliniaBoard", 101000301);
            this.AddField("ElliniaFlight", 200090010);
            this.AddField("ElliniaCabin", 200090011);

            this.AddField("OrbisStation", 200000100);
            this.AddField("OrbisBoard", 200000112);
            this.AddField("OrbisFlight", 200090000);
            this.AddField("OrbisCabin", 200090001);

            this.Schedule();
        }

        private void Schedule()
        {
            this.Set("Boarding", true);
            this.Set("Docking", false);
            this.Set("Flying", false);
            this.Set("Invasion", false);

            this.SwitchDirection();

            this.Register(CloseEntry, EntryTime * 60 * 1000);
        }

        private void CloseEntry()
        {
            this.Set("Boarding", false);
            this.Set("Docking", true);

            this.Register(Takeoff, BoardTime * 60 * 1000);
        }

        private void Takeoff()
        {
            this.Set("Docking", false);
            this.Set("Flying", true);

            if (this.Direction == OssyriaBoatDirection.Orbis)
            {
                this.Warp("ElliniaBoard", "ElliniaFlight");
            }
            else
            {
                this.Warp("OrbisBoard", "OrbisFlight");
            }

            this.Register(Arrive, FlightTime * 60 * 1000);
            this.Register(Invasion, InvasionTime * 60 * 1000);
        }

        private void Invasion()
        {
            if (Randomizer.Next(0, 10) <= 5)
            {
                this.Set("Invasion", true);
            }
        }

        private void Arrive()
        {
            if (this.Direction == OssyriaBoatDirection.Orbis)
            {
                this.Warp("ElliniaFlight", "OrbisStation");
                this.Warp("ElliniaCabin", "OrbisStation");
            }
            else
            {
                this.Warp("OrbisFlight", "ElliniaStation");
                this.Warp("OrbisCabin", "ElliniaStation");
            }

            this.Schedule();
        }

        private void SwitchDirection()
        {
            switch (this.Direction)
            {
                case OssyriaBoatDirection.NotSet:
                    this.Set("Direction", "Orbis");
                    this.Direction = OssyriaBoatDirection.Orbis;
                    break;

                case OssyriaBoatDirection.Orbis:
                    this.Set("Direction", "Ellinia");
                    this.Direction = OssyriaBoatDirection.Ellinia;
                    break;

                case OssyriaBoatDirection.Ellinia:
                    this.Set("Direction", "Orbis");
                    this.Direction = OssyriaBoatDirection.Orbis;
                    break;
            }
        }


        public override void Initiate(Characters.Character participant) { }
    }

    internal enum OssyriaBoatDirection : byte
    {
        NotSet,
        Ellinia,
        Orbis
    }
}
