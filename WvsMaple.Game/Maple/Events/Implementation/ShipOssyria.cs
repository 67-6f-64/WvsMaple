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
        public override string Name
        {
            get { return "ShipOssyria"; }
        }

        public override bool IsBackground
        {
            get { return true; }
        }

        #region Constants

        private BoatDirection Direction = BoatDirection.NotSet;

        private const int EntryTime =           5; // Note: Time to accept entries.
        private const int BoardTime =           1; // NOTE: Time until take off.
        private const int FlightTime =          15; // NOTE: Time for each direction.
        private const int InvasionTime =        1; // NOTE: Time to spawn Balrogs.

        private const int Balrog =              8150000;
        private const byte BalrogAmount =       2;

        private int[,] Fields = new int[,]
        {
            {101000300, 101000301, 200090010, 200090011}, // NOTE: Station, Board, Flight, Cabin.
            {200000100, 200000112, 200090000, 200090001} // NOTE: Station, Board, Flight, Cabin.
        };

        #endregion

        public override void Initiate()
        {
            this.Schedule();
        }

        private void Schedule()
        {
            this.Set("Board", true);
            this.Set("Odocked", false);
            this.Set("Flying", false);

            this.SwitchDirection();

            Delay.Execute(EntryTime * 60 * 1000, () =>
            {
                this.CloseEntry();
            });
        }

        private void CloseEntry()
        {
            this.Set("Board", false);
            this.Set("Odocked", true);

            Delay.Execute(BoardTime * 60 * 1000, () =>
            {
                this.Takeoff();
            });
        }

        private void Takeoff()
        {
            this.Set("Odocked", false);
            this.Set("Flying", true);

            this.GetField(this.Direction == BoatDirection.Orbis ? Fields[0, 0] : Fields[1, 0]).HasShip = false;
            this.GetField(this.Direction == BoatDirection.Orbis ? Fields[0, 0] : Fields[1, 0]).ShowEffect(FieldEffect.ShipLeave);
            this.Warp(this.Direction == BoatDirection.Orbis ? Fields[0, 1] : Fields[1, 1], this.Direction == BoatDirection.Orbis ? Fields[0, 2] : Fields[1, 2]);

            Delay.Execute(FlightTime * 60 * 1000, () =>
            {
                this.Arrive();
            });

            Delay.Execute(InvasionTime * 60 * 1000, () =>
            {
                this.Invasion();
            });
        }

        private void Invasion()
        {
            if (Randomizer.Next(0, 10) <= 5)
            {
                this.Set("Invasion", true);
                this.Spawn(this.Direction == BoatDirection.Orbis ? Fields[0, 2] : Fields[1, 2], Balrog, BalrogAmount, this.Direction == BoatDirection.Orbis ? new Fields.Position(483, -221) : new Fields.Position(-590, -221));

                // TODO: Change BGM to 'Bgm04/ArabPirate' & Send the boat effect.
            }
        }

        private void Arrive()
        {
            this.Warp(this.Direction == BoatDirection.Orbis ? Fields[0, 2] : Fields[1, 2], this.Direction == BoatDirection.Orbis ? Fields[1, 0] : Fields[0, 0]);
            this.Warp(this.Direction == BoatDirection.Orbis ? Fields[0,3 ] : Fields[1, 3], this.Direction == BoatDirection.Orbis ? Fields[1, 0] : Fields[0, 0]);

            this.Schedule();
        }

        private void SwitchDirection()
        {
            switch (this.Direction)
            {
                case BoatDirection.NotSet:
                    this.Set("Direction", "Orbis");
                    this.Direction = BoatDirection.Orbis;
                    this.GetField(Fields[0, 0]).HasShip = true;
                    this.GetField(Fields[0, 0]).ShowEffect(FieldEffect.ShipArrive);
                    break;

                case BoatDirection.Orbis:
                    this.Set("Direction", "Ellinia");
                    this.Direction = BoatDirection.Ellinia;
                    this.GetField(Fields[1, 0]).HasShip = true;
                    this.GetField(Fields[1, 0]).ShowEffect(FieldEffect.ShipArrive);
                    break;

                case BoatDirection.Ellinia:
                    this.Set("Direction", "Orbis");
                    this.Direction = BoatDirection.Orbis;
                    this.GetField(Fields[0, 0]).HasShip = true;
                    this.GetField(Fields[0, 0]).ShowEffect(FieldEffect.ShipArrive);
                    break;
            }
        }


        public override void Initiate(Characters.Character participant) { }
    }

    internal enum BoatDirection : byte
    {
        NotSet,
        Ellinia,
        Orbis
    }
}
