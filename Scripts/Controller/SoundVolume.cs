using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolume : MonoBehaviour
{
    [Header("Slider")]
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    [SerializeField] float slider_offset = 1;

    private void Start()
    {
        SetSlider();
        ChangeBGMSlider();
        ChangeSESlider();
    }

    public void ChangeBGMSlider()
    {
        SoundController.Instance.SetVolume();
        if (BGMSlider == null || SESlider == null) return;
        SoundController.bgm_volume = BGMSlider.value * slider_offset;
        SoundController.Instance.SetVolume();
    }

    public void ChangeSESlider()
    {
        SoundController.Instance.SetVolume();
        if (BGMSlider == null || SESlider == null) return;
        SoundController.se_volume = SESlider.value * slider_offset;
        SoundController.Instance.SetVolume();
    }


    public void SetSlider()
    {
        if (BGMSlider == null || SESlider == null) return;
        BGMSlider.value = SoundController.bgm_volume /slider_offset;
        SESlider.value = SoundController.se_volume /slider_offset;
    }

}
