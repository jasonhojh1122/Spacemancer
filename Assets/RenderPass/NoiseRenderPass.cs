using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class NoiseRenderPass : ScriptableRenderPass
{
    Material material;
    RenderTargetIdentifier source;
    RenderTargetHandle tmpTexture;
    public NoiseRenderPass(Material material) : base()
    {
        this.material = material;
        tmpTexture.Init("_TempNoiseTexture");
    }

    public void SetSource(RenderTargetIdentifier source)
    {
        this.source = source;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("NoiseRendererFeature");

        RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
        cameraTextureDesc.depthBufferBits = 0;
        cmd.GetTemporaryRT(tmpTexture.id, cameraTextureDesc, FilterMode.Bilinear);

        Blit(cmd, source, tmpTexture.Identifier(), material, 0);
        Blit(cmd, tmpTexture.Identifier(), source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tmpTexture.id);
        base.FrameCleanup(cmd);
    }
}