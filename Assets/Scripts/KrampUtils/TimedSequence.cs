using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace KrampUtils {

    public class TimedSequence<T> {
        private Dictionary<int, float> m_absolutes;


        public TimedSequence(object t) {
            // reflection magic
        }

        public float GetAbsolute(string nameof) {
            return 0;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SeqDuration : Attribute {
        public int Order { get; }
        public SeqDuration([CallerLineNumber] int order = 0) {
            Order = order;
        }

    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SeqAbsolute : Attribute {
        public SeqAbsolute() {
        }

    }

}