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
        UIController.Instance.RenderText(KyouyuuText, "共有する");
        UIController.Instance.RenderText(text[0],"スタートへ戻る");
        UIController.Instance.RenderText(text[1],"現在のステージからやり直す");

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
            UIController.Instance.TypeText(GameOverSerifText, "一番大切なものを、\n失ってしまった");
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
                    UIController.Instance.TypeText(GameOverSerifText, "一番大切なものを、\n失ってしまった");
                }
                else//デフォルト
                {
                    UIController.Instance.TypeText(GameOverSerifText, "失う覚悟、\nそれが足りなかっただけ");
                }
                break;
            case 1:
                UIController.Instance.TypeText(GameOverSerifText, "言語は、\n意味があるから読めるんだ");
                break;
            case 2:
                UIController.Instance.TypeText(GameOverSerifText, "何も見えない。\n普段から、目に頼りすぎてたかも");
                break;
            case 3:
                UIController.Instance.TypeText(GameOverSerifText, "何も聞こえない、\n大切な人が呼ぶ声すらも");
                break;
            case 4:
                UIController.Instance.TypeText(GameOverSerifText, "笑顔が素敵！\n昔言われたことを思い出す");
                break;
            case 5:
                UIController.Instance.TypeText(GameOverSerifText, "もう誰も、\nわたしのことを呼んではくれない");
                break;
            case 6:
                UIController.Instance.TypeText(GameOverSerifText, "キッチンから漂ういい匂い、\n脳裏にこびり付いている");
                break;
            case 7:
                UIController.Instance.TypeText(GameOverSerifText, "痛みがなくても、\n感じていないだけ");
                break;
            case 8:
                UIController.Instance.TypeText(GameOverSerifText, "どんな食べ物も、\nただの物に見える…");
                break;
            case 9:
                UIController.Instance.TypeText(GameOverSerifText, "隙間から漏れる\n朝の陽ざしが好きだった");
                break;
            case 10:
                UIController.Instance.TypeText(GameOverSerifText, "まだそれがなにかも分からないまま、\nわたしは…");
                break;
            case 11:
                UIController.Instance.TypeText(GameOverSerifText, "考えることを、\nただあきらめた");
                break;
            case 12:
                UIController.Instance.TypeText(GameOverSerifText, "これがないと、\nなにもできない");
                break;
            case 13:
                UIController.Instance.TypeText(GameOverSerifText, "これじゃあもう、\nどこへも行けない");
                break;
            case 14:
                UIController.Instance.TypeText(GameOverSerifText, "わたしの好きな色、\nどんな色だったっけ");
                break;
            case 15:
                UIController.Instance.TypeText(GameOverSerifText, "人が1番最初に\n忘れちゃうのは声らしい");
                break;
            case 16:
                UIController.Instance.TypeText(GameOverSerifText, "大人になれないまま、\n終わっちゃうのかな");
                break;
            case 17:
                UIController.Instance.TypeText(GameOverSerifText, "大好きなお誕生日パーティ、\nもうできないな");
                break;
            case 18:
                UIController.Instance.TypeText(GameOverSerifText, "わたしって、\n誰の子供なのかな");
                break;
            case 19:
                UIController.Instance.TypeText(GameOverSerifText, "いた気がするの、\nふわふわで、可愛い、あの…");
                break;
            case 20:
                UIController.Instance.TypeText(GameOverSerifText, "もうこれで夜は、\n怖くなくなるのかな");
                break;
            case 21:
                UIController.Instance.TypeText(GameOverSerifText, "毎日一緒に遊んでたのに、\nなんだか遠い昔のことみたい");
                break;
            case 22:
                UIController.Instance.TypeText(GameOverSerifText, "もう何も怖くないのに、\nどうして");
                break;
            case 23:
                UIController.Instance.TypeText(GameOverSerifText, "わたし、お勉強ができる\nえらい子だったのに");
                break;
            case 24:
                UIController.Instance.TypeText(GameOverSerifText, "わたしには足りなかったから、\nこうなっちゃったんだ");
                break;
            case 25:
                UIController.Instance.TypeText(GameOverSerifText, "もう少し運動ができれば、\nこうはならなかったのかな");
                break;
            case 26:
                UIController.Instance.TypeText(GameOverSerifText, "わたしがこうなるのも、\n仕方がなかったんだ");
                break;
            case 27:
                UIController.Instance.TypeText(GameOverSerifText, "こんな目に遭っているのに、\nどうして悲しくないんだろう…");
                break;
            case 28:
                UIController.Instance.TypeText(GameOverSerifText, "あなたは優しい子って\n言われてたのに");
                break;
            case 29:
                UIController.Instance.TypeText(GameOverSerifText, "生きて帰りたかった、\nただそれだけなのに");
                break;
            case 30:
                UIController.Instance.TypeText(GameOverSerifText, "わたし、やっぱり\nこうなる運命だったんだ");
                break;
            case 31:
                UIController.Instance.TypeText(GameOverSerifText, "もう二度と、\n可愛いねって褒めてもらえないのかな");
                break;
            case 33:
                UIController.Instance.TypeText(GameOverSerifText, "クッキーって、\n本当に美味しかったのかな");
                break;
            case 34:
                UIController.Instance.TypeText(GameOverSerifText, "あの人のせいで、\nわたしは…");
                break;
            case 32://呼吸
                UIController.Instance.TypeText(GameOverSerifText, "苦しい…苦しい…\n意識が遠のいていく…");
                break;
            default:
                UIController.Instance.TypeText(GameOverSerifText, "帰りたい、\nただそれだけなのに");
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
