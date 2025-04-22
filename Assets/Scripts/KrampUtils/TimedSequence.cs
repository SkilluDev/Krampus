using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace KrampUtils {

    public class TimedSequence<T> {
        private Dictionary<string, float> m_absolutes;
        private Dictionary<string, float> m_relatives;

        public void Init() {
            m_absolutes = new Dictionary<string, float>();
            m_relatives = new Dictionary<string, float>();

            var sequentials = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(w => w.FieldType == typeof(float) && w.GetCustomAttribute<SeqDurationAttribute>() != null)
                .OrderBy(w => w.GetCustomAttribute<SeqDurationAttribute>().Order);

            var absolutes = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(w => w.FieldType == typeof(float) && w.GetCustomAttribute<SeqAbsoluteAttribute>() != null);

            foreach (var w in absolutes) {
                m_absolutes.Add(w.Name, (float)w.GetValue(this));
                m_relatives.Add(w.Name, (float)w.GetValue(this));
            }

            float accum = 0;
            foreach (var w in sequentials) {
                float duration = (float)w.GetValue(this);
                accum += duration;
                m_absolutes.Add(w.Name, accum);
                m_relatives.Add(w.Name, duration);
            }
        }

        public float Beg(string nameof) {
            if (!m_absolutes.ContainsKey(nameof)) throw new Exception("Invalid timing");
            return m_absolutes[nameof] - m_relatives[nameof];
        }

        public float End(string nameof) {
            if (!m_absolutes.ContainsKey(nameof)) throw new Exception("Invalid timing");
            return m_absolutes[nameof];
        }

        public float Between(string from, string to) {
            if (!m_absolutes.ContainsKey(from) || !m_absolutes.ContainsKey(to)) throw new Exception("Invalid timing");
            return m_absolutes[to] - m_absolutes[from];
        }

        public float InverseLerp(string from, string to, float t) {
            return Mathf.InverseLerp(Beg(from), Beg(to), t);
        }

        public float InverseLerp(string from, float t) {
            return Mathf.InverseLerp(Beg(from), End(from), t);
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