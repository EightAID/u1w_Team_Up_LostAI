using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardInteraction : MonoBehaviour ,IDragHandler,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    [Header("でぃそるぶ")]
    [SerializeField] private Material dissolveMaterial;

    [Header("カードサイズの設定")]
    [SerializeField] float startsize = 0.15f;
    [SerializeField] float entersize = 0.20f;

    [Header("このカードのrt")]
    [SerializeField] RectTransform rectTransform;
    //カードの背面
    [SerializeField] GameObject backImage_obj;
    [SerializeField] RectTransform HideObj;

    [Header("カードを使用するための座標offset")]
    [SerializeField] float useCardPosY = 0;

    [Header("使用時UI用リスト")]
    [SerializeField] List<RectTransform> frontui;
    [SerializeField] UIEffect FadeTransitionEffect;

    //このカードのデータ
    public CardData.Param cardData;

    [SerializeField] Image[] image;

    //カードの先行入力
    bool isInputCard = false;

    bool isKeyDown = false;
    float clicktTime = 0;

    bool isUsed = false;

    private void Start()
    {
        rectTransform.localScale = new Vector2(startsize, startsize);

        //既にグレーなら
        if (CardDataController.isCardUses[14])
        {
            foreach (var item in image)
            {
                Glayscale.Instance.SetGlay(item);
            }
            if (FadeTransitionEffect != null) FadeTransitionEffect.toneIntensity = 1.0f;
        }

        //視覚を失っているのなら
        if(GameController.Instance.nowState != GameController.GameState.LASTITIRAN)
        {
            if (CardDataController.isCardUses[2])
            {
                HideObj.gameObject.SetActive(true);
                HideObj.localScale = new Vector2(1.3f, 1.3f);
            }
        }
    }

    void LateUpdate()
    {
        //何も捜査していないときに、カードの上にポインター置いてた人向け
        if(GameController.Instance.nowState == GameController.GameState.SELECT && isInputCard)
        {
            isInputCard = false;
            EnterAction();
        }

        //回転をリセットする
        if (rectTransform.rotation.eulerAngles.y >= 360 || rectTransform.rotation.eulerAngles.y < -360)
        {
            Debug.Log("回転のリセット");
            //度数法を採用
            rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        //回転的に裏の時に
        if ((rectTransform.rotation.eulerAngles.y > 90 && rectTransform.rotation.eulerAngles.y < 270) || (rectTransform.rotation.eulerAngles.y < -90 && rectTransform.rotation.eulerAngles.y > -270))
        {
            backImage_obj.SetActive(true);
            foreach(RectTransform rt in frontui)
            {
                rt.localScale = Vector3.zero;
            }
        }
        else
        {
            backImage_obj.SetActive(false);
            foreach (RectTransform rt in frontui)
            {
                rt.localScale = Vector3.one;
            }
            HideObj.localScale = new Vector2(1.3f, 1.3f);
        }

        clicktTime += Time.deltaTime;
    }

    // ポインターが押されたとき
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
        isKeyDown = true;
        clicktTime = 0;
    }

    // ポインターが離されたとき
    public async void OnPointerUp(PointerEventData eventData)
    {
        isKeyDown = false;
        if (GameController.Instance.nowState != GameController.GameState.SELECT && GameController.Instance.nowState != GameController.GameState.ENDING) return;
        Debug.Log("Pointer Up");
        //カードが使用する範囲以上なら
        if(rectTransform.anchoredPosition.y >= useCardPosY)
        {
            if (GameController.Instance.nowState != GameController.GameState.ENDING) GameController.Instance.nowState = GameController.GameState.USE;

            await UseCard();
        }
        else
        {
            //入力が短かったら使用判定
            if (clicktTime < 0.1f)
            {
                if (GameController.Instance.nowState != GameController.GameState.ENDING) GameController.Instance.nowState = GameController.GameState.USE;
                transform.SetAsLastSibling();
                UIController.Instance.CloseSerif();
                await UseCard();
            }
            else
            {
                CardViewController.Instance.HandCardView();
            }
        }
    }

    public async UniTask CardAnimation()
    {
        //このオブジェクトを、0,150の座標に移動させる

        Vector2 targetPosition = new Vector2(0, 200);
        Vector3 originalScale = rectTransform.localScale;
        Vector3 emphasizedScale = originalScale * 1.1f;

        float moveDuration = 0.2f;

        // Sequenceでアニメーションをまとめる
        Sequence sequence = DOTween.Sequence();

        sequence.Join(rectTransform.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.InOutQuad));
        sequence.Join(rectTransform.DOScale(emphasizedScale, moveDuration / 2).SetLoops(2, LoopType.Yoyo));
        sequence.Join(rectTransform.DORotate(new Vector3(0, 180, 0), 0.8f, RotateMode.WorldAxisAdd));

        SoundController.Instance.PlaySFX(3);
        await sequence.AsyncWaitForCompletion();
        await UniTask.Delay(400);

        //カードを削除する
        SoundController.Instance.PlaySFX(4);

        await AnimateTransitionAsync(0, 1, 1.3f);
    }

    public async UniTask UseCard()
    {
        await CardAnimation();

        if (isUsed) return;
        //最終のみ
        if (GameController.Instance.nowState == GameController.GameState.ENDING)
        {
            isUsed = true;
            PrologController.isClear = true;
            Debug.Log("クリアした");
            CardDataController.Instance.MoveCard(cardData,CardDataController.DeckState.HAND,CardDataController.DeckState.DECK);
            await UIController.Instance.AnimateTransitionAsyncFlash(1, 0, 3f);
            await StageController.Instance.LastSerif2();
            return;
        }

        await UniTask.Delay(300);
        await GameController.Instance.UseCard(cardData);
    }

    // ドラッグ中
    public void OnDrag(PointerEventData eventData)
    {
        if (GameController.Instance.nowState != GameController.GameState.SELECT && GameController.Instance.nowState != GameController.GameState.ENDING) return;
        Vector2 mousePos = Input.mousePosition;
        rectTransform.position = mousePos;
        transform.SetAsLastSibling();
        UIController.Instance.CloseSerif();
    }

    // カーソルが入ったとき
    //詳細情報を表示したりしたいかもなぁ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.nowState != GameController.GameState.SELECT && GameController.Instance.nowState != GameController.GameState.ENDING)
        {
            isInputCard = true;
            return;
        }
        EnterAction();
    }

    private void EnterAction()
    {
        SoundController.Instance.PlaySFX(0);

        //全ての感情が消えたら
        if(CardDataController.isCardUses[26] && CardDataController.isCardUses[27] && CardDataController.isCardUses[28] && CardDataController.isCardUses[29] && CardDataController.isCardUses[30])
        {
            UIController.Instance.OpenSerif("もう、どうでもいいな…");
            return;
        }

        //視覚失ってたら
        if (CardDataController.isCardUses[2])
        {
            if (CardDataController.isCardUses[23])
                UIController.Instance.OpenSerif("なにもみえないよ…");
            else
                UIController.Instance.OpenSerif("うう…なにも見えない…");
            return;
        }

        //恐怖心のとき
        if (CardDataController.isCardUses[22])
        {
            if (CardDataController.isCardUses[2])//視覚
                UIController.Instance.OpenSerif("わたし、まっくらのへいき！");
            else
                UIController.Instance.OpenSerif("わたし、暗い場所平気だな");
            return;
        }

        if (GameController.Instance.nowState == GameController.GameState.ENDING)
        {
            if (CardDataController.isCardUses[23])
                UIController.Instance.OpenSerif("(わたしにとって、これはだいじなものだから…！)");
            else
                UIController.Instance.OpenSerif("(わたしにとって、これは大事なものだから…！)");
            return;
        }
        else
        {

            string text = cardData.flavar;
            //特殊実装

            if (CardDataController.isCardUses[18])//両親切ったら
            {
                if (CardDataController.isCardUses[23])
                {
                    switch (cardData.CostID)
                    {
                        case 5:
                            text = "なまえかぁ…いったいだれが付けたんだろう";
                            break;
                        case 8:
                            text = "はんばーぐ、だいすきなんだ…";
                            break;
                        case 10://性欲
                            text = "ちゅーしたいきもちってこと？";
                            break;
                        case 17:
                            text = "たんじょうび、わたしはあんまりとくべつっておもったことない…";
                            break;
                        case 20:
                            text = "よるはこわくていけない…";
                            break;
                    }
                }
                else
                {
                    switch (cardData.CostID)
                    {
                        case 5:
                            text = "(名前…由来なんだっけな)";
                            break;
                        case 8:
                            text = "ハンバーグ食べたい…";
                            break;
                        case 10://性欲
                            text = "キスしたい気持ちってこと？";
                            break;
                        case 17:
                            text = "誕生日かぁ…わたし、祝ってもらった記憶がない…";
                            break;
                        case 20:
                            text = "いつも夜は怖くて行けなかったなぁ…";
                            break;
                    }
                }

            }

            if (CardDataController.isCardUses[23]) text = cardData.flavar2;

            UIController.Instance.OpenSerif(text);
        }

        rectTransform.localScale = new Vector2(entersize, entersize);
    }

    // カーソルが出たとき
    public void OnPointerExit(PointerEventData eventData)
    {
        isInputCard = false;
        if (GameController.Instance.nowState != GameController.GameState.SELECT && GameController.Instance.nowState != GameController.GameState.ENDING) return;
        UIController.Instance.CloseSerif();
        rectTransform.localScale = new Vector2(startsize, startsize);
    }

    /// <summary>
    /// Transitionを非同期にアニメーション
    /// </summary>
    public async UniTask AnimateTransitionAsync(float from, float to, float duration)
    {
        if (FadeTransitionEffect == null) return;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            FadeTransitionEffect.transitionRate = Mathf.Lerp(from, to, t);
            await UniTask.Yield(); // 次のフレームまで待機
        }
        FadeTransitionEffect.transitionRate = to; // 最終値で確定させる
    }
}
