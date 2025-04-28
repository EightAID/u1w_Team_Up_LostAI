using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//�v���C���[�̕\��
public class FaceView : MonoBehaviour
{
    public static FaceView Instance { get; private set; }

    [Header("�\��")]
    [SerializeField] private Image[] PlayerImg;
    [SerializeField] private Image[] HandImg;
    [SerializeField] private Image[] OptionImg;

    [Header("�\��f��")]
    [SerializeField] private Sprite[] bodySprites;
    [SerializeField] private Sprite[] handSprites;
    [SerializeField] private Sprite[] optionSprites;

    [Header("�r�B��")]
    [SerializeField] GameObject[] handHideObj;

    /// <summary>
    /// �\���
    /// </summary>
    public enum FaceState
    {
        NORMAL,
        ASERI,
        HUSEME,
        ZETUBOU,
        METOZI,
        GURUME,
        EGAO,
    }
    [Header("�\����")]
    [SerializeField] public FaceState faceState = FaceState.NORMAL;

    /// <summary>
    /// �r�̍���
    /// </summary>
    public enum ArmState
    {
        NULL,
        DOWNUP,
        UPUP,
        DOWNDOWN,
    }
    [SerializeField] public ArmState armState = ArmState.DOWNUP;

    public enum OptionState
    {
        NULL,
        NONE,
        SIROME,
        KUROME,
        YOGORE,
    }
    [SerializeField] public OptionState optionState = OptionState.NONE;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// ��̕\���ݒ肷��֐�
    /// </summary>
    /// <param name="face"></param>
    /// <param name="arm"></param>
    /// <param name="option"></param>
    public void SetFaceView(FaceState face = FaceState.NORMAL,ArmState arm = ArmState.NULL,OptionState option = OptionState.NULL)
    {
        for(int i = 0; i < PlayerImg.Length; i++)
        {
            PlayerImg[i].sprite = bodySprites[(int)face];
            if (arm != ArmState.NULL) HandImg[i].sprite = handSprites[(int)arm - 1];
            if (option != OptionState.NULL) OptionImg[i].sprite = optionSprites[(int)option - 1];



            //����
            if (CardDataController.isCardUses[11])//�v�l��
            {
                PlayerImg[i].sprite = bodySprites[(int)FaceState.GURUME];
            }

            if (CardDataController.isCardUses[12])//��
            {
                HandImg[i].sprite = handSprites[(int)ArmState.DOWNDOWN - 1];
                if (handHideObj != null)
                {
                    for(int j = 0; j < handHideObj.Length; j++)
                    {
                        handHideObj[j].SetActive(true);
                    }
                } 
            }
        }

    }

    public void SetOption(OptionState option = OptionState.NULL)
    {
        for (int i = 0; i < PlayerImg.Length; i++)
        {
            if (option != OptionState.NULL) OptionImg[i].sprite = optionSprites[(int)option - 1];
        }
    }
}
