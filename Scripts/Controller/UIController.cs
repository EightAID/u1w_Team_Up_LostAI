using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using static GameController;
using Unity.VisualScripting;
using System.Xml;

//UIの全てをつかさどる
public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("カメラ")]
    [SerializeField] Transform CameraTransform;

    [Header("セリフ枠")]
    [SerializeField] GameObject serifBorderObj;
    [SerializeField] public TextMeshProUGUI serifText;
    [SerializeField] public TextMeshProUGUI thinkText;
    [SerializeField] public TextMeshProUGUI think2Text;
    [SerializeField] Image SerifSankaku;

    private Coroutine typingCoroutine;

    [Header("トランジション")]
    [SerializeField] GameObject FadeTransitionObj;
    [SerializeField] UIEffect FadeTransitionEffect;
    [Header("最後のトランジション")]
    [SerializeField] GameObject FadeTransitionFlashObj;
    [SerializeField] UIEffect FadeTransitionFlashEffect;
    [SerializeField] GameObject LoveTransitonObj;
    [SerializeField] UIEffect LoveTransitonEffect;
    [Header("真エンドのトランジション")]
    [SerializeField] GameObject FadeTransitionEnding;
    [SerializeField] UIEffect FadeTransitionEndingEffect;
    [SerializeField] GameObject LastFadeObj;
    [SerializeField] UIEffect FadeTransLastEffect;
    [SerializeField] GameObject LastItiranObj;
    [SerializeField] TextMeshProUGUI lostUIText;
    [SerializeField] TextMeshProUGUI NokoriUIText;
    [Header("0枚エンドのトランジション")]
    [SerializeField] GameObject LostFadeObj;
    [SerializeField] UIEffect LostFadeTransLastEffect;
    [SerializeField] RectTransform Lostimg;

    [Header("開始時のアニメーション")]
    [SerializeField] RectTransform RotateRT;
    [SerializeField] RectTransform StartPlayerRT;
    [SerializeField] TextMeshProUGUI StageTitleText;
    [SerializeField] TextMeshProUGUI NokoroText;
    [SerializeField] TextMeshProUGUI DeckCountText;

    [Header("DeckButton")]
    [SerializeField] public TextMeshProUGUI DeckCountSmallText;

    [Header("HeartHide")]
    [SerializeField] GameObject[] HeartHideObj;
    [SerializeField] GameObject[] hideFace;


    [Header("Menu")]
    [SerializeField] GameObject MenuObj;
    [SerializeField] TextMeshProUGUI titleTextBGM;
    [SerializeField] TextMeshProUGUI titleTextSE;
    [SerializeField] TextMeshProUGUI titleTextBattleBack;
    [SerializeField] TextMeshProUGUI titleTextTitle;

    [Header("タイトルバック")]
    [SerializeField] GameObject titleBackObj;
    [SerializeField] TextMeshProUGUI titleText1;
    [SerializeField] TextMeshProUGUI titleText2;
    [SerializeField] TextMeshProUGUI titleText3;
    [SerializeField] TextMeshProUGUI titleText4;

    [Header("itiran")]
    [SerializeField] GameObject ItiranObj;
    [SerializeField] TextMeshProUGUI[] itiranText;


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


    public void TypeText(TextMeshProUGUI renderText, string text, float typeSpeed = 0.05f)
    {
        if(SerifSankaku != null)
        {
            SerifSankaku.color = new Color(0, 0, 0, 0);
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeTextCoro(renderText,text,typeSpeed));
    }

    private IEnumerator TypeTextCoro(TextMeshProUGUI renderText, string text,float typeSpeed = 0.05f)
    {
        RenderText(renderText, "");
        Debug.Log(text);
        for (int i = 0; i < text.Length; i++)
        {
            renderText.text += text[i];
            yield return new WaitForSeconds(typeSpeed);
        }
        yield return new WaitForSeconds(0.1f);

        if(SerifSankaku != null)
        {
            SerifSankaku.color = new Color(1, 1, 1, 1);
        }
    }

    /// <summary>
    /// テキストの表現をする
    /// </summary>
    /// <param name="text"></param>
    public void RenderText(TextMeshProUGUI renderText,string text)
    {
        string result = text;
        //今後、表示周りを改良するとき用
        if (CardDataController.isCardUses[1])//言葉を消失している時
        {
            renderText.text = TextBug(text);
        }
        else
        {
            renderText.text = result;
        }
    }

    public string TextBug(string text)
    {
        char[] chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] != ' ')
            {
                //int offset = Random.Range(1, 5); // ±何文字ズラすか
                chars[i] = (char)(chars[i] + 10);
            }
        }
        return new string(chars);
    }

    /// <summary>
    /// Transitionを非同期にアニメーション
    /// </summary>
    public async UniTask AnimateTransitionAsync(float from, float to, float duration)
    {
        FadeTransitionObj.SetActive(true);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            FadeTransitionEffect.transitionRate = Mathf.Lerp(from, to, t);
            await UniTask.Yield(); // 次のフレームまで待機
        }

        //フェードアウトなら残る

        FadeTransitionEffect.transitionRate = to; // 最終値で確定させる
        if (to > 0)
        {
            FadeTransitionObj.SetActive(false);
        }
    }

    /// <summary>
    /// 最後の白色フラッシュトランジション
    /// </summary>
    public async UniTask AnimateTransitionAsyncFlash(float from, float to, float duration)
    {
        FadeTransitionFlashObj.SetActive(true);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            FadeTransitionFlashEffect.transitionRate = Mathf.Lerp(from, to, t);
            await UniTask.Yield(); // 次のフレームまで待機
        }

        //フェードアウトなら残る

        FadeTransitionFlashEffect.transitionRate = to; // 最終値で確定させる
        if (to > 0)
        {
            FadeTransitionFlashObj.SetActive(false);
        }
    }

    public async UniTask AnimateTransitionAsyncLove(float from, float to, float duration)
    {
        LoveTransitonObj.SetActive(true);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            LoveTransitonEffect.transitionRate = Mathf.Lerp(from, to, t);
            await UniTask.Yield(); // 次のフレームまで待機
        }

        //フェードアウトなら残る

        LoveTransitonEffect.transitionRate = to; // 最終値で確定させる
        if (to > 0)
        {
            //LoveTransitonObj.SetActive(false);
        }
    }

    public async UniTask AnimateTransitionAsyncEnding(float from, float to, float duration)
    {
        FadeTransitionEnding.SetActive(true);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            FadeTransitionEndingEffect.transitionRate = Mathf.Lerp(from, to, t);
            await UniTask.Yield(); // 次のフレームまで待機
        }

        //フェードアウトなら残る
        FadeTransitionEndingEffect.transitionRate = to; // 最終値で確定させる
    }


    public async UniTask AnimateTransitionAsyncLastItiran(float from, float to, float duration)
    {
        LastFadeObj.SetActive(true);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
           FadeTransLastEffect.transitionRate = Mathf.Lerp(from, to, t);
            await UniTask.Yield(); // 次のフレームまで待機
        }

        //フェードアウトなら残る
        FadeTransLastEffect.transitionRate = to; // 最終値で確定させる
    }

    /// <summary>
    /// 0枚エンド
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask AnimateTransitionAsyncLost(float from, float to, float duration)
    {
        LostFadeObj.SetActive(true);
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            LostFadeTransLastEffect.transitionRate = Mathf.Lerp(from, to, t);
            Vector2 size = Lostimg.sizeDelta;
            size.x = Mathf.Lerp(to,from, t) * 2000;
            Lostimg.sizeDelta = size;
            await UniTask.Yield(); // 次のフレームまで待機
        }

        //フェードアウトなら残る
        LostFadeTransLastEffect.transitionRate = to; // 最終値で確定させる
    }

    public void OpenSerif(string text,string tiseiText = "",float time = 0.05f)
    {
        Debug.Log("Serif");
        serifBorderObj.SetActive(true);

        string openText = text;
        if (CardDataController.isCardUses[23] && tiseiText != "") openText = tiseiText;

        if (CardDataController.isCardUses[1])//言葉を失ったら
        {
            string resultText = TextBug(openText);
            TypeText(serifText, resultText, time);
        }
        else
        {
            if(CardDataController.isCardUses[15] && openText[0] != '(' && openText[0] != '（')//声
            {
                string str = "ん…!(" + openText + ")";
                TypeText(serifText, str, time);
            }
            else
            {
                TypeText(serifText, openText, time);
            }
        }
    }

    public void CloseSerif()
    {
        serifBorderObj.SetActive(false);    
    }


    public void CameraShake()
    {
        Vector3 originalPos = CameraTransform.position;
        // カメラを揺らす
        CameraTransform.DOShakePosition(0.1f, 1f, 5, 0)
            .OnComplete(() => CameraTransform.localPosition = originalPos); // 終わったら元の位置に戻す
    }

    public void UIShake(RectTransform target,float duration,float move)
    {
        Vector3 originalPos = target.anchoredPosition;
        // カメラを揺らす
        target.DOShakePosition(duration, move, 5, 0)
            .OnComplete(() => target.anchoredPosition = originalPos); // 終わったら元の位置に戻す
    }

    /// <summary>
    /// ダメージを受けたときの、体力減少関数
    /// </summary>
    public void TakeDamegeView(TextMeshProUGUI text,int currentValue,int damage,float duration,bool isEnemy = false)
    {
        int newValue = Mathf.Max(0, currentValue - damage);
        if(!isEnemy)//プレイヤーなら
        {
            if (CardDataController.isCardUses[5])//名前   
            {
                if (newValue > 40)
                {
                    newValue = 40;
                }
            }
            else
            {
                if (newValue > 20)
                {
                    newValue = 20;
                }
            }
        }

        if (CardDataController.isCardUses[1])//言葉を失ったら
        {
            DOTween.To(() => currentValue, x => {
                currentValue = x;
                text.text = TextBug(currentValue.ToString());
            }, newValue, duration).SetEase(Ease.OutCubic);
        }
        else
        {
            DOTween.To(() => currentValue, x => {
                currentValue = x;
                text.text = currentValue.ToString();
            }, newValue, duration).SetEase(Ease.OutCubic);
        }
    }

    //入ってくるやつ
    public async UniTask StartCutIn()
    {
        RotateRT.eulerAngles = new Vector3(0, 0, 130);
        RotateRT.DORotate(Vector3.zero,1f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        StartPlayerRT.gameObject.SetActive(true);
        StartPlayerRT.DOAnchorPos(new Vector2(400,50),0.8f).SetEase(Ease.OutBack);
    }

    public async UniTask StartCutOut()
    {
        //初期設定
        StartPlayerRT.gameObject.SetActive(true);
        StartPlayerRT.anchoredPosition = new Vector2(400, 50);
        RotateRT.eulerAngles = new Vector3(0, 0, 0);

        if (CardDataController.isCardUses[1])
        {
            string resultText = TextBug($"{StageController.Instance.StageName[StageController.StageIndex]}");
            TypeText(StageTitleText, resultText, 0.1f);
        } 
        else
        {
            TypeText(StageTitleText, $"{StageController.Instance.StageName[StageController.StageIndex]}", 0.1f);
        }

        RenderText(NokoroText,"残りカード枚数　　　枚");
        RenderText(DeckCountText, "");
        FadeTransitionObj.SetActive(true);
        await UniTask.Delay(2000);

        SoundController.Instance.PlaySFX(7);
        if (CardDataController.isCardUses[1])
        {
            string resultText = TextBug($"{CardDataController.Instance.GetCardList(CardDataController.DeckState.DECK).Count}");
            TypeText(DeckCountText, resultText);
        }
        else
        {
            TypeText(DeckCountText, $"{CardDataController.Instance.GetCardList(CardDataController.DeckState.DECK).Count}");
        }


        await UniTask.Delay(1500);
        StartPlayerRT.DOAnchorPos(new Vector2(-400, 50), 0.8f).SetEase(Ease.InBack);
        await UniTask.Delay(1000);
        SoundController.Instance.PlaySFX(3);
        StartPlayerRT.gameObject.SetActive(false);
        RotateRT.DORotate(new Vector3(0,0,150), 1f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
    }

    //ハートを見えなくする
    public void HeartHide()
    {
        foreach(GameObject obj in HeartHideObj)
        {
            obj.SetActive(true);
        }
    }

    public void faceHide()
    {
        for(int i = 0; i < hideFace.Length; i++)
        {
            hideFace[i].SetActive(true);
        }
    }

    public void MenuPanels(bool i)
    {
        SoundController.Instance.PlaySFX(6);
        MenuObj.SetActive(i);
    }

    public void ItiranPanels(bool i)
    {
        if (GameController.Instance.nowState != GameController.GameState.SELECT && !ItiranObj.activeSelf　&& GameController.Instance.nowState != GameController.GameState.ENDING) return;
        SoundController.Instance.PlaySFX(6);
        ItiranObj.SetActive(i);

        if(i)
        {
            GameController.Instance.nowState = GameController.GameState.ITIRAN;
            CardViewController.Instance.DeckCardView(CardDataController.DeckState.DECK);
        }
        else
        {
            if(GameController.Instance.isFinish)
            {
                GameController.Instance.nowState = GameState.ENDING;
            }
            else
            {
                GameController.Instance.nowState = GameController.GameState.SELECT;
            }
        }
    }

    public void MozibakeruUI()
    {
        RenderText(titleTextBGM, "BGM");
        RenderText(titleTextSE, "SE");
        RenderText(titleTextBattleBack, "戦闘に戻る");
        RenderText(titleTextTitle, "タイトルへ");

        RenderText(titleText1, "タイトルに\n戻りますか？");
        RenderText(titleText4, "※現在の状態は失われます");
        RenderText(titleText2, "はい");
        RenderText(titleText3,"いいえ");

        RenderText(itiranText[0], "閉じる");
        RenderText(itiranText[1], "山札");
        RenderText(itiranText[2], "手札");
        RenderText(itiranText[3], "捨て札");
        RenderText(itiranText[4], "使用済");
    }

    public void TitileBackOpen(bool i)
    {
        titleBackObj.SetActive(i);
    }

    public void SetObj(GameObject obj,bool i)
    {
        obj.SetActive(i);
    }

    /// <summary>
    /// 揺れる
    /// </summary>
    public void StartBreathing(RectTransform rt,float moveAmount,float duration)
    {
        Vector2 originalPos = rt.anchoredPosition;
        rt.DOAnchorPosY(originalPos.y + moveAmount, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void SetSankakuReathing()
    {
        StartBreathing(SerifSankaku.gameObject.GetComponent<RectTransform>(),10,1);
    }


    public void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color currentColor = text.color;
        currentColor.a = Mathf.Clamp01(alpha);
        text.color = currentColor;
    }

    //一覧表示の設定
    public async void LastResultTransiton()
    {
        LastItiranObj.SetActive(true);
        SetLastButton(0);
        await AnimateTransitionAsyncLastItiran(0, 1, 2);
    }

    public void SetLastButton(int i)
    {
        if (i == 0)//残したもの表示
        {
            SetAlpha(lostUIText, 0.5f);
            SetAlpha(NokoriUIText, 1f);
            CardViewController.Instance.DeckCardView(CardDataController.DeckState.DECK,1);
        }
        else if(i == 1)//失ったもの表示
        {
            SetAlpha(lostUIText, 1f);
            SetAlpha(NokoriUIText, 0.5f);
            CardViewController.Instance.DeckCardView(CardDataController.DeckState.EXCLUSION, 1,false);
        }
        else//タイトルへ戻る
        {
            SceneController.Instance.LoadSceneIndex(0);
        }
    }
}
