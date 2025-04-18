using System;
using UnityEngine;

namespace KrampUtils {

    [Flags]
    [Serializable]
    public enum QuadDirection {
        NONE = 0,
        NORTH = 0b0000_1000,
        EAST = 0b0000_0100,
        SOUTH = 0b0000_0010,
        WEST = 0b0000_0001,
        ALL = 0b0000_1111
    }

    public static class DirectionMethods {
        public static QuadDirection Rotate90Clockwise(this QuadDirection direction) {
            byte db = (byte)direction;
            return (QuadDirection)((db >> 1) | (db << 3)) & QuadDirection.ALL;
        }

        public static QuadDirection Rotate90Clockwise(this QuadDirection direction, int times) {
            var tmp = direction;
            for (int i = 0; i < times % 4; i++) tmp = tmp.Rotate90Clockwise();
            return tmp;
        }


        public static QuadDirection Invert(this QuadDirection direction) {
            byte db = (byte)direction;
            return (QuadDirection)((db >> 2) | (db << 2)) & QuadDirection.ALL;
        }

        public static Vector3 XZ(this QuadDirection direction) {
            return new Vector3(
                (direction.HasFlag(QuadDirection.EAST) ? 1 : 0) +
                (direction.HasFlag(QuadDirection.WEST) ? -1 : 0),
                0,
                (direction.HasFlag(QuadDirection.NORTH) ? 1 : 0) +
                (direction.HasFlag(QuadDirection.SOUTH) ? -1 : 0)
            );
        }

        public static Vector2Int IJ(this QuadDirection direction) {
            return new Vector2Int(
                (direction.HasFlag(QuadDirection.EAST) ? 1 : 0) +
                (direction.HasFlag(QuadDirection.WEST) ? -1 : 0),
                (direction.HasFlag(QuadDirection.NORTH) ? -1 : 0) +
                (direction.HasFlag(QuadDirection.SOUTH) ? 1 : 0)
            );
        }

        public static Quaternion YRotation(this QuadDirection direction) {
            return Quaternion.LookRotation(XZ(direction), Vector3.up);
        }

        public static readonly QuadDirection[] CARDINALS = { QuadDirection.NORTH, QuadDirection.EAST, QuadDirection.SOUTH, QuadDirection.WEST };
    }
}