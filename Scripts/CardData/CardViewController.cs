using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using static CardDataController;
using System.Linq;

//カードの描画周りを管理する関数
public class CardViewController : MonoBehaviour
{
    public static CardViewController Instance { get; private set; }

    [Header("スクリプト")]
    [SerializeField] UIController uiController;

    [Header("カードのプレハブ等")]
    //カードの描画される基盤
    [SerializeField] GameObject cardViewPrefab;
    //カードを描画する時の親オブジェクト
    [SerializeField] RectTransform cardViewParent;

    [Header("手札カードの処理配置の設定")]
    [SerializeField] Vector2 handcard_startpos;
    [SerializeField] float hand_offset = 270;
    [SerializeField] RectTransform drawcard_startpos;
    [SerializeField] RectTransform discordcard_endpos;

    public List<CardViewParts> instanceCards;

    [Header("icon画像データ")]
    [SerializeField] Sprite[] iconSprites;
    [Header("愛専用スプライト")]
    [SerializeField] Sprite[] aiSprite;

    [Header("一覧表示")]
    [SerializeField] RectTransform itiranParent;
    [SerializeField] RectTransform LastItiranParent;

    public class CardViewParts
    {
        public RectTransform body;
        public Image body_img;
        public Image icon;
        public Image back_img;
        public TextMeshProUGUI title;
        public TextMeshProUGUI text;
        public Vector2 start_pos;

        public CardViewParts(Transform root)
        {
            // 子オブジェクトの名前が "Body", "BodyImage", "Icon", "Text" だと仮定
            body = root.GetComponent<RectTransform>();
            body_img = root.Find("CardViewImage").GetComponent<Image>();
            icon = root.Find("Icon")?.GetComponent<Image>();
            back_img = root.Find("backImage").GetComponent <Image>();
            title = root.Find("titleText")?.GetComponent<TextMeshProUGUI>();
            text = root.Find("effectText")?.GetComponent<TextMeshProUGUI>();
            start_pos = body.anchoredPosition;
        }
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (instanceCards == null)
        {
            instanceCards = new List<CardViewParts>();
        }
        instanceCards.Clear();
    }

    /// <summary>
    /// カードのドロー演出
    /// </summary>
    public async UniTask DrawCardView()
    {
        Debug.Log("カードをドロー");
        //カードを引く
        CardDataController.Instance.DrawCard();
        CardDataController.Instance.DrawCard();
        CardDataController.Instance.DrawCard();
        CardDataController.Instance.DrawCard();
        HandCardView();


        for(int i = 0;i < instanceCards.Count; i++)
        {
            instanceCards[i].body.anchoredPosition = drawcard_startpos.anchoredPosition;
        }

        int delays = 300;

        for (int i = 0; i < instanceCards.Count; i++)
        {

            _ = CreateMoveAndSpinSequence(instanceCards[i].body, drawcard_startpos.anchoredPosition, SetCardPos(i), 0.5f, 2160);
            await UniTask.Delay(delays);
            SoundController.Instance.PlaySFX(1);
            UIController.Instance.RenderText(UIController.Instance.DeckCountSmallText, (CardDataController.Instance.GetCardList(DeckState.DECK).Count + (4 - i)).ToString());
        }
        await UniTask.Delay(200);
    }

    /// <summary>
    /// 捨て札にカードを送る演出
    /// </summary>
    /// <returns></returns>
    public async UniTask DiscordCardView()
    {
        HandCardView();
        int delays = 300;

        for (int i = 0; i < instanceCards.Count; i++)
        {
            SoundController.Instance.PlaySFX(1);
            _ = CreateMoveAndSpinSequence(instanceCards[i].body, new Vector2(handcard_startpos.x + hand_offset * i, handcard_startpos.y), discordcard_endpos.anchoredPosition, 0.5f, 720);
            await UniTask.Delay(delays);
        }
    }

    /// <summary>
    /// 回転＋移動するやつ
    /// </summary>
    /// <param name="target"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="duration"></param>
    /// <param name="spinSpeed"></param>
    /// <returns></returns>
    private async UniTask<Sequence> CreateMoveAndSpinSequence(RectTransform target, Vector3 startPos, Vector3 endPos, float duration, float spinSpeed)
    {
        target.anchoredPosition = startPos;
        var seq = DOTween.Sequence().SetLink(target.gameObject); // Destroyされたら自動停止;
        seq.Join(target.DOAnchorPos(endPos, duration).SetEase(Ease.InOutQuad));
        seq.Join(target.DORotate(new Vector3(0, 0, spinSpeed), duration, RotateMode.FastBeyond360));

        return seq;
    }

    /// <summary>
    /// 手札カードの生成
    /// </summary>
    private CardViewParts CreateHandCardView(CardData.Param card_data,RectTransform parent)
    {

        GameObject obj = Instantiate(cardViewPrefab, parent);
        obj.GetComponent<CardInteraction>().cardData = card_data;
        CardViewParts part = new CardViewParts(obj.transform);
        instanceCards.Add(part);
        //内容の入力
        if (card_data != null)
        {
            if (CardDataController.isCardUses[23])//知性切り
            {
                uiController.RenderText(part.title, card_data.Name2.ToString());
                EffectView(card_data, part,true);
            }
            else
            {
                uiController.RenderText(part.title, card_data.Name.ToString());
                EffectView(card_data, part,false);
            }


            part.icon.sprite = iconSprites[card_data.CostID];
            if(card_data.CostID == 0)//愛なら
            {
                part.body_img.sprite = aiSprite[0];
                part.back_img.sprite = aiSprite[1];
            }
            if(card_data.CostID == 34)
            {
                part.body_img.sprite = aiSprite[2];
                part.back_img.sprite = aiSprite[3];
            }
        }
        return part;
    }

    public void EffectView(CardData.Param card_data, CardViewParts part, bool eff = false)
    {
        Debug.Log("EffectView");
        string str = card_data.effect;
        if (eff) str = card_data.effect2;

        if (GameController.Instance.nowState == GameController.GameState.ENDING)
        {
            if(card_data.CostID == 0)
            {
                if (CardDataController.isCardUses[23])
                    str = "ゆめからさめる";
                else
                    str = "夢から覚める";
            }
        }

        //。が含まれているか否か
        if (GameController.TrySplitByPeriod(str, out string before, out string after))
        {
            Debug.Log($"before:{before} + after:{after}");
            if (GameController.TryParsePlaceholder(before, out string key1, out int value1, out string before2, out string after2))//カード効果に通知が含まれているなら
            {
                if (GameController.TryParsePlaceholder(after, out string key2, out int value2, out string before3, out string after4))
                {
                    uiController.RenderText(part.text, before2 + GameController.Instance.GetBattleDatas(key1,value1) + after2 +"。"+ before3 + GameController.Instance.GetBattleDatas(key2, value2) + after4);
                }
                else//戦闘のみ
                {
                    uiController.RenderText(part.text, before2 + GameController.Instance.GetBattleDatas(key1, value1) + after2);
                }
            }
            else
            {
                uiController.RenderText(part.text, str.ToString());
            }
        }
        else//含まれていない
        {
            if (GameController.TryParsePlaceholder(str, out string key, out int value, out string before2, out string after2))//カード効果に通知が含まれているなら
            {
                uiController.RenderText(part.text, before2 + GameController.Instance.GetBattleDatas(key, value) + after2.ToString());
            }
            else
            {
                uiController.RenderText(part.text, str.ToString());
            }
        }
    }

    //手札の全破棄
    public void DeleteAllhandCard()
    {
        foreach (Transform child in cardViewParent)
        {
            Destroy(child.gameObject);
        }
        instanceCards.Clear();
    }

    //手札のカード表示
    public void HandCardView()
    {
        DeleteAllhandCard();

        int index = 0;
        //手札の内容の取得
        foreach(CardData.Param card_data in CardDataController.Instance.GetCardList(CardDataController.DeckState.HAND))
        {
            CardViewParts part = CreateHandCardView(card_data,cardViewParent);
            Vector2 vec = SetCardPos(index);
            part.body.anchoredPosition = vec;

            index++;
        }
    }

    bool issss = false;
    //カードを生成する(運勢用)
    async public UniTask CreateLakky()
    {
        int res = UnityEngine.Random.Range(0, CardDataController.Instance.GetCardList(CardDataController.DeckState.ALL).Count);

        CardViewParts part = CardViewController.Instance.CreateHandCardView(CardDataController.Instance.GetCardList(CardDataController.DeckState.ALL)[res],cardViewParent);
        Vector2 vec = Vector2.zero;
        part.body.anchoredPosition = vec;
        await UniTask.Delay(1000);
        await part.body.gameObject.GetComponent<CardInteraction>().CardAnimation();

        CardData.Param cardData = part.body.gameObject.GetComponent<CardInteraction>().cardData;

        //効果発動
        await GameController.Instance.CardEffect(cardData);

        //代償支払い
        await GameController.Instance.CardCost(cardData,true);
    }

    private Vector2 SetCardPos(int index = 0)
    {
        return new Vector2(handcard_startpos.x + index * hand_offset, handcard_startpos.y);
    }

    //一覧表示のカード表示
    public void DeckCardView(CardDataController.DeckState state,int i = 0,bool isShuffle = true)
    {
        RectTransform parents = itiranParent;
        if (i == 1) parents = LastItiranParent;

        foreach (Transform child in parents)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        Debug.Log($"state:{state}");
        List<CardData.Param> card_views = new List<CardData.Param>();
        foreach (CardData.Param card_data in CardDataController.Instance.GetCardList(state))
        {
            card_views.Add(card_data);
        }
        //シャッフル処理
        if(isShuffle)card_views = card_views.OrderBy(card => card.CostID).ToList();

        //手札の内容の取得
        foreach (CardData.Param card_data in card_views)
        {
            CardViewParts part = CreateHandCardView(card_data, parents);
            Vector2 vec = Vector2.zero;
            part.body.anchoredPosition = vec;
            index++;
        }
    }

    public void OpenItiran(int i)
    {
        DeckCardView((CardDataController.DeckState)i);
    }

    /// <summary>
    /// 愛のカードだけをドローする
    /// </summary>
    /// <returns></returns>
    public async UniTask DrawCardLOVE()
    {
        Debug.Log("カードをドロー");
        //カードを引く
        CardDataController.Instance.MoveCard(CardDataController.Instance.LOVECardData, CardDataController.DeckState.DECK,CardDataController.DeckState.HAND);
        HandCardView();


        for (int i = 0; i < instanceCards.Count; i++)
        {
            instanceCards[i].body.anchoredPosition = drawcard_startpos.anchoredPosition;
        }

        int delays = 300;

        for (int i = 0; i < instanceCards.Count; i++)
        {
            SoundController.Instance.PlaySFX(1);
            _ = CreateMoveAndSpinSequence(instanceCards[i].body, drawcard_startpos.anchoredPosition, SetCardPos(i), 0.5f, 2160);
            await UniTask.Delay(delays);
        }
        await UniTask.Delay(200);
    }

    //デッキの上に新規カード追加
    async public UniTask AddEXCard()
    {
        CardDataController.Instance.GetCardList(DeckState.DECK).Insert(0, CardDataController.Instance.ZIKENCardData);
        CardViewParts part = CreateHandCardView(CardDataController.Instance.ZIKENCardData, cardViewParent);
        Vector2 vec = new Vector2(0, 0);
        part.body.anchoredPosition = vec;

        await UniTask.Delay(1500);
        _ = CreateMoveAndSpinSequence(part.body, vec, drawcard_startpos.anchoredPosition, 1f, 2160);
        await UniTask.Delay(2000);
    }
}
