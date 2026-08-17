using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;

public class TempTexture : ContextItem
{
    public TextureHandle texture = TextureHandle.nullHandle;

    public override void Reset()
    {
        texture = TextureHandle.nullHandle;
    }

}

// Source -> Temp (DownSmapling)
public class Start_CustomBloomRenderPass : ScriptableRenderPass
{
    
    CustomBloomRenderFeature.Settings settings;
    public Start_CustomBloomRenderPass(CustomBloomRenderFeature.Settings settings)
    {
        this.settings = settings;
        renderPassEvent = settings.passEvent;

    }

    class PassData
    {
        public TextureHandle source;
    }
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {

        var tempTexExist = frameData.Contains<TempTexture>();
        var temp = frameData.GetOrCreate<TempTexture>();

        // First time running this pass.
        if(!tempTexExist)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            temp.texture = resourceData.activeColorTexture;
        }


        using var builder = renderGraph.AddRasterRenderPass<PassData>("Custom Bloom Render Pass", out var passData);

        passData.source = resourceData.activeColorTexture;

        
        TextureDesc desc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
        desc.name = "Bloom";
        desc.clearBuffer = false;

        RenderTextureDescriptor textureProperties = new RenderTextureDescriptor(desc.width / 2, desc.height / 2);

        TextureHandle destination = renderGraph.CreateTexture(desc);



        builder.UseTexture(passData.source, AccessFlags.Read);
        builder.UseTexture(temp, AccessFlags.Read);
        builder.SetRenderAttachment(temp, 0, AccessFlags.Write);
        builder.SetRenderAttachment(destination, 0, AccessFlags.Write);
    
    
    }

    // Temp -> Destination (Draw)
    public class End_CustomBloomRenderPass : ScriptableRenderPass
    {
        
    }


}