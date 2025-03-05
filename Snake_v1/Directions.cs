

using System;
using System.Collections.Generic;

namespace Snake_v1
{
    public class Directions
    {   
        public readonly static Directions Left = new Directions(0, -1);
        public readonly static Directions Right = new Directions(0, 1);
        public readonly static Directions Up = new Directions(-1, 0);
        public readonly static Directions Down = new Directions(1, 0);
        
        
        public int RowOffset { get; }
        public int ColOffset { get; }

        private Directions(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        public Directions Opposite()
        {
            return new Directions(-RowOffset, -ColOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Directions directions &&
                   RowOffset == directions.RowOffset &&
                   ColOffset == directions.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(Directions left, Directions right)
        {
            return EqualityComparer<Directions>.Default.Equals(left, right);
        }

        public static bool operator !=(Directions left, Directions right)
        {
            return !(left == right);
        }
    }
}
