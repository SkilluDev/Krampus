using System;

namespace KrampUtils {

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
}