﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source;

        private Material _material;

        private RTHandle tempRenderTarget;
        private RTHandle tempRenderTarget2;

        public CustomRenderPass(Material mat)
        {
            _material = mat;

            tempRenderTarget = RTHandles.Alloc("_TemporaryColourTexture", name: "_TemporaryColourTexture");
            tempRenderTarget2 = RTHandles.Alloc("_TemporaryDepthTexture", name: "_TemporaryDepthTexture");
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get();

                // Get RTHandles for source and temporary render target
                RTHandle tempRT = RTHandles.Alloc(
                    renderingData.cameraData.cameraTargetDescriptor,
                    name: "_TempRT"
                );

                // Perform the blits using RTHandles
                Blitter.BlitCameraTexture(commandBuffer, source, tempRT, _material, 0);
                Blitter.BlitCameraTexture(commandBuffer, tempRT, source);

                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);

                // Release the temporary RTHandle
                tempRT.Release();
            }
        }



        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    [System.Serializable]
    public class _Settings
    {
        //[HideInInspector]
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if(settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);

        // Configures where the render pass should be injected.
        //m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Use cameraColorTargetHandle instead of cameraColorTarget
        m_ScriptablePass.source = renderer.cameraColorTargetHandle;

        renderer.EnqueuePass(m_ScriptablePass);
    }
}


