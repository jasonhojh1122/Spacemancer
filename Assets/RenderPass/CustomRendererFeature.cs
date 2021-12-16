using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CustomRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FeatureSettings
    {
        public bool IsEnabled = true;
        public RenderPassEvent insertOn = RenderPassEvent.AfterRendering;
        public Material toonMaterial;
        public Material outlineMaterial;
        public Material glitchMaterial;
    }

    [SerializeField] FeatureSettings settings = new FeatureSettings();

    RenderTargetHandle renderTargetHandle;
    CustomRenderPass renderPass;

    public override void Create()
    {
        renderPass = new CustomRenderPass(settings.toonMaterial, settings.outlineMaterial, settings.glitchMaterial);
        renderPass.renderPassEvent = settings.insertOn;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.IsEnabled)
        {
            return;
        }
        renderPass.SetSource(renderer.cameraColorTarget);
        renderer.EnqueuePass(renderPass);
    }
}
