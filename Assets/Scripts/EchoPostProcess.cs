using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(EchoPostProcessRenderer), PostProcessEvent.AfterStack, "TheGPUMan/ECHO")]
public class EchoPostProcess : PostProcessEffectSettings
{
    [Tooltip("Displays the Echo Effects in debug view.")]
    public BoolParameter DebugView = new BoolParameter { value = false };

    [Range(0f, 1f), Tooltip("Effect opacity.")]
    public FloatParameter Opacity = new FloatParameter { value = 1f };

    [Tooltip("Color multiplier applied to the ECHO main color.")]
    public FloatParameter HDRMultiplier = new FloatParameter { value = 1f };
}

public class EchoPostProcessRenderer : PostProcessEffectRenderer<EchoPostProcess>
{
    private static readonly int sOpacityID = Shader.PropertyToID("_Opacity");
    private static readonly int sHDRMultiplier = Shader.PropertyToID("_HDRMultiplier");

    private int mGlobalEchoTexID;
    private Shader mEchoShader;

    public override DepthTextureMode GetCameraFlags()
    {
        return DepthTextureMode.Depth;
    }

    public override void Init()
    {
        mGlobalEchoTexID = Shader.PropertyToID("_GlobalEchoTex");
        mEchoShader = Shader.Find("Hidden/TheGPUMan/ECHO");
        base.Init();
    }

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(mEchoShader);

        if (!settings.DebugView)
        {
            context.command.GetTemporaryRT(mGlobalEchoTexID,
                context.camera.pixelWidth,
                context.camera.pixelHeight,
                0, FilterMode.Point, RenderTextureFormat.ARGB32);
            context.command.SetRenderTarget(mGlobalEchoTexID);
            context.command.ClearRenderTarget(false, true, Color.clear);
        }

        sheet.properties.SetFloat(sOpacityID, settings.Opacity);
        sheet.properties.SetFloat(sHDRMultiplier, settings.HDRMultiplier);

        EchoManager.Instance.PopulateCommandBuffer(context.command);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}