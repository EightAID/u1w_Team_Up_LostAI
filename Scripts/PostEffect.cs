using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostEffect : ScriptableRendererFeature
{
    [System.Serializable]
    public class GrayscaleSetting
    {
        // レンダリングの実行タイミング
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    /// <summary>
    /// Grayscale実行Pass
    /// </summary>
    class GrayScalePass : ScriptableRenderPass
    {
        private readonly string profilerTag = "GrayScale Pass";

        public Material grayscaleMaterial; // グレースケール計算用マテリアル

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;

            // コマンドバッファ
            var cmd = CommandBufferPool.Get(profilerTag);

            // マテリアル実行
            cmd.Blit(cameraColorTarget, cameraColorTarget, grayscaleMaterial);

            context.ExecuteCommandBuffer(cmd);
        }
    }

    [SerializeField] private GrayscaleSetting settings = new GrayscaleSetting();
    private GrayScalePass scriptablePass;

    public override void Create()
    {
        var shader = Shader.Find("PostEffect/Grayscale");
        if (shader)
        {
            scriptablePass = new GrayScalePass();
            scriptablePass.grayscaleMaterial = new Material(shader);
            scriptablePass.renderPassEvent = settings.renderPassEvent;
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (scriptablePass != null && scriptablePass.grayscaleMaterial != null)
        {
            renderer.EnqueuePass(scriptablePass);
        }
    }
}
