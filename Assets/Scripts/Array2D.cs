using System;
using System.Linq;
using UnityEngine;

#pragma warning disable CS0693

//based on https://github.com/ovsky/TArray/blob/main/Assets/Lab7/TArray/Scripts/Runtime/TArray.cs


[SerializeField]
public interface ITArray {
    public Array2D<T> Cast<T>();
    public Array2D<T> GetArray<T>();
}


[Serializable]
public struct NullableSerializationContainer<T> {
    public bool hasValue;
    public T value;


    public NullableSerializationContainer(T o) {
        if (o == null) {
            this.hasValue = false;
            this.value = default(T);
        } else {
            this.value = o;
            this.hasValue = true;
        }
    }
}

[Serializable]
public class Array2D<T> : ITArray, IEquatable<Array2D<T>> {

    [SerializeField] private Vector2Int m_size;
    [SerializeField, HideInInspector] private NullableSerializationContainer<T>[] m_data;


    public static implicit operator T[](Array2D<T> array) => array.m_data.Select(w => w.hasValue ? w.value : default).ToArray();
    public static implicit operator T[,](Array2D<T> array) => array.Matrix;


    public Vector2Int Size => m_size;
    public int Width => Size.x;
    public int Height => Size.y;
    public int Length => m_data.Length;
    public T[] Array => m_data.Select(w => w.hasValue ? w.value : default).ToArray();


    public T[,] Matrix {
        get {
            T[,] matrix = new T[m_size.x, m_size.y];
            for (int i = 0; i < m_data.Length; i++) {
                matrix[i % m_size.x, i / m_size.x] = m_data[i].hasValue ? m_data[i].value : default;
            }
            return matrix;
        }
    }


    public Array2D(int x, int y) : this(new Vector2Int(x, y)) {

    }

    public Array2D(Vector2Int size) {
        this.m_size = size;
        m_data = new NullableSerializationContainer<T>[size.x * size.y];
    }

    public Array2D(T[,] matrix) {
        m_size = new Vector2Int(matrix.GetLength(0), matrix.GetLength(1));
        m_data = matrix.Cast<T>().Select<T, NullableSerializationContainer<T>>(w => new(w)).ToArray();
    }

    public Array2D(T[] array, Vector2Int size) {
        this.m_size = size;
        m_data = array.Select<T, NullableSerializationContainer<T>>(w => new(w)).ToArray();
    }



#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public T this[int x, int y] {
        get {
            if (x < 0 || x >= m_size.x || y < 0 || y >= m_size.y) {
                Debug.LogError($"TArray[{x}, {y}] is out of bounds of size: {m_size}");
                return default;
            }

            if (!m_data[(m_size.x * y) + x].hasValue) return default;
            return m_data[(m_size.x * y) + x].value;
        }
        set {
            if (x < 0 || x >= m_size.x || y < 0 || y >= m_size.y) {
                Debug.LogError($"TArray[{x}, {y}] is out of bounds of size: {m_size}");
                return;
            }
            m_data[(m_size.x * y) + x] = new(value);
        }
    }

#else

        public T this[int x, int y]
        {
            get => data[(size.x * y) + x];
            set => data[(size.x * y) + x] = value;
        }
#endif

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    public T Get(int x, int y) {
        if (x < 0 || x >= m_size.x || y < 0 || y >= m_size.y) {
            Debug.LogError($"TArray[{x}, {y}] is out of bounds of size: {m_size}");
            return default;
        }
        if (!m_data[(m_size.x * y) + x].hasValue) return default;
        return m_data[(m_size.x * y) + x].value;
    }

#else

        public T Get(int x, int y) => data[(size.x * y) + x];
        
#endif

    public T this[Vector2Int pos] {
        get => m_data[(m_size.x * pos.y) + pos.x].hasValue ? m_data[(m_size.x * pos.y) + pos.x].value : default;
        set => m_data[(m_size.x * pos.y) + pos.x] = new(value);
    }

    public Array2D<T1> Cast<T1>()
        => new Array2D<T1>(m_data.Cast<T1>().ToArray(), m_size);

    public Array2D<T> GetArray<T>()
        => new Array2D<T>(m_data?.Cast<T>()?.ToArray(), m_size);


    public bool Equals(Array2D<T> other) {
        if (m_size != other.m_size) {
            return false;
        }

        for (int i = 0; i < m_data.Length; i++) {
            if (!m_data[i].Equals(other.m_data[i])) {
                return false;
            }
        }

        return true;
    }
}

