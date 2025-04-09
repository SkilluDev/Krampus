using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Roomgen {

    public static class RoomVariantManager {
        private static List<RoomType> m_typeInstances;
        private static List<GameObject> m_prefabInstances;

        private static GameObject m_fakePrefabRoot;
        private static GameObject FakePrefabRoot {
            get {
                if (m_fakePrefabRoot == null) {
                    m_fakePrefabRoot = new GameObject("Fake Prefab Root");
                    m_fakePrefabRoot.SetActive(false);
                    Object.DontDestroyOnLoad(m_fakePrefabRoot);
                    //m_fakePrefabRoot.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_fakePrefabRoot;
            }
        }

        static RoomVariantManager() {
            Application.quitting += ReleaseAll;
            m_typeInstances = new List<RoomType>();
            m_prefabInstances = new List<GameObject>();
        }

        public static void ReleaseAll() {
            foreach (var type in m_typeInstances) {
                Object.Destroy(type.prefab);
                Object.Destroy(type);
            }
            m_prefabInstances.Clear();
            m_typeInstances.Clear();
            Debug.Log("Releasing Room Variants");
        }

        public static RoomType CreateRotatedInstance(RoomType original, int rotations) {
            var typeInstance = Object.Instantiate(original);
            var prefabInstance = Object.Instantiate(original.prefab, FakePrefabRoot.transform);
            var roomInstance = prefabInstance.GetComponent<Room>();

            typeInstance.prefab = prefabInstance;
            typeInstance.basedOn = original;
            roomInstance.m_type = typeInstance;
            prefabInstance.name = $"{original.prefab.name} (R{rotations})";
            typeInstance.name = $"{original.name} (R{rotations})";
            for (int i = 0; i < rotations; i++) {
                typeInstance.ConstraintRot90Clockwise();
                roomInstance.Rotate90Clockwise();
            }
            m_typeInstances.Add(typeInstance);
            m_prefabInstances.Add(prefabInstance);
            return typeInstance;
        }

        public static void Release(RoomType type) {
            if (m_typeInstances.Contains(type)) {
                m_prefabInstances.Remove(type.prefab);
                m_typeInstances.Remove(type);

                Object.Destroy(type.prefab);
                Object.Destroy(type);
            }
        }
    }
}