using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomBloomRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public Settings settings = new Settings();
    Start_CustomBloomRenderPass m_BloomPass;

    public override void Create()
    {
        m_BloomPass = new Start_CustomBloomRenderPass(settings); 
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_BloomPass);
    }
}
