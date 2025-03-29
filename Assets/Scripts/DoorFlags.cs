using System;
using UnityEngine;
using static QuadDirection;

[Serializable]
public class DoorFlags : QuadDirectional<DoorFlags, bool> {
    [SerializeField][HideInInspector] private QuadDirection m_doors;

    public static implicit operator QuadDirection(DoorFlags set) => set.m_doors;
    public static implicit operator byte(DoorFlags set) => (byte)(set.m_doors & ALL);
    public static implicit operator DoorFlags(byte b) => new DoorFlags((QuadDirection)b);

    public DoorFlags() : this(NONE) { }

    public DoorFlags(QuadDirection b) {
        m_doors = b & ALL;
    }

    public override DoorFlags Rotate90Clockwise() {
        return new DoorFlags(m_doors.Rotate90Clockwise());
    }

    public override DoorFlags Invert() {
        return new DoorFlags(m_doors.Invert());
    }


    public override bool North {
        get => m_doors.HasFlag(NORTH);
        set {
            if (value) m_doors |= NORTH;
            else m_doors &= ~NORTH;
            m_doors &= ALL;
        }
    }

    public override bool East {
        get => m_doors.HasFlag(EAST);
        set {
            if (value) m_doors |= EAST;
            else m_doors &= ~EAST;
            m_doors &= ALL;
        }
    }

    public override bool South {
        get => m_doors.HasFlag(SOUTH);
        set {
            if (value) m_doors |= SOUTH;
            else m_doors &= ~SOUTH;
            m_doors &= ALL;
        }
    }

    public override bool West {
        get => m_doors.HasFlag(WEST);
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
