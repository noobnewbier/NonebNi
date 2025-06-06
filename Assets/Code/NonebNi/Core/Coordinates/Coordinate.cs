﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NonebNi.Core.Actions;
using NonebNi.Core.Maps;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Coordinates
{
    /// <summary>
    ///     Using Cube Coordinate(which is just axial coordinate) : https://www.redblobgames.com/grids/hexagons/#map-storage
    ///     1. AxialCoordinate refers to cube coordinate(in game logic)
    ///     2. FlatCoordinate refers to the (x, z) value in a packed(without empty padding values) 2d array
    /// </summary>
    [Serializable]
    public record Coordinate : IActionTarget
    {
        public static readonly Coordinate Zero = new(0, 0);
        [SerializeField] private int x;
        [SerializeField] private int z;

        public Coordinate(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public Coordinate() { }

        public int X => x;

        public int Z => z;

        public int Y => -X - Z;

        public Coordinate[] Neighbours => new[]
        {
            this + HexDirection.West,
            this + HexDirection.NorthWest,
            this + HexDirection.NorthEast,
            this + HexDirection.East,
            this + HexDirection.SouthEast,
            this + HexDirection.SouthWest
        };

        public Coordinate RotateRight() => new(-Y, -x);

        public Coordinate RotateLeft() => new(-z, -Y);

        public Coordinate Normalized()
        {
            var normalizedCoordInVec = Vector3.Normalize(new Vector3(x, Y, z));
            return new Coordinate(Mathf.RoundToInt(normalizedCoordInVec.x), Mathf.RoundToInt(normalizedCoordInVec.z));
        }

        public int DistanceTo(Coordinate coordinate)
        {
            //Ref: https://www.redblobgames.com/grids/hexagons/#distances
            var subtractedCoordinate = new Coordinate(
                X - coordinate.X,
                Z - coordinate.Z
            );

            var distance =
                (
                    Mathf.Abs(subtractedCoordinate.X) +
                    Mathf.Abs(subtractedCoordinate.Y) +
                    Mathf.Abs(subtractedCoordinate.Z)
                ) /
                2f;

            return (int)distance;
        }

        /// <summary>
        /// I will be honest, I don't know what I am writing but just transcribed the following.
        /// I mean, it looks like it's going to be correct, so I'll leave it at that.
        /// https://www.redblobgames.com/grids/hexagons/#range-coordinate
        /// </summary>
        public IEnumerable<Coordinate> WithinDistance(int distance)
        {
            var xInRangeStart = -distance;
            var xInRangeEnd = +distance;

            for (var inRangeX = xInRangeStart; inRangeX <= xInRangeEnd; inRangeX++)
            {
                var zInRangeStart = Mathf.Max(-distance, -inRangeX - distance);
                var zInRangeEnd = Mathf.Min(+distance, -inRangeX + distance);
                for (var inRangeZ = zInRangeStart; inRangeZ <= zInRangeEnd; inRangeZ++) yield return this + (inRangeX, inRangeZ);
            }
        }

        public bool IsOnSameLineWith(Coordinate coordinate) =>
            X == coordinate.X || Y == coordinate.Y || Z == coordinate.Z;

        public IEnumerable<Coordinate> GetCoordinatesBetween(Coordinate coordinate)
        {
            if (!IsOnSameLineWith(coordinate))
            {
                Log.Error("This method only support coordinates on a straight line at the moment!");
                yield break;
            }

            if (Z == coordinate.Z)
            {
                var minX = Math.Min(X, coordinate.X);
                var maxX = Math.Max(X, coordinate.X);
                for (var xInBetween = minX + 1; xInBetween < maxX; xInBetween++)
                    yield return new Coordinate(xInBetween, Z);
            }
            else if (Z == coordinate.Z)
            {
                var minZ = Math.Min(Z, coordinate.Z);
                var maxZ = Math.Max(Z, coordinate.Z);
                for (var zInBetween = minZ + 1; zInBetween < maxZ; zInBetween++)
                    yield return new Coordinate(X, zInBetween);
            }
            else if (Y == coordinate.Y)
            {
                var minZ = Math.Min(Z, coordinate.Z);
                var maxZ = Math.Max(Z, coordinate.Z);
                var maxX = Math.Max(X, coordinate.X);
                for (var i = 1; i < maxZ - minZ; i++)
                {
                    var xInBetween = maxX - i;
                    var zInBetween = minZ + i;

                    yield return new Coordinate(xInBetween, zInBetween);
                }
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X * 397 ^ Z;
            }
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public static Coordinate operator +(Coordinate a, Coordinate b) => new(a.X + b.X, a.Z + b.Z);
        public static Coordinate operator -(Coordinate a, Coordinate b) => new(a.X - b.X, a.Z - b.Z);
        public static Coordinate operator *(Coordinate a, int i) => new(a.X * i, a.Z * i);
        public static Coordinate operator -(Coordinate c) => new(-c.X, -c.Z);

        [PublicAPI]
        public void Deconstruct(out int outX, out int outZ)
        {
            outX = x;
            outZ = z;
        }

        public static implicit operator Coordinate((int x, int z) tuple) => new(tuple.x, tuple.z);
    }
}