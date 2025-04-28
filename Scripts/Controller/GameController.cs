using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using unityroom.Api;
using static CardDataController;
using static CardViewController;
using static UnityEngine.Rendering.DebugUI;

//�Q�[���̓�����i��
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    [SerializeField] public PlayerController playerController;
    [SerializeField] public EnemyController enemyController;
    [SerializeField] EndButton endButton;

    private bool isRunning = true;

    private float kokyuuuTime = 0;

    public int TurnCount = 0;

    public static bool isRetry = false;

    public bool isFinish = false;

    /// <summary>
    /// �ŏI�I�ɉ����c������
    /// </summary>
    public static int DeckCount = 34;

    /// <summary>
    /// �Q�[���̌��ݏ�Ԃ̎擾
    /// </summary>
    public enum GameState
    {
        START,
        DRAW,
        HANDAN,
        SELECT,
        USE,
        END,
        ITIRAN,
        PLAYERDEAD,
        ENEMYDEAD,
        ENDING,
        LASTITIRAN,
    }
    public GameState nowState = GameState.START;

    //�����̎�������@�\
    public Toggle Autotoggle;
    public static bool isTypeAuto =false;

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

    // Start is called before the first frame update
    void Start()
    {
        CardDataController.Instance.ResetCardata();
        if (isRetry)//���g���C���Ă�Ȃ�
        {
            isRetry = false;
            Debug.Log("�f�[�^�̃��[�h");
            Road();
        }
        else
        {
            isCardUses = new bool[100];
            SaveIsCardUses = new bool[100];
        }
        Save();
        UpdateLoop().Forget();
    }

    //Update�֐�
    private async UniTaskVoid UpdateLoop()
    {
        while (isRunning)
        {
            if (isCardUses[32] && nowState != GameState.ENDING && nowState != GameState.LASTITIRAN)//�ċz
            {
                kokyuuuTime += Time.deltaTime;
                if(kokyuuuTime > 4)
                {
                    kokyuuuTime = 0;
                    //�G�̃^�[��
                    playerController.Damage(2);

                    //����
                    bool isSet = await GameSet();
                }
            }

            switch (nowState)
            {
                case GameState.START:
                    Debug.Log("�����ݒ�");
                    Save();
                    playerController.ResetPlayer();
                    enemyController.ResetEnemy();
                    TurnCount = 0;
                    UIController.Instance.RenderText(UIController.Instance.DeckCountSmallText, CardDataController.Instance.GetCardList(DeckState.DECK).Count.ToString());
                    SoundController.Instance.PlayBGM(0, 0);
                    beforeData = null;
                    //UI�p�l�������
                    UIController.Instance.MenuPanels(false);
                    UIController.Instance.ItiranPanels(false);
                    UIController.Instance.TitileBackOpen(false);


                    await UIController.Instance.StartCutOut();

                    endButton.SetButtonColor(true);

                    await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);

                    await StageController.Instance.StartSerif();

                    nowState = GameState.DRAW;
                    break;
                case GameState.DRAW:
                    TurnCount++;
                    if (TurnCount == 1 && StageController.StageIndex == StageController.Instance.StageName.Length-1)
                    {
                        await CardViewController.Instance.AddEXCard();
                    }

                    await CardViewController.Instance.DrawCardView();

                    if (isCardUses[25])//�^���_�o���Ȃ�
                    {
                        playerController.Damage(1);
                        FaceView.Instance.SetOption(FaceView.OptionState.YOGORE);

                        if(isCardUses[7])//�Ɋo
                        {
                            if (isCardUses[23])//�m��
                                UIController.Instance.OpenSerif("����񂾂��ǁA�������Ȃ�");
                            else
                                UIController.Instance.OpenSerif("�]�񂶂�����c�ł��ɂ��Ȃ�");
                        }
                        else
                        {
                            if (isCardUses[23])//�m��
                                UIController.Instance.OpenSerif("����񂶂�����c");
                            else
                                UIController.Instance.OpenSerif("�]�񂶂�����c");
                        }

                        await WaitForClickOrTimeout();
                        UIController.Instance.CloseSerif();
                        await UniTask.Delay(100);
                    }

                    if (isCardUses[11])//���f�͂���������
                    {
                        nowState = GameState.HANDAN;
                    }
                    else
                    {
                        UIController.Instance.RenderText(UIController.Instance.DeckCountSmallText,CardDataController.Instance.GetCardList(DeckState.DECK).Count.ToString());

                        //1�ʂ�������`���[�g���A���ɓ���
                        if(TurnCount == 1 && StageController.StageIndex == 0) await StageController.Instance.Thutorial();

                        nowState = GameState.SELECT;
                    }
                    break;
                case GameState.HANDAN:
                    UIController.Instance.OpenSerif("(�Ȃɂ��l�����Ȃ���c�I)");
                    await WaitForClickOrTimeout();
                    UIController.Instance.CloseSerif();

                    UnityEngine.Random.InitState(DateTime.Now.Millisecond);
                    await CardViewController.Instance.instanceCards[UnityEngine.Random.Range(0, CardViewController.Instance.instanceCards.Count)].body.gameObject.GetComponent<CardInteraction>().UseCard();
                    break;
                case GameState.SELECT:
                    if (CardDataController.isCardUses[22] && CardDataController.Instance.GetCardList(DeckState.HAND).Count > 3)
                    {
                        endButton.SetButtonColor(true);
                    }
                    else
                    {
                        endButton.SetButtonColor(false);
                    }

                    break;
                case GameState.USE:
                    endButton.SetButtonColor(true);
                    break;
                case GameState.END:
                    endButton.SetButtonColor(true);
                    
                    break;
                case GameState.PLAYERDEAD:
                    endButton.SetButtonColor(true);
                    await UIController.Instance.AnimateTransitionAsync(1, 0, 2);
                    SceneController.Instance.LoadSceneIndex(2);
                    return;
                case GameState.ENEMYDEAD:
                    endButton.SetButtonColor(true);
                    await enemyController.StartDisappear();
                    //���̃X�e�[�W�N���A

                    if(StageController.StageIndex == StageController.Instance.StageName.Length-1)//�ŏI�X�e�[�W�Ȃ�
                    {
                        nowState = GameState.ENDING;
                        if (isFinish) return;
                        isFinish = true;

                        SoundController.Instance.PlayBGM(2);
                        CardDataController.Instance.ResetDeck();

                        await StageController.Instance.LastSerif();

                        //���̃J�[�h���h���[����
                        await CardViewController.Instance.DrawCardLOVE();
                        // �{�[�hNo1�ɂ̂���J�[�h�����𑗐M����B
                        UnityroomApiClient.Instance.SendScore(1, CardDataController.Instance.GetCardList(DeckState.DECK).Count, ScoreboardWriteMode.HighScoreDesc);
                        DeckCount = CardDataController.Instance.GetCardList(DeckState.DECK).Count;
                    }
                    else
                    {
                        await UIController.Instance.AnimateTransitionAsync(1, 0, 2);
                        CardDataController.Instance.ResetDeck();
                        StageController.StageIndex++;
                        nowState = GameState.START;
                    }
                    break;
                case GameState.ENDING:


                    break;
            }

            await UniTask.Yield(); // ���̃t���[���܂ő҂�
        }
    }

    /// <summary>
    /// �J�[�h���g�p�����Ƃ��̏���
    /// </summary>
    async public UniTask UseCard(CardData.Param cardData,bool isLakky = false)
    {
        nowState = GameState.USE;
        Debug.Log("�J�[�h���g�p����");
        CardDataController.Instance.MoveCard(cardData, DeckState.HAND, DeckState.EXCLUSION);
        CardViewController.Instance.HandCardView();

        //���ʔ���
        await CardEffect(cardData);

        //�㏞�x����
        await CardCost(cardData);

        await UniTask.Delay(500);

        if (isLakky) return;//�^���Ȃ�A�Ԃ�

        bool isSet = await GameSet();
        if (isSet) return;

        if (CardDataController.isCardUses[0])
        {
            nowState = GameState.PLAYERDEAD;
            return;
        }

        if (isCardUses[11])//���f�͂���������
        {
            nowState = GameState.SELECT;
            EndButton();
        }
        else
        {
            nowState = GameState.SELECT;
        }
    }

    private async UniTask<bool> GameSet()
    {
        //���S����
        if (playerController.playerHaert <= 0)
        {
            nowState = GameState.PLAYERDEAD;
            return true;
        }
        else if (enemyController.enemyHaert <= 0)
        {
            nowState = GameState.ENEMYDEAD;
            return true;
        }

        return false;
    }


    public static CardData.Param beforeData;
    public static CardData.Param usebeforeData;
    /// <summary>
    /// ���ʔ���
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    async public UniTask CardEffect(CardData.Param cardData)
    {
        if (cardData == null) return;
        switch (cardData.CostID)
        {
            case 0:
                break;
            case 1://����
            case 2://���o
                enemyController.Damage(GetBattleDatas("damage", 15));
                break;
            case 3://���o
                enemyController.Damage(GetBattleDatas("damage", 10));
                break;
            case 4://�Ί�
                isSmaile = true;
                break;
            case 6:
                enemyController.Damage(GetBattleDatas("damage", 5));
                break;
            case 7://�Ɋo
                await CardViewController.Instance.DiscordCardView();
                //�J�[�h���̂ĎD�ɑ���
                CardDataController.Instance.DiscordCard();
                await UniTask.Delay(300);
                await CardViewController.Instance.DrawCardView();
                break;
            case 8:
            case 9:
            case 10:
                playerController.Health(GetBattleDatas("health", 5));
                break;
            case 11://�v�l��
                enemyController.Damage(GetBattleDatas("damage", 15));
                break;
            case 12:
            case 13:
                enemyController.Damage(GetBattleDatas("damage", 8));
                await UniTask.Delay(200);
                playerController.Damage(1);
                break;
            case 14:
            case 15:
                enemyController.Damage(GetBattleDatas("damage", 8));
                break;
            case 16:
            case 17:
                enemyController.Damage(GetBattleDatas("damage", 5));
                break;
            case 18:
                enemyController.Damage(GetBattleDatas("damage", 20));
                await UniTask.Delay(200);
                playerController.Damage(5);
                break;
            case 19:
                enemyController.Damage(GetBattleDatas("damage", 12));
                AddHearlth -= 2;
                break;
            case 20:
                enemyController.Damage(GetBattleDatas("damage", 5));
                break;
            case 21:
                playerController.Health(GetBattleDatas("health", 5));
                break;
            case 22:
                enemyController.Damage(GetBattleDatas("damage", 9));
                break;
            case 23:
                AddDamage += 2;
                break;
            case 24:
                AddHearlth += 2;
                break;
            case 25://�^���_�o
                isBaria = true;
                break;
            case 26://�{��
                AddDamage -= 2;
                AddHearlth += 2;
                break;
            case 27:
                isKanasimi = true;
                break;
            case 28:
                AddDamage += 3;
                break;
            case 29:
                await CardEffect(beforeData);
                usebeforeData = beforeData;
                Debug.Log("��]�͂���`�`�`�`�`�`�`");
                break;
            case 30://��]
                await CardViewController.Instance.DiscordCardView();
                //�J�[�h���̂ĎD�ɑ���
                CardDataController.Instance.DiscordCard();
                await UniTask.Delay(300);
                await CardViewController.Instance.DrawCardView();
                break;
            case 31://��
                enemyController.Damage(GetBattleDatas("damage", 4));
                await UniTask.Delay(300);
                playerController.Health(GetBattleDatas("health", 4));
                break;
            case 32://�ċz
                enemyController.Damage(GetBattleDatas("damage", 15));
                break;
            case 33://�ċz
                enemyController.Damage(GetBattleDatas("damage", 5));
                break;
            case 34://�g���E�}
                playerController.Damage(10);
                AddDamage = AddDamage - 2;
                AddHearlth = AddHearlth - 2;
                break;
            case 35://���b�L�[
                beforeData = cardData;
                await CardViewController.Instance.CreateLakky();
                CardViewController.Instance.HandCardView();
                return;
        }

        beforeData = cardData;
        CardViewController.Instance.HandCardView();
        await UniTask.Delay(300);
    }

    //�_���[�W����
    public static bool TryParsePlaceholder(string template, out string key, out int value, out string before, out string after)
    {
        key = null;
        value = 0;
        before = null;
        after = null;

        Match match = Regex.Match(template, @"\{(.*?):(\d+)\}");
        if (match.Success)
        {
            key = match.Groups[1].Value;
            if (int.TryParse(match.Groups[2].Value, out value))
            {
                before = template.Substring(0, match.Index);
                after = template.Substring(match.Index + match.Length);
                return true;
            }
        }
        return false;
    }

    //�B�ŕ���
    public static bool TrySplitByPeriod(string input, out string before, out string after)
    {
        before = null;
        after = null;

        int index = input.IndexOf('�B');
        if (index >= 0 && index < input.Length - 1)
        {
            before = input.Substring(0, index);
            after = input.Substring(index + 1); // �u�B�v�̎�����Ō�܂�
            return true;
        }

        return false;
    }

    public int AddDamage = 0;
    public int AddHearlth = 0;

    public bool isSmaile = false;
    public bool isBaria = false;
    public bool isKanasimi = false;


    //�~�����������Ԃ�
    public int GetBattleDatas(string key,int value)
    {
        int result = value;
        Debug.Log($"{key}:{value}");
        switch (key)
        {
            case null:
                return 0;
            case "damage":
                if(isSmaile)
                {
                    return AddDamage + result + 10;
                }
                else return AddDamage + result;
            case "health":
                return AddHearlth +result;
        }

        return 0;
    }

    /// <summary>
    /// �㏞
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    async public UniTask CardCost(CardData.Param cardData,bool islakky = false)
    {
        await UniTask.Delay(300);
        //�J�[�h���g�p�ς݂Ƃ���
        CardDataController.isCardUses[cardData.CostID] = true;
        Debug.Log($"ID:{cardData.CostID}:{cardData.Name}");
        switch (cardData.CostID)
        {
            case 0://��
                   //�G�̃^�[��
                playerController.Damage(9999);
                await UIController.Instance.AnimateTransitionAsyncLove(1, 0.5f, 1);
                await UniTask.Delay(1000);
                UIController.Instance.OpenSerif("������������A�l�́c");
                await WaitForClickOrTimeout();
                UIController.Instance.CloseSerif();

                break;
            case 1://���t
                await UIController.Instance.AnimateTransitionAsync(1, 0, 1f);
                CardViewController.Instance.HandCardView();
                playerController.SetDamageText(playerController.playerHaert);
                enemyController.SetDamageText(enemyController.enemyHaert);
                UIController.Instance.RenderText(UIController.Instance.DeckCountSmallText, CardDataController.Instance.GetCardList(DeckState.DECK).Count.ToString());

                //UI�܂��̕�������
                UIController.Instance.MozibakeruUI();

                await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);
                break;
            case 2://���o
                await UIController.Instance.AnimateTransitionAsync(1, 0, 1f);
                CardViewController.Instance.HandCardView();
                UIController.Instance.HeartHide();
                await playerController.SetFace();
                await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);
                break;
            case 3://���o
                await SoundController.Instance.StopAllAudioUT();
                break;
            case 4://�Ί�
                FaceView.Instance.SetFaceView(FaceView.FaceState.NORMAL);
                break;
            case 7://�Ɋo
                await UIController.Instance.AnimateTransitionAsync(1, 0, 0.8f);
                CardViewController.Instance.HandCardView();
                FaceView.Instance.SetFaceView(FaceView.FaceState.EGAO, FaceView.ArmState.UPUP, FaceView.OptionState.SIROME);
                await UIController.Instance.AnimateTransitionAsync(0, 1, 0.8f);
                break;
            case 12://��
                await UIController.Instance.AnimateTransitionAsync(1, 0, 1f);
                CardViewController.Instance.HandCardView();
                FaceView.Instance.SetFaceView();
                await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);
                break;
            case 14://�F�o
                await UIController.Instance.AnimateTransitionAsync(1, 0, 1f);
                CardViewController.Instance.HandCardView();
                Glayscale.Instance.SetGlayscale();
                await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);
                break;
            case 23://�m��
                await UIController.Instance.AnimateTransitionAsync(1, 0, 1f);
                CardViewController.Instance.HandCardView();
                await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);
                break;
            case 25://�^���_�o
                await UIController.Instance.AnimateTransitionAsync(1, 0, 1f);
                CardViewController.Instance.HandCardView();
                FaceView.Instance.SetOption(FaceView.OptionState.YOGORE);
                await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);
                break;
            case 31://��
                await UIController.Instance.AnimateTransitionAsync(1, 0, 1f);
                CardViewController.Instance.HandCardView();
                UIController.Instance.faceHide();
                await UIController.Instance.AnimateTransitionAsync(0, 1, 1.5f);
                break;
        }
        await UniTask.Delay(500);

        string resultText = cardData.resultText;

        if (isCardUses[23])//�m��
        {
            resultText = cardData.resultText2;
        }

        //���ʂ��o��
        if (isCardUses[2] && cardData.CostID == 14)//���o�����ĂāA�F�o�g�p��
        {
            if (isCardUses[23]) resultText = "�Ȃɂ���������񂾂낤�H";
            else resultText = "�����ς�����񂾂낤�H";
        }

        if (isCardUses[2] && cardData.CostID == 31)//���o�������āA��g�p
        {
            if (isCardUses[23]) resultText = "�Ȃɂ���������񂾂낤�H";
            else resultText = "�����ς�����񂾂낤�H";
        }

        //�S�Ă̊����������
        if (CardDataController.isCardUses[26] && CardDataController.isCardUses[27] && CardDataController.isCardUses[28] && CardDataController.isCardUses[29] && CardDataController.isCardUses[30])
        {
            resultText = "�����A�Ȃ�ł������ȁc";
        }


        if (islakky)
        {
            return;
        }

        UIController.Instance.OpenSerif(resultText);

        await WaitForClickOrTimeout();
        UIController.Instance.CloseSerif();
    }

    /// <summary>
    /// �I���{�^���������̏���
    /// </summary>
    public async void EndButton()
    {
        if (nowState != GameState.SELECT && nowState != GameState.HANDAN) return;
        if (nowState == GameState.SELECT && CardDataController.isCardUses[22] && CardDataController.Instance.GetCardList(DeckState.HAND).Count > 3) return;
        nowState = GameState.END;
        SoundController.Instance.PlaySFX(2);
        await CardViewController.Instance.DiscordCardView();
        //�J�[�h���̂ĎD�ɑ���
        CardDataController.Instance.DiscordCard();

        //�G�̃^�[��
        enemyController.EndAttack();


        //����
        bool isSet = await GameSet();
        if (isSet) return;

        nowState = GameState.DRAW;
    }


    void OnDestroy()
    {
        isRunning = false;
    }

    public async void SerifCutIn(string text)
    {
        UIController.Instance.OpenSerif(text);
        await WaitForClickOrTimeout();
        UIController.Instance.CloseSerif();
    }

    /// <summary>
    /// �N���b�N�҂��֐�
    /// </summary>
    /// <returns></returns>
    public async UniTask WaitForClickOrTimeout()
    {

        if (isTypeAuto)
        {
            var clickTask = UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            var timeoutTask = UniTask.Delay(5000);
            await UniTask.WhenAny(clickTask, timeoutTask);
        }
        else
        {
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        SoundController.Instance.PlaySFX(8);
    }

    public void SetAutoToggle()
    {
        isTypeAuto = Autotoggle.isOn;
    }

    public void Save()
    {
        for(int i = 0; i < CardDataController.isCardUses.Length; i++)
        {
            CardDataController.SaveIsCardUses[i] = isCardUses[i];
        }
        CardDataController.SaveLife = playerController.playerHaert;
    }

    public void Road()
    {
        playerController.playerHaert= CardDataController.SaveLife;

        playerController.Health(5);

        //�Z�[�u�z�񂩂�擾����
        for (int i = 0; i < CardDataController.isCardUses.Length; i++)
        {
            isCardUses[i] = SaveIsCardUses[i];
            if (i == 34) continue;

            if (isCardUses[i])//�g�p�ς݂Ȃ�
            {
                //���Ɏg�p�ς݂̃J�[�h���f�b�L����ړ�������
                CardDataController.Instance.MoveCard(CardDataController.Instance.GetCardData(i), DeckState.DECK, DeckState.EXCLUSION);

                Debug.Log($"{i}:{CardDataController.Instance.GetCardData(i).CostID}");
                switch (i)
                {

                    case 1://���t
                        playerController.SetDamageText(playerController.playerHaert);
                        enemyController.SetDamageText(enemyController.enemyHaert);
                        UIController.Instance.RenderText(UIController.Instance.DeckCountSmallText, CardDataController.Instance.GetCardList(DeckState.DECK).Count.ToString());

                        //UI�܂��̕�������
                        UIController.Instance.MozibakeruUI();
                        break;
                    case 2://���o
                        UIController.Instance.HeartHide();
                        playerController.SetFace();
                        break;
                    case 3://���o
                        SoundController.Instance.StopALLAudio();
                        break;
                    case 4://�Ί�
                        FaceView.Instance.SetFaceView(FaceView.FaceState.NORMAL);
                        break;
                    case 7://�Ɋo
                        FaceView.Instance.SetFaceView(FaceView.FaceState.EGAO, FaceView.ArmState.UPUP, FaceView.OptionState.SIROME);
                        break;
                    case 12://��
                        FaceView.Instance.SetFaceView();
                        break;
                    case 14://�F�o
                        Glayscale.Instance.SetGlayscale();
                        break;
                    case 31://��
                        UIController.Instance.faceHide();
                        break;
                }

                isSmaile = false;
                //�d�l����
                switch (i)
                {
                    case 4://�Ί�
                        isSmaile = true;
                        break;
                    case 19:
                        AddHearlth -= 2;
                        break;
                    case 23:
                        AddDamage += 2;
                        break;
                    case 24://�E�C
                        AddHearlth += 2;

                        break;
                    case 25:
                        isBaria = true;
                        break;
                    case 26://�{��

                        AddDamage -= 2;
                        AddHearlth += 2;
                        break;
                    case 27:
                        isKanasimi = true;
                        break;
                    case 28:
                        AddDamage += 3;
                        break;
                    case 29://��]
                        if(usebeforeData != null)RoadEffect(usebeforeData.CostID);
                        break;
                    case 34://�ċz
                        AddDamage = Math.Max(0, AddDamage - 2);
                        AddHearlth = Math.Max(0, AddHearlth - 2);
                        break;
                }
            }
        }
        usebeforeData = null;
        CardViewController.Instance.HandCardView();
    }


    void RoadEffect(int i)
    {
        switch (i)
        {
            case 4://�Ί�
                isSmaile = true;
                break;
            case 19:
                AddHearlth -= 2;
                break;
            case 23:
                AddDamage += 2;
                break;
            case 24://�E�C
                AddHearlth += 2;

                break;
            case 25:
                isBaria = true;
                break;
            case 26://�{��

                AddDamage -= 2;
                AddHearlth += 2;
                break;
            case 27:
                isKanasimi = true;
                break;
            case 28:
                AddDamage += 3;
                break;
            case 34://�ċz
                AddDamage = Math.Max(0, AddDamage - 2);
                AddHearlth = Math.Max(0, AddHearlth - 2);
                break;
        }
    }
}
