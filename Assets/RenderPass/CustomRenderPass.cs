using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class CustomRenderPass : ScriptableRenderPass
{
    Material toonMat, outlineMat, glitchMat;
    RenderTargetIdentifier source;
    RenderTargetHandle tmpTexture1;
    RenderTargetHandle tmpTexture2;

    public CustomRenderPass(Material toonMat, Material outlineMat, Material glitchMat) : base()
    {
        this.toonMat = toonMat;
        this.outlineMat = outlineMat;
        this.glitchMat = glitchMat;
        tmpTexture1.Init("_TmpTexture1");
        tmpTexture2.Init("_TmpTexture2");
    }

    public void SetSource(RenderTargetIdentifier source)
    {
        this.source = source;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("CustomRendererFeature");

        RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
        cameraTextureDesc.depthBufferBits = 0;
        cmd.GetTemporaryRT(tmpTexture1.id, cameraTextureDesc, FilterMode.Point);
        cmd.GetTemporaryRT(tmpTexture2.id, cameraTextureDesc, FilterMode.Point);

        Blit(cmd, source, tmpTexture1.Identifier(), toonMat, 0);
        Blit(cmd, tmpTexture1.Identifier(), tmpTexture2.Identifier(), outlineMat, 0);
        Blit(cmd, tmpTexture2.Identifier(), source, glitchMat, 0);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tmpTexture1.id);
        cmd.ReleaseTemporaryRT(tmpTexture2.id);
        base.FrameCleanup(cmd);
    }
}