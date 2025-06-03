using UnityEngine;
using KrampUtils;

namespace Roomgen {
    [System.Serializable]
    public class DoorPropGroups : QuadDirectional<DoorPropGroups, PropGroup> {
        public bool phantom = false;
        [SerializeField] private PropGroup m_north, m_east, m_south, m_west; // need that for serializing??
        public override PropGroup North {
            get => m_north;
            set => m_north = value;
        }
        public override PropGroup East {
            get => m_east;
            set => m_east = value;
        }
        public override PropGroup South {
            get => m_south;
            set => m_south = value;
        }
        public override PropGroup West {
            get => m_west;
            set => m_west = value;
        }


        public void SetState(QuadDirection dir) {
            foreach (var d in DirectionMethods.CARDINALS) {
                if (this[d] != null) this[d].SetState(!dir.HasFlag(d));
            }
        }

        internal void Destroy(QuadDirection dir = QuadDirection.ALL) {
            foreach (var d in DirectionMethods.CARDINALS) {
                if (this[d] != null && dir.HasFlag(d)) Object.DestroyImmediate(this[d].gameObject);
            }
        }

    }
}