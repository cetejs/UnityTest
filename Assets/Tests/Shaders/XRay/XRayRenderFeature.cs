using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
    public LayerMask xrayLayerMask;
    public Material xrayMaterial;

    class CustomRenderPass : ScriptableRenderPass
    {
        private LayerMask layerMask;
        private Material material;
        private string profilerTag = "XRayPass";
        private ShaderTagId shaderTagId = new ShaderTagId("UniversalForward");

        public CustomRenderPass(LayerMask layerMask, Material material)
        {
            this.layerMask = layerMask;
            this.material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
            {
                return;
            }

            var cmd = CommandBufferPool.Get(profilerTag);
            using (new ProfilingScope(cmd, new ProfilingSampler(profilerTag)))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var drawSettings = CreateDrawingSettings(shaderTagId, ref renderingData, SortingCriteria.CommonOpaque);
                drawSettings.overrideMaterial = material;

                var filterSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
                var stateBlock = new RenderStateBlock(RenderStateMask.Depth)
                {
                    depthState = new DepthState()
                    {
                        writeEnabled = false, compareFunction = CompareFunction.Greater
                    }
                };

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings, ref stateBlock);
            }
        }
    }

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(xrayLayerMask, xrayMaterial);

        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}