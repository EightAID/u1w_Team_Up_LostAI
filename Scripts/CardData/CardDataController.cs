using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardDataController : MonoBehaviour
{
    public static CardDataController Instance { get; private set; }

    [Header("�J�[�h�f�[�^")]
    [SerializeField] CardData cardData;
    /// <summary>
    /// �J�[�h���g�p�ς݂��ۂ�
    /// </summary>
    public static bool[] isCardUses = new bool[100];

    [Header("�f�b�L�܂킵������")]
    /// <summary>��D</summary>
    [SerializeField] List<CardData.Param> handlist;
    /// <summary>�R�D</summary>
    [SerializeField] List<CardData.Param> decklist;
    /// <summary>�̂ĎD</summary>
    [SerializeField] List<CardData.Param> discordlist;
    /// <summary>���O�D</summary>
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
    /// �f�b�L���̎擾
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
        //�ǂ�ł��Ȃ�������
        return handlist;
    }

    /// <summary>
    /// �J�[�h��ǉ�����֐�
    /// </summary>
    /// <param name="carddata">�J�[�h���</param>
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
    /// �J�[�h���폜����֐�
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
        //���̕�����폜����
        RemoveCard(carddata, startstate);
        //�ړ���ɒǉ�����
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
        //�V���b�t������
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
        //�f�b�L�ɉ����Ȃ�������
        if(decklist.Count <= 0)
        {
            //�̂ĎD����f�b�L�ɃJ�[�h��߂�
            foreach (CardData.Param cardParam in discordlist)
            {
                AddCard(cardParam,DeckState.DECK);
            }
            Shuffle(DeckState.DECK);
            discordlist.Clear();
        }
        if (decklist.Count <= 0)
        {
            Debug.Log("���S�Ƀf�b�L��0���ł�");
            return;
        }

        CardData.Param card = decklist[0];
        MoveCard(card, DeckState.DECK, DeckState.HAND);
    }

    int cardIndex = 0;
    /// <summary>
    /// �����ݒ�
    /// </summary>
    public void ResetCardata()
    {
        foreach (CardData.Sheet cardSheet in cardData.sheets)
        {
            foreach (CardData.Param cardParam in cardSheet.list)
            {

                //�J�[�h��S�ăf�b�L�ɒǉ�
                if(cardParam.CostID <= 33) AddCard(cardParam, DeckState.DECK);
                if (cardIndex == 0) LOVECardData = cardParam;//���̃J�[�h
                if(cardParam.CostID == 34) ZIKENCardData = cardParam;//�g���E�}�J�[�h
                if(cardParam.CostID == 35) AddCard(cardParam, DeckState.DECK);
                cardIndex++;

                if(cardParam.CostID != 34) ALLCardlist.Add(cardParam);
            }
        }
        cardIndex = 0;
        Shuffle(DeckState.DECK);
    }

    /// <summary>
    /// ��D���̂ĎD�ɑS�ړ�
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
    /// ��D�A�̂ĎD��߂�
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
