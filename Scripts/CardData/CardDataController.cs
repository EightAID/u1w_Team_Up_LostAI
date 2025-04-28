using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardDataController : MonoBehaviour
{
    public static CardDataController Instance { get; private set; }

    [Header("カードデータ")]
    [SerializeField] CardData cardData;
    /// <summary>
    /// カードを使用済みか否か
    /// </summary>
    public static bool[] isCardUses = new bool[100];

    [Header("デッキまわしするやつ")]
    /// <summary>手札</summary>
    [SerializeField] List<CardData.Param> handlist;
    /// <summary>山札</summary>
    [SerializeField] List<CardData.Param> decklist;
    /// <summary>捨て札</summary>
    [SerializeField] List<CardData.Param> discordlist;
    /// <summary>除外札</summary>
    [SerializeField] List<CardData.Param> exclusionlist;

    [SerializeField] List<CardData.Param> ALLCardlist;


    public CardData.Param LOVECardData;
    public CardData.Param ZIKENCardData;

    public static  bool[] SaveIsCardUses = new bool[100];
    public static int SaveLife = 0;

    public enum DeckState
    {
        HAND,
        DECK,
        DISCORD,
        EXCLUSION,
        ALL,
    }

    private void Awake()
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
    /// デッキ情報の取得
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public List<CardData.Param> GetCardList(DeckState state)
    {
        switch (state)
        {
            case DeckState.HAND: return handlist;
            case DeckState.DECK: return decklist;
            case DeckState.DISCORD: return discordlist;
            case DeckState.EXCLUSION: return exclusionlist;
            case DeckState.ALL: return ALLCardlist;
        }
        //どれでもなかったら
        return handlist;
    }

    /// <summary>
    /// カードを追加する関数
    /// </summary>
    /// <param name="carddata">カード情報</param>
    /// <param name="state"></param>
    public void AddCard(CardData.Param carddata,DeckState state)
    {
        switch (state)
        {
            case DeckState.HAND:
                handlist.Add(carddata);
                break;
            case DeckState.DECK:
                decklist.Add(carddata);
                break;
            case DeckState.DISCORD:
                discordlist.Add(carddata);
                break;
            case DeckState.EXCLUSION:
                exclusionlist.Add(carddata);
                break;
        }
    }

    /// <summary>
    /// カードを削除する関数
    /// </summary>
    /// <param name="carddata"></param>
    /// <param name="state"></param>
    public void RemoveCard(CardData.Param carddata, DeckState state)
    {
        switch (state)
        {
            case DeckState.HAND:
                handlist.Remove(carddata);
                break;
            case DeckState.DECK:
                decklist.Remove(carddata);
                break;
            case DeckState.DISCORD:
                discordlist.Remove(carddata);
                break;
            case DeckState.EXCLUSION:
                exclusionlist.Remove(carddata);
                break;
        }
    }

    public void MoveCard(CardData.Param carddata, DeckState startstate,DeckState endstate)
    {
        //元の方から削除する
        RemoveCard(carddata, startstate);
        //移動先に追加する
        AddCard(carddata, endstate);
    }

    public void Shuffle(DeckState state)
    {
        List<CardData.Param> list = handlist;
        switch (state)
        {
            case DeckState.HAND:
                list = handlist;
                break;
            case DeckState.DECK:
                list = decklist;
                break;
            case DeckState.DISCORD:
                list = discordlist;
                break;
            case DeckState.EXCLUSION:
                list = exclusionlist;
                break;
        }
        //シャッフル処理
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
        switch (state)
        {
            case DeckState.HAND:
                handlist = list;
                break;
            case DeckState.DECK:
                decklist = list;
                break;
            case DeckState.DISCORD:
                discordlist = list;
                break;
            case DeckState.EXCLUSION:
                exclusionlist = list;
                break;
        }
    }

    public void DrawCard()
    {
        //デッキに何もなかったら
        if(decklist.Count <= 0)
        {
            //捨て札からデッキにカードを戻す
            foreach (CardData.Param cardParam in discordlist)
            {
                AddCard(cardParam,DeckState.DECK);
            }
            Shuffle(DeckState.DECK);
            discordlist.Clear();
        }
        if (decklist.Count <= 0)
        {
            Debug.Log("完全にデッキが0枚です");
            return;
        }

        CardData.Param card = decklist[0];
        MoveCard(card, DeckState.DECK, DeckState.HAND);
    }

    int cardIndex = 0;
    /// <summary>
    /// 初期設定
    /// </summary>
    public void ResetCardata()
    {
        foreach (CardData.Sheet cardSheet in cardData.sheets)
        {
            foreach (CardData.Param cardParam in cardSheet.list)
            {

                //カードを全てデッキに追加
                if(cardParam.CostID <= 33) AddCard(cardParam, DeckState.DECK);
                if (cardIndex == 0) LOVECardData = cardParam;//愛のカード
                if(cardParam.CostID == 34) ZIKENCardData = cardParam;//トラウマカード
                if(cardParam.CostID == 35) AddCard(cardParam, DeckState.DECK);
                cardIndex++;

                if(cardParam.CostID != 34) ALLCardlist.Add(cardParam);
            }
        }
        cardIndex = 0;
        Shuffle(DeckState.DECK);
    }

    /// <summary>
    /// 手札を捨て札に全移動
    /// </summary>
    public void DiscordCard()
    {
        List<CardData.Param> data = GetCardList(DeckState.HAND);

        foreach(CardData.Param cardParam in data)
        {
            AddCard(cardParam, DeckState.DISCORD);
        }
        handlist.Clear();
    }

    /// <summary>
    /// 手札、捨て札を戻す
    /// </summary>
    public void ResetDeck()
    {
        List<CardData.Param> data = GetCardList(DeckState.HAND);

        foreach (CardData.Param cardParam in data)
        {
            AddCard(cardParam, DeckState.DECK);
        }
        handlist.Clear();

        List<CardData.Param> datas = GetCardList(DeckState.DISCORD);

        foreach (CardData.Param cardParam in datas)
        {
            AddCard(cardParam, DeckState.DECK);
        }
        discordlist.Clear();
        CardViewController.Instance.DeleteAllhandCard();
    }

    public CardData.Param GetCardData(int id)
    {
        foreach (CardData.Sheet cardSheet in cardData.sheets)
        {
            foreach (CardData.Param cardParam in cardSheet.list)
            {

               if(cardParam.CostID == id)
                    return cardParam;
            }
        }
        return null;
    }
}
