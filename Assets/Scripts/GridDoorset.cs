using System;
using UnityEngine;
using static QuadDirection;

[Serializable]
public class GridDoorset : QuadDirectional<GridDoorset, bool> {
    [SerializeField][HideInInspector] private QuadDirection m_doors;

    public static implicit operator QuadDirection(GridDoorset set) => set.m_doors;
    public static implicit operator byte(GridDoorset set) => (byte)(set.m_doors & ALL);
    public static implicit operator GridDoorset(byte b) => new GridDoorset((QuadDirection)b);

    public GridDoorset() : this(NONE) { }

    public GridDoorset(QuadDirection b) {
        m_doors = b & ALL;
    }

    public override GridDoorset Rotate90Clockwise() {
        return new GridDoorset(m_doors.Rotate90Clockwise());
    }

    public override GridDoorset Invert() {
        return new GridDoorset(m_doors.Invert());
    }


    public override bool North {
        get => (m_doors & NORTH) != 0;
        set {
            if (value) m_doors |= NORTH;
            else m_doors &= ~NORTH;
            m_doors &= ALL;
        }
    }

    public override bool East {
        get => (m_doors & EAST) != 0;
        set {
            if (value) m_doors |= EAST;
            else m_doors &= ~EAST;
            m_doors &= ALL;
        }
    }

    public override bool South {
        get => (m_doors & SOUTH) != 0;
        set {
            if (value) m_doors |= SOUTH;
            else m_doors &= ~SOUTH;
            m_doors &= ALL;
        }
    }

    public override bool West {
        get => (m_doors & WEST) != 0;
        set {
            if (value) m_doors |= WEST;
            else m_doors &= ~WEST;
            m_doors &= ALL;
        }
    }

    public int Count {
        get {
            int c = 0;
            if (North) c++;
            if (East) c++;
            if (South) c++;
            if (West) c++;
            return c;
        }
    }
}
