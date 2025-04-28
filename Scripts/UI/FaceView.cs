using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//プレイヤーの表情
public class FaceView : MonoBehaviour
{
    public static FaceView Instance { get; private set; }

    [Header("表情")]
    [SerializeField] private Image[] PlayerImg;
    [SerializeField] private Image[] HandImg;
    [SerializeField] private Image[] OptionImg;

    [Header("表情素材")]
    [SerializeField] private Sprite[] bodySprites;
    [SerializeField] private Sprite[] handSprites;
    [SerializeField] private Sprite[] optionSprites;

    [Header("腕隠し")]
    [SerializeField] GameObject[] handHideObj;

    /// <summary>
    /// 表情差分
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
    [Header("表情状態")]
    [SerializeField] public FaceState faceState = FaceState.NORMAL;

    /// <summary>
    /// 腕の差分
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
    /// 顔の表情を設定する関数
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



            //特殊
            if (CardDataController.isCardUses[11])//思考力
            {
                PlayerImg[i].sprite = bodySprites[(int)FaceState.GURUME];
            }

            if (CardDataController.isCardUses[12])//手
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
