using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�F�𔒍��ɂ�����
public class Glayscale : MonoBehaviour
{
    public static Glayscale Instance { get; private set; }

    [Header("�}�e���A��")]
    [SerializeField] Material GlayMaterial;
    [SerializeField] Material normalMaterial;

    [Header("�Q�[���I�u�W�F�N�g")]
    [SerializeField] SpriteRenderer[] transforms;
    [Header("UI��")]
    [SerializeField] Image[] rectTransforms;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (CardDataController.isCardUses[14]) SetGlayscale();
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetGlay(Image img)
    {
        img.material = GlayMaterial;
    }

    public void CancellGlay(Image img)
    {
        img.material = null;
    }

    public void SetGlayscale() 
    {
        foreach (SpriteRenderer t in transforms)
        {
            t.material = GlayMaterial;
        }

        foreach(Image t in rectTransforms)
        {
            t.material = GlayMaterial;
        }
    }

    public void CancellGlayScale()
    {
        foreach (SpriteRenderer t in transforms)
        {
            t.material = normalMaterial;
        }

        foreach (Image t in rectTransforms)
        {
            t.material = null;
        }
    }

}
