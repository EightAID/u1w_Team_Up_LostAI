using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    [SerializeField] private Material m_Material;

    private Material[] _originMaterials;
    private Material[] _destinationMaterials;

    private const float MinDuration = 0.001f;
    [SerializeField][Min(MinDuration)]
    private float _duration = 2.0f;

    public float Duration
    {
        get
        {
            return _duration;
        }
        set
        {
            _duration = Mathf.Max(value, _duration);
        }
    }

    private Coroutine _coroutine;

    // Start is called before the first frame update
    void Start()
    {
        //それぞれのキャラクターでプロパティを制御するためにマテリアルを動的に生成
        _originMaterials = m_SkinnedMeshRenderer.materials;
        _destinationMaterials = new Material[m_SkinnedMeshRenderer.materials.Length];
        for(var materialIndex = 0; materialIndex < m_SkinnedMeshRenderer.sharedMaterials.Length; materialIndex++)
        {
            //動的にマテリアル生成
            _destinationMaterials[materialIndex] = new Material(m_Material);

            _destinationMaterials[materialIndex].SetTexture("_BaseMap", m_SkinnedMeshRenderer.sharedMaterials[materialIndex].GetTexture("_BaseMap"));
            _destinationMaterials[materialIndex].SetTexture("_BumpMap", m_SkinnedMeshRenderer.sharedMaterials[materialIndex].GetTexture("_BumpMap"));
        }
    }

    public void Play()
    {
       
        //マテリアルの変更
        m_SkinnedMeshRenderer.materials = _destinationMaterials;

        _coroutine = StartCoroutine(Run());
    }
    public void Stop()
    {
       if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

       m_SkinnedMeshRenderer.materials = _originMaterials;
    }
    private void SetCutoff(float value)
    {
        for(int materiaiIndex = 0;materiaiIndex < _destinationMaterials.Length;materiaiIndex++)
        {
            _destinationMaterials[materiaiIndex].SetFloat("_Cutoff", value);
        }
    }
    private IEnumerator Run()
    {
        float timer = 0;
        while(timer < _duration)
        {
            SetCutoff(timer/_duration);

            yield return null;

            timer += Time.deltaTime;
        }
    }
}