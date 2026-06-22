using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Shaders {
    [Serializable]
    [DisplayInfo(name = "Outline")]
    [VolumeComponentMenu("Krampus/Outline")]
    public class OutlineComponent : VolumeComponent, IPostProcessComponent {
        public ClampedFloatParameter outlineIntensity = new ClampedFloatParameter(0.0f, 0.0f, 5.0f);
        public ClampedFloatParameter outlineNoiseIntensity = new ClampedFloatParameter(0.0f, 0.0f, 0.1f);
        public ClampedFloatParameter outlineSecondary = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);
        public ClampedFloatParameter outlineNoiseScale = new ClampedFloatParameter(0.0f, 0.0f, 0.1f);
        public ColorParameter outlineColor = new ColorParameter(Color.black);
        public ClampedFloatParameter outlineThickness = new ClampedFloatParameter(0.0f, 0.0f, 5.0f);
        public ClampedFloatParameter outlineDepthSensitivity = new ClampedFloatParameter(0.0f, 0.0f, 100.0f);

        public bool IsActive() => outlineIntensity.value > 0.0f;
    }
}