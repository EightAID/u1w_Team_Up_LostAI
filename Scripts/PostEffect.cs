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
        // �����_�����O�̎��s�^�C�~���O
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    /// <summary>
    /// Grayscale���sPass
    /// </summary>
    class GrayScalePass : ScriptableRenderPass
    {
        private readonly string profilerTag = "GrayScale Pass";

        public Material grayscaleMaterial; // �O���[�X�P�[���v�Z�p�}�e���A��

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;

            // �R�}���h�o�b�t�@
            var cmd = CommandBufferPool.Get(profilerTag);

            // �}�e���A�����s
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
