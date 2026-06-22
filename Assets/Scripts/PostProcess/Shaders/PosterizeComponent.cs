using System;
using UnityEngine.Rendering;

namespace Shaders {
    [Serializable]
    [DisplayInfo(name = "Posterize")]
    [VolumeComponentMenu("Krampus/Posterize")]
    public class PosterizeComponent : VolumeComponent, IPostProcessComponent {

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public FloatParameter levels = new FloatParameter(32f);
        public FloatParameter gammaBoost = new ClampedFloatParameter(1.8f, 0f, 10f);

        public bool IsActive() => intensity.value > 0f;
    }
}