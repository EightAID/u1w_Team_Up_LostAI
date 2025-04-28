using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI GameOverText;
    [SerializeField] TextMeshProUGUI GameOverSerifText;
    [SerializeField] TextMeshProUGUI KyouyuuText;
    [SerializeField] TextMeshProUGUI[] text;

    // Start is called before the first frame update
     async void Start()
    {
        UIController.Instance.RenderText(GameOverText, "Game Over");
        UIController.Instance.RenderText(GameOverSerifText, "");
        UIController.Instance.RenderText(KyouyuuText, "���L����");
        UIController.Instance.RenderText(text[0],"�X�^�[�g�֖߂�");
        UIController.Instance.RenderText(text[1],"���݂̃X�e�[�W�����蒼��");

        SoundController.Instance.PlayBGM(2);

        await UIController.Instance.AnimateTransitionAsync(0, 1, 3);
        await UniTask.Delay(500);
        SetSerif();
    }

    List<int> isCardIndex = new List<int>();
    public void SetSerif()
    {
        isCardIndex.Clear();
        for (int i = 0;  i< CardDataController.isCardUses.Length; i++)
        {
            if (CardDataController.isCardUses[i])
            {
                isCardIndex.Add(i);
            }
        }

        if (CardDataController.isCardUses[0])
        {
            UIController.Instance.TypeText(GameOverSerifText, "��ԑ�؂Ȃ��̂��A\n�����Ă��܂���");
            return;
        }


        int index = 0;
        if (GameController.beforeData != null)
            index = GameController.beforeData.CostID;
        //if (isCardIndex.Count > 0)
        //{
        //    index = Random.Range(0, isCardIndex.Count);
        //    index = isCardIndex[index];
        //}

        switch (index)
        {
            case 0:
                if(CardDataController.isCardUses[0])
                {
                    UIController.Instance.TypeText(GameOverSerifText, "��ԑ�؂Ȃ��̂��A\n�����Ă��܂���");
                }
                else//�f�t�H���g
                {
                    UIController.Instance.TypeText(GameOverSerifText, "�����o��A\n���ꂪ����Ȃ���������");
                }
                break;
            case 1:
                UIController.Instance.TypeText(GameOverSerifText, "����́A\n�Ӗ������邩��ǂ߂��");
                break;
            case 2:
                UIController.Instance.TypeText(GameOverSerifText, "���������Ȃ��B\n���i����A�ڂɗ��肷���Ă�����");
                break;
            case 3:
                UIController.Instance.TypeText(GameOverSerifText, "�����������Ȃ��A\n��؂Ȑl���ĂԐ������");
                break;
            case 4:
                UIController.Instance.TypeText(GameOverSerifText, "�Ί炪�f�G�I\n�̌���ꂽ���Ƃ��v���o��");
                break;
            case 5:
                UIController.Instance.TypeText(GameOverSerifText, "�����N���A\n�킽���̂��Ƃ��Ă�ł͂���Ȃ�");
                break;
            case 6:
                UIController.Instance.TypeText(GameOverSerifText, "�L�b�`������Y�����������A\n�]���ɂ��т�t���Ă���");
                break;
            case 7:
                UIController.Instance.TypeText(GameOverSerifText, "�ɂ݂��Ȃ��Ă��A\n�����Ă��Ȃ�����");
                break;
            case 8:
                UIController.Instance.TypeText(GameOverSerifText, "�ǂ�ȐH�ו����A\n�����̕��Ɍ�����c");
                break;
            case 9:
                UIController.Instance.TypeText(GameOverSerifText, "���Ԃ���R���\n���̗z�������D��������");
                break;
            case 10:
                UIController.Instance.TypeText(GameOverSerifText, "�܂����ꂪ�Ȃɂ���������Ȃ��܂܁A\n�킽���́c");
                break;
            case 11:
                UIController.Instance.TypeText(GameOverSerifText, "�l���邱�Ƃ��A\n����������߂�");
                break;
            case 12:
                UIController.Instance.TypeText(GameOverSerifText, "���ꂪ�Ȃ��ƁA\n�Ȃɂ��ł��Ȃ�");
                break;
            case 13:
                UIController.Instance.TypeText(GameOverSerifText, "���ꂶ�Ⴀ�����A\n�ǂ��ւ��s���Ȃ�");
                break;
            case 14:
                UIController.Instance.TypeText(GameOverSerifText, "�킽���̍D���ȐF�A\n�ǂ�ȐF����������");
                break;
            case 15:
                UIController.Instance.TypeText(GameOverSerifText, "�l��1�ԍŏ���\n�Y�ꂿ�Ⴄ�̂͐��炵��");
                break;
            case 16:
                UIController.Instance.TypeText(GameOverSerifText, "��l�ɂȂ�Ȃ��܂܁A\n�I������Ⴄ�̂���");
                break;
            case 17:
                UIController.Instance.TypeText(GameOverSerifText, "��D���Ȃ��a�����p�[�e�B�A\n�����ł��Ȃ���");
                break;
            case 18:
                UIController.Instance.TypeText(GameOverSerifText, "�킽�����āA\n�N�̎q���Ȃ̂���");
                break;
            case 19:
                UIController.Instance.TypeText(GameOverSerifText, "�����C������́A\n�ӂ�ӂ�ŁA�����A���́c");
                break;
            case 20:
                UIController.Instance.TypeText(GameOverSerifText, "��������Ŗ�́A\n�|���Ȃ��Ȃ�̂���");
                break;
            case 21:
                UIController.Instance.TypeText(GameOverSerifText, "�����ꏏ�ɗV��ł��̂ɁA\n�Ȃ񂾂������̂̂��Ƃ݂���");
                break;
            case 22:
                UIController.Instance.TypeText(GameOverSerifText, "���������|���Ȃ��̂ɁA\n�ǂ�����");
                break;
            case 23:
                UIController.Instance.TypeText(GameOverSerifText, "�킽���A���׋����ł���\n���炢�q�������̂�");
                break;
            case 24:
                UIController.Instance.TypeText(GameOverSerifText, "�킽���ɂ͑���Ȃ���������A\n�����Ȃ����������");
                break;
            case 25:
                UIController.Instance.TypeText(GameOverSerifText, "���������^�����ł���΁A\n�����͂Ȃ�Ȃ������̂���");
                break;
            case 26:
                UIController.Instance.TypeText(GameOverSerifText, "�킽���������Ȃ�̂��A\n�d�����Ȃ�������");
                break;
            case 27:
                UIController.Instance.TypeText(GameOverSerifText, "����Ȗڂɑ����Ă���̂ɁA\n�ǂ����Ĕ߂����Ȃ��񂾂낤�c");
                break;
            case 28:
                UIController.Instance.TypeText(GameOverSerifText, "���Ȃ��͗D�����q����\n�����Ă��̂�");
                break;
            case 29:
                UIController.Instance.TypeText(GameOverSerifText, "�����ċA�肽�������A\n�������ꂾ���Ȃ̂�");
                break;
            case 30:
                UIController.Instance.TypeText(GameOverSerifText, "�킽���A����ς�\n�����Ȃ�^����������");
                break;
            case 31:
                UIController.Instance.TypeText(GameOverSerifText, "������x�ƁA\n�����˂��ĖJ�߂Ă��炦�Ȃ��̂���");
                break;
            case 33:
                UIController.Instance.TypeText(GameOverSerifText, "�N�b�L�[���āA\n�{���ɔ������������̂���");
                break;
            case 34:
                UIController.Instance.TypeText(GameOverSerifText, "���̐l�̂����ŁA\n�킽���́c");
                break;
            case 32://�ċz
                UIController.Instance.TypeText(GameOverSerifText, "�ꂵ���c�ꂵ���c\n�ӎ������̂��Ă����c");
                break;
            default:
                UIController.Instance.TypeText(GameOverSerifText, "�A�肽���A\n�������ꂾ���Ȃ̂�");
                break;
        }
    }

    async public void Retrybutton()
    {
        await UIController.Instance.AnimateTransitionAsync(1, 0, 2);
        GameController.isRetry = true;
        SceneController.Instance.LoadSceneIndex(1);
    }
}
