using System;
using UnityEngine;
using static GridDoorset.Direction;

[Serializable]
public struct GridDoorset {
    [SerializeField][HideInInspector] private Direction m_doors;

    [Flags]
    [Serializable]
    public enum Direction {
        NONE = 0,
        NORTH = 0b0000_1000,
        EAST = 0b0000_0100,
        SOUTH = 0b0000_0010,
        WEST = 0b0000_0001,
        ALL = 0b0000_1111
    }

    public static implicit operator Direction(GridDoorset set) => set.m_doors;
    public static implicit operator byte(GridDoorset set) => (byte)(set.m_doors & ALL);
    public static implicit operator GridDoorset(byte b) => new GridDoorset((Direction)b);



    public static Direction Rotate90Clockwise(Direction direction) {
        byte db = (byte)direction;
        return (Direction)((db >> 1) | (db << 3)) & ALL;
    }

    public static Direction Invert(Direction direction) {
        byte db = (byte)direction;
        return (Direction)((db >> 2) | (db << 2)) & ALL;
    }

    public bool this[Direction index] {
        get => index switch {
            NORTH => North,
            EAST => East,
            SOUTH => South,
            WEST => West,
            _ => throw new Exception("cannot get multiple doors via indexing!")
        };
        set {
            var _ = index switch {
                NORTH => North = value,
                EAST => East = value,
                SOUTH => South = value,
                WEST => West = value,
                _ => throw new Exception("cannot set multiple doors via indexing!")
            };
        }
    }

    public GridDoorset(Direction b) {
        m_doors = b & ALL;
    }

    public GridDoorset Rotate90Clockwise() {
        return new GridDoorset(Rotate90Clockwise(m_doors));
    }

    public GridDoorset Invert() {
        return new GridDoorset(Invert(m_doors));
    }

    public bool North {
        get => (m_doors & NORTH) != 0;
        set {
            if (value) m_doors |= NORTH;
            else m_doors &= ~NORTH;
            m_doors &= ALL;
        }
    }

    public bool East {
        get => (m_doors & EAST) != 0;
        set {
            if (value) m_doors |= EAST;
            else m_doors &= ~EAST;
            m_doors &= ALL;
        }
    }

    public bool South {
        get => (m_doors & SOUTH) != 0;
        set {
            if (value) m_doors |= SOUTH;
            else m_doors &= ~SOUTH;
            m_doors &= ALL;
        }
    }

    public bool West {
        get => (m_doors & WEST) != 0;
        set {
            if (value) m_doors |= WEST;
            else m_doors &= ~WEST;
            m_doors &= ALL;
        }
    }
}
