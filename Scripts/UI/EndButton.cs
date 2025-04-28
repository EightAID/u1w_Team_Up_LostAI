using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndButton : MonoBehaviour
{
    [SerializeField] Image my_img;
    [SerializeField] Sprite my_sprite;
    [SerializeField] Sprite my_sprite_tisei;
    [SerializeField] public Color normalColor = Color.clear; // �C���X�y�N�^�[�Ŏw��
    [SerializeField] public Color targetColor = Color.clear; // �C���X�y�N�^�[�Ŏw��

    private void Update()
    {
        if(CardDataController.isCardUses[23])
        {
            my_img.sprite = my_sprite_tisei;
        }

        if (CardDataController.isCardUses[1])//���t�������Ă�����
        {
            my_img.sprite = my_sprite;
            return;
        }
    }

    public void SetButtonColor(bool i)
    {
        if(i)
        {
            my_img.color = targetColor;
        }
        else
        {
            my_img.color = normalColor;
        }
    }
}
