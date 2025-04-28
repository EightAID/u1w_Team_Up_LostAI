using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

//主人公を管理する
public class PlayerController : MonoBehaviour
{
    [Header("主人公")]
    [SerializeField] RectTransform player_rt;
    private Vector2 originalPos;

    [Header("表情差分")]
    [SerializeField] Sprite[] player_sprite;

    [Header("UI")]
    [SerializeField] GameObject HeartObj;
    [SerializeField] TextMeshProUGUI lifeText;
    [SerializeField] RectTransform CanvasRT;

    [Header("呼吸の揺れ")]
    public float moveAmount = 10f;     // 上下の移動量
    public float duration = 2f;        // 1往復の時間

    public int playerHaert = 20;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = player_rt.anchoredPosition;

        StartBreathing();
        UIController.Instance.RenderText(lifeText, playerHaert.ToString());

        UIController.Instance.SetSankakuReathing();
    }

    /// <summary>
    /// ダメージを受けたときの関数
    /// </summary>
    public void Damage(int damage)
    {
        if (GameController.Instance.isKanasimi)//悲しみの効果時
        {
            GameController.Instance.isKanasimi = false;
            int dp = Mathf.Max(0, damage - 10);
            SetDamageText(dp);
            playerHaert = Mathf.Max(0, playerHaert - dp);
        }
        else
        {
            SetDamageText(damage);
            playerHaert = Mathf.Max(0, playerHaert - damage);
        }


        SoundController.Instance.PlaySFX(5);
        UIController.Instance.UIShake(CanvasRT, 0.1f, 40);
    }

    public void Health(int health)
    {
        if (CardDataController.isCardUses[28]) return;
        SetHealthText(health);
        SoundController.Instance.PlaySFX(9);
        playerHaert = Mathf.Max(0, playerHaert + health);
        if (CardDataController.isCardUses[5])//名前   
        {
            if (playerHaert > 40)
            {
                playerHaert = 40;
            }
        }
        else
        {
            if (playerHaert > 20)
            {
                playerHaert = 20;
            }
        }
    }

    async public UniTask SetFace(int delay = 0)
    {
        await UniTask.Delay(delay);
        FaceView.Instance.SetFaceView(FaceView.FaceState.NORMAL);
        if (playerHaert <= 15)
        {
            FaceView.Instance.SetFaceView(FaceView.FaceState.ASERI);
        }
        if (playerHaert <= 10)
        {
            FaceView.Instance.SetFaceView(FaceView.FaceState.HUSEME);
        }
        if (playerHaert <= 5)
        {
            FaceView.Instance.SetFaceView(FaceView.FaceState.ZETUBOU);
        }

        //特殊系

        //視覚
        if (CardDataController.isCardUses[2])
        {
            FaceView.Instance.SetFaceView(FaceView.FaceState.NORMAL,FaceView.ArmState.NULL,FaceView.OptionState.KUROME);
        }
    }

    public void SetDamageText(int damage)
    {
        //痛覚なしなら
        if (CardDataController.isCardUses[7])
        {
            GameController.Instance.SerifCutIn("全然痛くない！！");
            FaceView.Instance.SetFaceView(FaceView.FaceState.EGAO,FaceView.ArmState.UPUP,FaceView.OptionState.SIROME);
            return;
        }
        UIController.Instance.TakeDamegeView(lifeText, playerHaert, damage, 0.5f);
        if (playerHaert <= 0) UIController.Instance.SetObj(HeartObj, false);
        FaceView.Instance.SetFaceView(FaceView.FaceState.METOZI);
        SetFace(500).Forget();
    }


    /// <summary>
    /// 回復するUIの関数
    /// </summary>
    /// <param name="health"></param>
    public void SetHealthText(int health)
    {
        if (CardDataController.isCardUses[7]) return;//痛覚
        UIController.Instance.TakeDamegeView(lifeText, playerHaert, -health, 0.5f);
        if (playerHaert <= 0) UIController.Instance.SetObj(HeartObj, false);
        //笑顔切ってるなら
        if(CardDataController.isCardUses[4])
        {
            FaceView.Instance.SetFaceView(FaceView.FaceState.NORMAL);
        }
        else
        {
            FaceView.Instance.SetFaceView(FaceView.FaceState.EGAO);
        }
        SetFace(500).Forget();
    }

    //揺れる
    public void StartBreathing()
    {
        player_rt.DOAnchorPosY(originalPos.y + moveAmount, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBreathing()
    {
        player_rt.DOKill(); // アニメーション停止
        player_rt.anchoredPosition = originalPos; // 元の位置に戻す
    }

    public void ResetPlayer()
    {
        UIController.Instance.SetObj(HeartObj, true);
        UIController.Instance.RenderText(lifeText, playerHaert.ToString());
    }
}
