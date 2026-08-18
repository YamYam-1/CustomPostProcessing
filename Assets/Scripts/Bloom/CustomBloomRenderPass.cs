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
        public Material material;
    }
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var temp = frameData.GetOrCreate<TempTexture>();

        var resourceData = frameData.Get<UniversalResourceData>();
        TextureDesc desc = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
        desc.name = "Bloom";
        desc.clearBuffer = false;
        desc.width = desc.width / 2;
        desc.height = desc.height / 2;

        RenderTextureDescriptor textureProperties = new RenderTextureDescriptor(desc);
        temp.texture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureProperties, "Down Sampling", false);


        using var builder = renderGraph.AddRasterRenderPass<PassData>("Custom Bloom Render Pass", out var passData);

        passData.source = resourceData.activeColorTexture;

        builder.UseTexture(passData.source, AccessFlags.Read);
        builder.SetRenderAttachment(temp.texture, 0, AccessFlags.Write);

        resourceData.cameraColor = temp.texture;

        builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
        {
            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
        });
    
    }

    // Temp -> Destination (Draw)
    public class End_CustomBloomRenderPass : ScriptableRenderPass
    {
        
    }


}