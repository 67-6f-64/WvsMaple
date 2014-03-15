using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Fields
{
    public abstract class FieldObject
    {
        public Position Position { get; set; }
        public byte Stance { get; set; }
        public virtual int ObjectID { get; set; }

        public Field Field { get; set; }

        public FieldObject()
        {
            this.ObjectID = -1;
        }
    }

    public class Position
    {
        public short X { get; set; }
        public short Y { get; set; }

        public Position(short x, short y)
        {
            this.X = x;
            this.Y = y;
        }

        public Position(int x, int y)
        {
            this.X = (short)x;
            this.Y = (short)y;
        }

        public double DistanceFrom(Position point)
        {
            return Math.Sqrt(Math.Pow(this.X - point.X, 2) + Math.Pow(this.Y - point.Y, 2));
        }

        public bool IsInRectangle(Rectangle rectangle)
        {
            return this.X >= rectangle.Top &&
                this.Y >= rectangle.Left &&
                this.X <= rectangle.Right &&
                this.Y <= rectangle.Bottom;
        }

        public static Position operator +(Position p1, Position p2)
        {
            return new Position(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Position operator -(Position p1, Position p2)
        {
            return new Position(p1.X - p2.X, p1.Y - p2.Y);
        }
    }
}
