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
        cmd.GetTemporaryRT(tmpTexture1.id, cameraTextureDesc, FilterMode.Bilinear);
        cmd.GetTemporaryRT(tmpTexture2.id, cameraTextureDesc, FilterMode.Bilinear);

        Blit(cmd, source, tmpTexture1.Identifier(), toonMat, 0);
        Blit(cmd, tmpTexture1.Identifier(), tmpTexture2.Identifier(), outlineMat, 0);
        Blit(cmd, tmpTexture2.Identifier(), tmpTexture1.Identifier(), glitchMat, 0);
        Blit(cmd, tmpTexture1.Identifier(), source);

        // var camera = renderingData.cameraData.camera;
        // cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
        // ApplyMat(cmd, source, toonTexture, cameraTextureDesc, toonMat);
        // ApplyMat(cmd, toonTexture.Identifier(), outlineTexture, cameraTextureDesc, outlineMat);
        // ApplyMat(cmd, outlineTexture.Identifier(), glitchTexture, cameraTextureDesc, glitchMat);
        // Blit(cmd, glitchTexture.Identifier(), source);
        // cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    void ApplyMat(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetHandle tmpTexture,
        RenderTextureDescriptor cameraTextureDesc, Material mat)
    {
        cmd.GetTemporaryRT(tmpTexture.id, cameraTextureDesc, FilterMode.Bilinear);
        cmd.SetRenderTarget(tmpTexture.Identifier());
        cmd.SetGlobalTexture("_MainTex", src);
        cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, mat);
        // Blit(cmd, src, tmpTexture.Identifier(), mat, 0);
    }

    void ApplyMat(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dst, Material mat)
    {
        cmd.SetRenderTarget(dst);
        cmd.SetGlobalTexture("_MainTex", src);
        cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, mat);
        // Blit(cmd, src, tmpTexture.Identifier(), mat, 0);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tmpTexture1.id);
        cmd.ReleaseTemporaryRT(tmpTexture2.id);
        base.FrameCleanup(cmd);
    }
}