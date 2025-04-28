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

//�J�[�h�̕`�������Ǘ�����֐�
public class CardViewController : MonoBehaviour
{
    public static CardViewController Instance { get; private set; }

    [Header("�X�N���v�g")]
    [SerializeField] UIController uiController;

    [Header("�J�[�h�̃v���n�u��")]
    //�J�[�h�̕`�悳�����
    [SerializeField] GameObject cardViewPrefab;
    //�J�[�h��`�悷�鎞�̐e�I�u�W�F�N�g
    [SerializeField] RectTransform cardViewParent;

    [Header("��D�J�[�h�̏����z�u�̐ݒ�")]
    [SerializeField] Vector2 handcard_startpos;
    [SerializeField] float hand_offset = 270;
    [SerializeField] RectTransform drawcard_startpos;
    [SerializeField] RectTransform discordcard_endpos;

    public List<CardViewParts> instanceCards;

    [Header("icon�摜�f�[�^")]
    [SerializeField] Sprite[] iconSprites;
    [Header("����p�X�v���C�g")]
    [SerializeField] Sprite[] aiSprite;

    [Header("�ꗗ�\��")]
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
            // �q�I�u�W�F�N�g�̖��O�� "Body", "BodyImage", "Icon", "Text" ���Ɖ���
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
    /// �J�[�h�̃h���[���o
    /// </summary>
    public async UniTask DrawCardView()
    {
        Debug.Log("�J�[�h���h���[");
        //�J�[�h������
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
    /// �̂ĎD�ɃJ�[�h�𑗂鉉�o
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
    /// ��]�{�ړ�������
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
        var seq = DOTween.Sequence().SetLink(target.gameObject); // Destroy���ꂽ�玩����~;
        seq.Join(target.DOAnchorPos(endPos, duration).SetEase(Ease.InOutQuad));
        seq.Join(target.DORotate(new Vector3(0, 0, spinSpeed), duration, RotateMode.FastBeyond360));

        return seq;
    }

    /// <summary>
    /// ��D�J�[�h�̐���
    /// </summary>
    private CardViewParts CreateHandCardView(CardData.Param card_data,RectTransform parent)
    {

        GameObject obj = Instantiate(cardViewPrefab, parent);
        obj.GetComponent<CardInteraction>().cardData = card_data;
        CardViewParts part = new CardViewParts(obj.transform);
        instanceCards.Add(part);
        //���e�̓���
        if (card_data != null)
        {
            if (CardDataController.isCardUses[23])//�m���؂�
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
            if(card_data.CostID == 0)//���Ȃ�
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
                    str = "��߂��炳�߂�";
                else
                    str = "������o�߂�";
            }
        }

        //�B���܂܂�Ă��邩�ۂ�
        if (GameController.TrySplitByPeriod(str, out string before, out string after))
        {
            Debug.Log($"before:{before} + after:{after}");
            if (GameController.TryParsePlaceholder(before, out string key1, out int value1, out string before2, out string after2))//�J�[�h���ʂɒʒm���܂܂�Ă���Ȃ�
            {
                if (GameController.TryParsePlaceholder(after, out string key2, out int value2, out string before3, out string after4))
                {
                    uiController.RenderText(part.text, before2 + GameController.Instance.GetBattleDatas(key1,value1) + after2 +"�B"+ before3 + GameController.Instance.GetBattleDatas(key2, value2) + after4);
                }
                else//�퓬�̂�
                {
                    uiController.RenderText(part.text, before2 + GameController.Instance.GetBattleDatas(key1, value1) + after2);
                }
            }
            else
            {
                uiController.RenderText(part.text, str.ToString());
            }
        }
        else//�܂܂�Ă��Ȃ�
        {
            if (GameController.TryParsePlaceholder(str, out string key, out int value, out string before2, out string after2))//�J�[�h���ʂɒʒm���܂܂�Ă���Ȃ�
            {
                uiController.RenderText(part.text, before2 + GameController.Instance.GetBattleDatas(key, value) + after2.ToString());
            }
            else
            {
                uiController.RenderText(part.text, str.ToString());
            }
        }
    }

    //��D�̑S�j��
    public void DeleteAllhandCard()
    {
        foreach (Transform child in cardViewParent)
        {
            Destroy(child.gameObject);
        }
        instanceCards.Clear();
    }

    //��D�̃J�[�h�\��
    public void HandCardView()
    {
        DeleteAllhandCard();

        int index = 0;
        //��D�̓��e�̎擾
        foreach(CardData.Param card_data in CardDataController.Instance.GetCardList(CardDataController.DeckState.HAND))
        {
            CardViewParts part = CreateHandCardView(card_data,cardViewParent);
            Vector2 vec = SetCardPos(index);
            part.body.anchoredPosition = vec;

            index++;
        }
    }

    bool issss = false;
    //�J�[�h�𐶐�����(�^���p)
    async public UniTask CreateLakky()
    {
        int res = UnityEngine.Random.Range(0, CardDataController.Instance.GetCardList(CardDataController.DeckState.ALL).Count);

        CardViewParts part = CardViewController.Instance.CreateHandCardView(CardDataController.Instance.GetCardList(CardDataController.DeckState.ALL)[res],cardViewParent);
        Vector2 vec = Vector2.zero;
        part.body.anchoredPosition = vec;
        await UniTask.Delay(1000);
        await part.body.gameObject.GetComponent<CardInteraction>().CardAnimation();

        CardData.Param cardData = part.body.gameObject.GetComponent<CardInteraction>().cardData;

        //���ʔ���
        await GameController.Instance.CardEffect(cardData);

        //�㏞�x����
        await GameController.Instance.CardCost(cardData,true);
    }

    private Vector2 SetCardPos(int index = 0)
    {
        return new Vector2(handcard_startpos.x + index * hand_offset, handcard_startpos.y);
    }

    //�ꗗ�\���̃J�[�h�\��
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
        //�V���b�t������
        if(isShuffle)card_views = card_views.OrderBy(card => card.CostID).ToList();

        //��D�̓��e�̎擾
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
    /// ���̃J�[�h�������h���[����
    /// </summary>
    /// <returns></returns>
    public async UniTask DrawCardLOVE()
    {
        Debug.Log("�J�[�h���h���[");
        //�J�[�h������
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

    //�f�b�L�̏�ɐV�K�J�[�h�ǉ�
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
