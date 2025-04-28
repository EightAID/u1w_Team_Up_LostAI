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
    [Header("�ł������")]
    [SerializeField] private Material dissolveMaterial;

    [Header("�J�[�h�T�C�Y�̐ݒ�")]
    [SerializeField] float startsize = 0.15f;
    [SerializeField] float entersize = 0.20f;

    [Header("���̃J�[�h��rt")]
    [SerializeField] RectTransform rectTransform;
    //�J�[�h�̔w��
    [SerializeField] GameObject backImage_obj;
    [SerializeField] RectTransform HideObj;

    [Header("�J�[�h���g�p���邽�߂̍��Woffset")]
    [SerializeField] float useCardPosY = 0;

    [Header("�g�p��UI�p���X�g")]
    [SerializeField] List<RectTransform> frontui;
    [SerializeField] UIEffect FadeTransitionEffect;

    //���̃J�[�h�̃f�[�^
    public CardData.Param cardData;

    [SerializeField] Image[] image;

    //�J�[�h�̐�s����
    bool isInputCard = false;

    bool isKeyDown = false;
    float clicktTime = 0;

    bool isUsed = false;

    private void Start()
    {
        rectTransform.localScale = new Vector2(startsize, startsize);

        //���ɃO���[�Ȃ�
        if (CardDataController.isCardUses[14])
        {
            foreach (var item in image)
            {
                Glayscale.Instance.SetGlay(item);
            }
            if (FadeTransitionEffect != null) FadeTransitionEffect.toneIntensity = 1.0f;
        }

        //���o�������Ă���̂Ȃ�
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
        //�����{�����Ă��Ȃ��Ƃ��ɁA�J�[�h�̏�Ƀ|�C���^�[�u���Ă��l����
        if(GameController.Instance.nowState == GameController.GameState.SELECT && isInputCard)
        {
            isInputCard = false;
            EnterAction();
        }

        //��]�����Z�b�g����
        if (rectTransform.rotation.eulerAngles.y >= 360 || rectTransform.rotation.eulerAngles.y < -360)
        {
            Debug.Log("��]�̃��Z�b�g");
            //�x���@���̗p
            rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        //��]�I�ɗ��̎���
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

    // �|�C���^�[�������ꂽ�Ƃ�
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
        isKeyDown = true;
        clicktTime = 0;
    }

    // �|�C���^�[�������ꂽ�Ƃ�
    public async void OnPointerUp(PointerEventData eventData)
    {
        isKeyDown = false;
        if (GameController.Instance.nowState != GameController.GameState.SELECT && GameController.Instance.nowState != GameController.GameState.ENDING) return;
        Debug.Log("Pointer Up");
        //�J�[�h���g�p����͈͈ȏ�Ȃ�
        if(rectTransform.anchoredPosition.y >= useCardPosY)
        {
            if (GameController.Instance.nowState != GameController.GameState.ENDING) GameController.Instance.nowState = GameController.GameState.USE;

            await UseCard();
        }
        else
        {
            //���͂��Z��������g�p����
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
        //���̃I�u�W�F�N�g���A0,150�̍��W�Ɉړ�������

        Vector2 targetPosition = new Vector2(0, 200);
        Vector3 originalScale = rectTransform.localScale;
        Vector3 emphasizedScale = originalScale * 1.1f;

        float moveDuration = 0.2f;

        // Sequence�ŃA�j���[�V�������܂Ƃ߂�
        Sequence sequence = DOTween.Sequence();

        sequence.Join(rectTransform.DOAnchorPos(targetPosition, moveDuration).SetEase(Ease.InOutQuad));
        sequence.Join(rectTransform.DOScale(emphasizedScale, moveDuration / 2).SetLoops(2, LoopType.Yoyo));
        sequence.Join(rectTransform.DORotate(new Vector3(0, 180, 0), 0.8f, RotateMode.WorldAxisAdd));

        SoundController.Instance.PlaySFX(3);
        await sequence.AsyncWaitForCompletion();
        await UniTask.Delay(400);

        //�J�[�h���폜����
        SoundController.Instance.PlaySFX(4);

        await AnimateTransitionAsync(0, 1, 1.3f);
    }

    public async UniTask UseCard()
    {
        await CardAnimation();

        if (isUsed) return;
        //�ŏI�̂�
        if (GameController.Instance.nowState == GameController.GameState.ENDING)
        {
            isUsed = true;
            PrologController.isClear = true;
            Debug.Log("�N���A����");
            CardDataController.Instance.MoveCard(cardData,CardDataController.DeckState.HAND,CardDataController.DeckState.DECK);
            await UIController.Instance.AnimateTransitionAsyncFlash(1, 0, 3f);
            await StageController.Instance.LastSerif2();
            return;
        }

        await UniTask.Delay(300);
        await GameController.Instance.UseCard(cardData);
    }

    // �h���b�O��
    public void OnDrag(PointerEventData eventData)
    {
        if (GameController.Instance.nowState != GameController.GameState.SELECT && GameController.Instance.nowState != GameController.GameState.ENDING) return;
        Vector2 mousePos = Input.mousePosition;
        rectTransform.position = mousePos;
        transform.SetAsLastSibling();
        UIController.Instance.CloseSerif();
    }

    // �J�[�\�����������Ƃ�
    //�ڍ׏���\�������肵���������Ȃ�
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

        //�S�Ă̊����������
        if(CardDataController.isCardUses[26] && CardDataController.isCardUses[27] && CardDataController.isCardUses[28] && CardDataController.isCardUses[29] && CardDataController.isCardUses[30])
        {
            UIController.Instance.OpenSerif("�����A�ǂ��ł������ȁc");
            return;
        }

        //���o�����Ă���
        if (CardDataController.isCardUses[2])
        {
            if (CardDataController.isCardUses[23])
                UIController.Instance.OpenSerif("�Ȃɂ��݂��Ȃ���c");
            else
                UIController.Instance.OpenSerif("�����c�Ȃɂ������Ȃ��c");
            return;
        }

        //���|�S�̂Ƃ�
        if (CardDataController.isCardUses[22])
        {
            if (CardDataController.isCardUses[2])//���o
                UIController.Instance.OpenSerif("�킽���A�܂�����̂ւ����I");
            else
                UIController.Instance.OpenSerif("�킽���A�Â��ꏊ���C����");
            return;
        }

        if (GameController.Instance.nowState == GameController.GameState.ENDING)
        {
            if (CardDataController.isCardUses[23])
                UIController.Instance.OpenSerif("(�킽���ɂƂ��āA����͂������Ȃ��̂�����c�I)");
            else
                UIController.Instance.OpenSerif("(�킽���ɂƂ��āA����͑厖�Ȃ��̂�����c�I)");
            return;
        }
        else
        {

            string text = cardData.flavar;
            //�������

            if (CardDataController.isCardUses[18])//���e�؂�����
            {
                if (CardDataController.isCardUses[23])
                {
                    switch (cardData.CostID)
                    {
                        case 5:
                            text = "�Ȃ܂������c�����������ꂪ�t�����񂾂낤";
                            break;
                        case 8:
                            text = "�͂�΁[���A���������Ȃ񂾁c";
                            break;
                        case 10://���~
                            text = "����[���������������Ă��ƁH";
                            break;
                        case 17:
                            text = "���񂶂傤�сA�킽���͂���܂�Ƃ��ׂ��Ă����������ƂȂ��c";
                            break;
                        case 20:
                            text = "���͂��킭�Ă����Ȃ��c";
                            break;
                    }
                }
                else
                {
                    switch (cardData.CostID)
                    {
                        case 5:
                            text = "(���O�c�R���Ȃ񂾂�����)";
                            break;
                        case 8:
                            text = "�n���o�[�O�H�ׂ����c";
                            break;
                        case 10://���~
                            text = "�L�X�������C�������Ă��ƁH";
                            break;
                        case 17:
                            text = "�a���������c�킽���A�j���Ă�������L�����Ȃ��c";
                            break;
                        case 20:
                            text = "������͕|���čs���Ȃ������Ȃ��c";
                            break;
                    }
                }

            }

            if (CardDataController.isCardUses[23]) text = cardData.flavar2;

            UIController.Instance.OpenSerif(text);
        }

        rectTransform.localScale = new Vector2(entersize, entersize);
    }

    // �J�[�\�����o���Ƃ�
    public void OnPointerExit(PointerEventData eventData)
    {
        isInputCard = false;
        if (GameController.Instance.nowState != GameController.GameState.SELECT && GameController.Instance.nowState != GameController.GameState.ENDING) return;
        UIController.Instance.CloseSerif();
        rectTransform.localScale = new Vector2(startsize, startsize);
    }

    /// <summary>
    /// Transition��񓯊��ɃA�j���[�V����
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
            await UniTask.Yield(); // ���̃t���[���܂őҋ@
        }
        FadeTransitionEffect.transitionRate = to; // �ŏI�l�Ŋm�肳����
    }
}
