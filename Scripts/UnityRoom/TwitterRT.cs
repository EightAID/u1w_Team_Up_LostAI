using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwitterRT : MonoBehaviour
{
    // �������̏����l�ł͂Ȃ��A�C���X�y�N�^��̒l��ς��Ă�������
    public string gameID = "lostai"; // unityroom��œ��e�����Q�[����ID
    public string tweetText1 = "�����́A";
    public string tweetText2 = "����؂Ȃ��̂��c������";
    public string hashTags = "\n#unityroom #�㏞���� #unity1week";

    [SerializeField] TextMeshProUGUI text;

    // �c�C�[�g�{�^������Ăяo�����J���\�b�h
    public void Tweet(int i)
    {
        switch(i)
        {
            case 0://�N���A��
                naichilab.UnityRoomTweet.Tweet(gameID, $"�����́y{GameController.DeckCount}���z�A��؂Ȃ��̂��c�����B\n#unityroom #�㏞���� #unity1week");
                break;
            case 1://�Q�[���I�[�o�[
                naichilab.UnityRoomTweet.Tweet(gameID, $"�yGame Over�z\n{text.text}�c\n#unityroom #�㏞���� #unity1week");
                break;
            case 2:
                naichilab.UnityRoomTweet.Tweet(gameID, $"#�㏞���� ��V�񂾁B\n#unityroom #unity1week");
                break;
        }
    }

    public void URL(int i)
    {
        switch(i)
        {
            case 0:
                Application.OpenURL("https://x.com/AIDunity");//""�̒��ɂ͊J������Web�y�[�W��URL����͂��܂�
                break;
            case 1:
                Application.OpenURL("https://x.com/beniimo_game");//""�̒��ɂ͊J������Web�y�[�W��URL����͂��܂�
                break;
            case 2:
                Application.OpenURL("https://unityroom.com/games/lostedai");//""�̒��ɂ͊J������Web�y�[�W��URL����͂��܂�
                break;
        }
    }
}
