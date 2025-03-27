using System;

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


    public static QuadDirection Invert(this QuadDirection direction) {
        byte db = (byte)direction;
        return (QuadDirection)((db >> 2) | (db << 2)) & QuadDirection.ALL;
    }

}