using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace KrampUtils {

    public class TimedSequence<T> {
        private readonly Dictionary<string, float> m_absolutes;

        public TimedSequence() {
            m_absolutes = new Dictionary<string, float>();

            var sequentials = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(w => w.FieldType == typeof(float) && w.GetCustomAttribute<SeqDurationAttribute>() != null)
                .OrderBy(w => w.GetCustomAttribute<SeqDurationAttribute>().Order);

            var absolutes = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(w => w.FieldType == typeof(float) && w.GetCustomAttribute<SeqAbsoluteAttribute>() != null);

            foreach (var w in absolutes) {
                m_absolutes.Add(w.Name, (float)w.GetValue(this));
            }

            float accum = 0;
            foreach (var w in sequentials) {
                accum += (float)w.GetValue(this);
                m_absolutes.Add(w.Name, accum);
            }
        }

        public float this[string nameof] {
            get => GetAbsolute(nameof);
        }

        public float GetAbsolute(string nameof) {
            if (!m_absolutes.ContainsKey(nameof)) throw new Exception("Invalid timing");
            return m_absolutes[nameof];
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SeqDurationAttribute : Attribute {
        public int Order { get; }
        public SeqDurationAttribute([CallerLineNumber] int order = 0) {
            Order = order;
        }

    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SeqAbsoluteAttribute : Attribute {
        public SeqAbsoluteAttribute() {
        }

    }

}