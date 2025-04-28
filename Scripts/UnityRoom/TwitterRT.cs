using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TwitterRT : MonoBehaviour
{
    // ※ここの初期値ではなく、インスペクタ上の値を変えてください
    public string gameID = "lostai"; // unityroom上で投稿したゲームのID
    public string tweetText1 = "少女は、";
    public string tweetText2 = "枚大切なものを残せたよ";
    public string hashTags = "\n#unityroom #代償少女 #unity1week";

    [SerializeField] TextMeshProUGUI text;

    // ツイートボタンから呼び出す公開メソッド
    public void Tweet(int i)
    {
        switch(i)
        {
            case 0://クリア時
                naichilab.UnityRoomTweet.Tweet(gameID, $"少女は【{GameController.DeckCount}枚】、大切なものを残せた。\n#unityroom #代償少女 #unity1week");
                break;
            case 1://ゲームオーバー
                naichilab.UnityRoomTweet.Tweet(gameID, $"【Game Over】\n{text.text}…\n#unityroom #代償少女 #unity1week");
                break;
            case 2:
                naichilab.UnityRoomTweet.Tweet(gameID, $"#代償少女 を遊んだ。\n#unityroom #unity1week");
                break;
        }
    }

    public void URL(int i)
    {
        switch(i)
        {
            case 0:
                Application.OpenURL("https://x.com/AIDunity");//""の中には開きたいWebページのURLを入力します
                break;
            case 1:
                Application.OpenURL("https://x.com/beniimo_game");//""の中には開きたいWebページのURLを入力します
                break;
            case 2:
                Application.OpenURL("https://unityroom.com/games/lostedai");//""の中には開きたいWebページのURLを入力します
                break;
        }
    }
}
