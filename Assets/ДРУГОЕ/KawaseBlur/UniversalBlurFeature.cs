using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Unified.Universal.Blur {
    public class UniversalBlurFeature : ScriptableRendererFeature {
        public enum InjectionPoint {
            BeforeRenderingTransparents = RenderPassEvent.BeforeRenderingTransparents,
            BeforeRenderingPostProcessing = RenderPassEvent.BeforeRenderingPostProcessing,
            AfterRenderingPostProcessing = RenderPassEvent.AfterRenderingPostProcessing
        }

        public Material passMaterial;
        [HideInInspector] public int passIndex = 0;

        [Header("Blur Settings")]
        public InjectionPoint injectionPoint = InjectionPoint.AfterRenderingPostProcessing;

        [Space]
        [Range(0f, 1f)] public float intensity = 1.0f;
        [Range(1f, 10f)] public float downsample = 2.0f;
        [Range(0f, 5f)] public float scale = .5f;
        [Range(1, 20)] public int iterations = 6;


        private UniversalBlurPass _fullScreenPass;
        private bool _requiresColor, _injectedBeforeTransparents;
        private UniversalBlurPass.PassData _PassData;


        /// <inheritdoc/>
        public override void Create() {
            _fullScreenPass = new UniversalBlurPass {
                renderPassEvent = (RenderPassEvent)injectionPoint
            };

            ScriptableRenderPassInput modifiedRequirements = ScriptableRenderPassInput.Color;

            _requiresColor = true;
            _injectedBeforeTransparents = injectionPoint <= InjectionPoint.BeforeRenderingTransparents;

            if (_requiresColor && !_injectedBeforeTransparents)
                modifiedRequirements ^= ScriptableRenderPassInput.Color;

            _fullScreenPass.ConfigureInput(modifiedRequirements);

            _PassData = new();
        }


        /// <inheritdoc/>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if (passMaterial == null) {
                Debug.LogWarningFormat("Missing Post Processing effect Material. {0} Fullscreen pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
                return;
            }

            SetupPassData(_PassData);
            _fullScreenPass.Setup(_PassData, downsample, renderingData);

            renderer.EnqueuePass(_fullScreenPass);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing) {
            _fullScreenPass.Dispose();
        }

        void SetupPassData(UniversalBlurPass.PassData passData) {
            passData.effectMaterial = passMaterial;
            passData.intensity = intensity;
            passData.passIndex = passIndex;
            passData.requiresColor = _requiresColor;
            passData.scale = scale;
            passData.iterations = iterations;
        }
    }

}