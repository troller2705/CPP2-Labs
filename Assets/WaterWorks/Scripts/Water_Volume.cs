using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule; // Required for RenderGraph APIs
using UnityEngine.Experimental.Rendering;


public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public TextureHandle source;

        private Material _material;

        public CustomRenderPass(Material mat)
        {
            _material = mat;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_material == null)
                return;

            var cameraData = frameData.Get<UniversalCameraData>();

            if (cameraData.cameraType == CameraType.Reflection)
                return;

            // Create temporary texture
            TextureHandle tempRT = renderGraph.CreateTexture(
                new TextureDesc(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height)
                {
                    name = "_TempRT",
                    colorFormat = GraphicsFormatUtility.GetGraphicsFormat(
                        cameraData.cameraTargetDescriptor.colorFormat,
                        RenderTextureReadWrite.Default
                    ),
                    depthBufferBits = DepthBits.None,
                    filterMode = FilterMode.Bilinear
                }
            );

            // Add Render Pass
            using (var builder = renderGraph.AddRenderPass<PassData>("Water Volume Pass", out var passData))
            {
                passData.source = renderGraph.ImportTexture(cameraData.renderer.cameraColorTargetHandle);
                passData.tempRT = tempRT;
                passData.material = _material;

                builder.ReadTexture(passData.source);
                builder.WriteTexture(passData.tempRT);

                builder.SetRenderFunc((PassData data, RenderGraphContext ctx) =>
                {
                    CommandBuffer cmd = ctx.cmd;
                    Blitter.BlitCameraTexture(cmd, data.source, data.tempRT, data.material, 0);
                    Blitter.BlitCameraTexture(cmd, data.tempRT, data.source);
                });
            }
        }

        private class PassData
        {
            public TextureHandle source;
            public TextureHandle tempRT;
            public Material material;
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Reflection)
            return;

        renderer.EnqueuePass(m_ScriptablePass);
    }
}
