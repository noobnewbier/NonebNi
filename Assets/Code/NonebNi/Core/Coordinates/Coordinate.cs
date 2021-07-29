using System;
using UnityEngine;

namespace Noneb.Core.Game.Coordinates
{
    //Using Cube Coordinate(which is just axial coordinate) : https://www.redblobgames.com/grids/hexagons/#map-storage
    //    1. AxialCoordinate refers to cube coordinate(in game logic)
    //    2. FlatCoordinate refers to the (x, z) value in a packed(without empty padding values) 2d array 
    [Serializable]
    public struct Coordinate : IEquatable<Coordinate>
    {
        [SerializeField] private int x;
        [SerializeField] private int z;

        public Coordinate(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public int X => x;

        public int Z => z;

        public int Y => -X - Z;

        public bool Equals(Coordinate other) => X == other.X && Z == other.Z;

        public override bool Equals(object obj) => obj is Coordinate other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Z;
            }
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.X + b.X, a.Z + b.Z);

        public static bool operator ==(Coordinate a, Coordinate b) => a.Equals(b);

        public static bool operator !=(Coordinate a, Coordinate b) => !(a == b);

        public static Coordinate operator -(Coordinate c) => new Coordinate(-c.X, -c.Z);

        public static float ManhattanDistance(Coordinate a, Coordinate b) =>
            a.X - b.X +
            (a.Y - b.Y) +
            (a.Z - b.Z);
    }
}