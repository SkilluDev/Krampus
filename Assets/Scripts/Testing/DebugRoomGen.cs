using System.Collections;
using System.Collections.Generic;
using Roomgen;
using UnityEngine;

namespace KrampTests {

    public class DebugRoomGen : RoomGeneratorBase {
        public override IReadOnlyCollection<Room> Rooms => throw new System.NotImplementedException();

        public override void Cleanup() => throw new System.NotImplementedException();
        public override IEnumerator Generate() => throw new System.NotImplementedException();
        public override Room GetRoomAt(Vector3 position) => throw new System.NotImplementedException();
        public override void Prepare() => throw new System.NotImplementedException();
    }

}
