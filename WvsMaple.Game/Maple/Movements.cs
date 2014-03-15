using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple
{
    public class Movements : List<Movement>
    {
        public static Movements Parse(Packet inPacket)
        {
            return new Movements(inPacket);
        }

        public Movements(Packet inPacket)
            : base()
        {
            inPacket.ReadShort();
            inPacket.ReadShort();

            byte commands = inPacket.ReadByte();

            for (byte b = 0; b < commands; b++)
            {
                byte type = inPacket.ReadByte();

                switch (type)
                {
                    case 0:
                    case 5:
                        this.Add(new AbsoluteMovement(type, inPacket));
                        break;

                    case 1:
                    case 2:
                    case 6:
                        this.Add(new RelativeMovement(type, inPacket));
                        break;

                    case 3:
                    case 4:
                    case 7:
                        this.Add(new InstantMovement(type, inPacket));
                        break;

                    case 8:
                        this.Add(new EquipmentMovement(type, inPacket));
                        break;
                }
            }
        }
    }

    public abstract class Movement
    {
        public byte Type { get; private set; }
        public byte NewStance { get; set; }

        public Movement(byte type)
        {
            this.Type = type;
        }
    }

    public class AbsoluteMovement : Movement
    {
        public Position Position { get; set; }
        public Position Wobble { get; set; }
        public short Duration { get; set; }
        public short Foothold { get; set; }

        public AbsoluteMovement(byte type, Packet inPacket)
            : base(type)
        {
            this.Position = new Position(inPacket.ReadShort(), inPacket.ReadShort());
            this.Wobble = new Position(inPacket.ReadShort(), inPacket.ReadShort());
            this.Foothold = inPacket.ReadShort();
            this.NewStance = inPacket.ReadByte();
            this.Duration = inPacket.ReadShort();
        }
    }

    public class RelativeMovement : Movement
    {
        public Position Delta { get; set; }
        public short Duration { get; set; }

        public RelativeMovement(byte type, Packet inPacket) :
            base(type)
        {
            this.Delta = new Position(inPacket.ReadShort(), inPacket.ReadShort());
            this.NewStance = inPacket.ReadByte();
            this.Duration = inPacket.ReadShort();
        }
    }

    public class InstantMovement : Movement
    {
        public Position Delta { get; set; }
        public short Duration { get; set; }
        public short Foothold { get; set; }

        public InstantMovement(byte type, Packet inPacket)
            : base(type)
        {
            this.Delta = new Position(inPacket.ReadShort(), inPacket.ReadShort());
            this.Foothold = inPacket.ReadShort();
            this.NewStance = inPacket.ReadByte();
            this.Duration = inPacket.ReadShort();
        }
    }

    public class EquipmentMovement : Movement
    {
        public byte Unknown { get; set; }

        public EquipmentMovement(byte type, Packet inPacket)
            : base(type)
        {
            this.Unknown = inPacket.ReadByte();
        }
    }
}
