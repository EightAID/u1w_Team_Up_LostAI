using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//色を白黒にするやつ
public class Glayscale : MonoBehaviour
{
    public static Glayscale Instance { get; private set; }

    [Header("マテリアル")]
    [SerializeField] Material GlayMaterial;
    [SerializeField] Material normalMaterial;

    [Header("ゲームオブジェクト")]
    [SerializeField] SpriteRenderer[] transforms;
    [Header("UI側")]
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
