using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//ステージ関連の情報を扱う
public class StageController : MonoBehaviour
{
    public static StageController Instance { get;private set; }

    [SerializeField] public string[] StageName;

    [SerializeField] public string[] StageSerifStart1;
    [SerializeField] public string[] StageSerifStart2;
    [SerializeField] public string[] StageSerifStart3;
    [SerializeField] public string[] StageSerifStart4;
    [SerializeField] public string[] StageSerifStart5;

    [Header("知性無し")]
    [SerializeField] public string[] StageSerifStart1T;
    [SerializeField] public string[] StageSerifStart2T;
    [SerializeField] public string[] StageSerifStart3T;
    [SerializeField] public string[] StageSerifStart4T;
    [SerializeField] public string[] StageSerifStart5T;

    [SerializeField] public static int StageIndex = 0;

    private Coroutine typingCoroutine;

    [TextArea] public string[] Tyutorial;

    // Start is called before the first frame update
    void Awake()
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
    /// ゲーム開始時に挟まる会話シーン
    /// </summary>
    /// <returns></returns>
    async public UniTask StartSerif()
    {
        string[] stageText = StageSerifStart1;

        if (CardDataController.isCardUses[23])//知性なら
        {
            switch (StageIndex)
            {
                case 0:
                    stageText = StageSerifStart1;
                    break;
                case 1:
                    stageText = StageSerifStart2T;
                    break;
                case 2:
                    stageText = StageSerifStart3T;
                    break;
                case 3:
                    stageText = StageSerifStart4T;
                    break;
                case 4:
                    stageText = StageSerifStart5T;
                    break;
            }
        }
        else
        {
            switch (StageIndex)
            {
                case 0:
                    stageText = StageSerifStart1;
                    break;
                case 1:
                    stageText = StageSerifStart2;
                    break;
                case 2:
                    stageText = StageSerifStart3;
                    break;
                case 3:
                    stageText = StageSerifStart4;
                    break;
                case 4:
                    stageText = StageSerifStart5;
                    break;
            }
        }


        for (int i = 0; i < stageText.Length; i++)
        {
            UIController.Instance.OpenSerif(stageText[i]);
            await GameController.Instance.WaitForClickOrTimeout();
        }
        UIController.Instance.CloseSerif();
    }

    async public UniTask Thutorial()
    {
        for (int i = 0; i < Tyutorial.Length; i++)
        {
            UIController.Instance.OpenSerif(Tyutorial[i]);
            if (i == 3) FaceView.Instance.SetFaceView(FaceView.FaceState.ASERI);
            if (i == 7) FaceView.Instance.SetFaceView(FaceView.FaceState.EGAO,FaceView.ArmState.UPUP);

            await GameController.Instance.WaitForClickOrTimeout();
        }
        UIController.Instance.CloseSerif();
    }

    public void TypeText(TextMeshProUGUI renderText, string text, float typeSpeed = 0.05f)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeTextCoro(renderText, text, typeSpeed));
    }

    private IEnumerator TypeTextCoro(TextMeshProUGUI renderText, string text, float typeSpeed = 0.05f)
    {
        UIController.Instance.RenderText(renderText, "");
        Debug.Log(text);
        for (int i = 0; i < text.Length; i++)
        {
            renderText.text += text[i];
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    async public UniTask LastSerif()
    {
        await SerifOpen("…やっといなくなった");


        if (CardDataController.isCardUses[34])//トラウマ消費
        {
            await SerifOpen("…なんだったんだろう、一体…");
            await SerifOpen("まあいいや");
            await SerifOpen("嫌だったこと、色々あった気がするけど");
            await SerifOpen("すっかり綺麗に忘れちゃった…");
        }
        else
        {
            await SerifOpen("あの人のせいでわたしは…");
            await SerifOpen("はぁ…");
            await SerifOpen("色々、思い出しちゃった…");
            await SerifOpen("思い出したくなかったなぁ…");
        }

        await SerifOpenLast("――アイ");
        await SerifOpen("…？");
        await SerifOpenLast("――アイ");
        await SerifOpen("どこから聞こえてるの？");
        if (CardDataController.isCardUses[3])//聴覚消えたら
        {
            await SerifOpen("耳はもう、失ってしまったのに");
            await SerifOpen("なんで、聞こえるんだろう…");
        }
        if (CardDataController.isCardUses[2])//視覚消えたら
        {
            await SerifOpen("見えてないだけで、目の前に誰かいるの？");
        }
        await SerifOpenLast("――アイ");
        await SerifOpen("…！だれ！？");
        await SerifOpenLast("――アイ、起きて…お願い、目を覚まして…");

        if (CardDataController.isCardUses[5])//名前切ってたら
        {
            await SerifOpen("アイ…？");
            await SerifOpen("誰かの名前かな？");
            await SerifOpen("誰を呼んでいるんだろう");
        }
        else
        {
            await SerifOpen("アイ…？");
            await SerifOpen("わたしのことを呼んでる！？");
        }

        await SerifOpen("この声、なんだか暖かい…");


        if (CardDataController.isCardUses[18])//両親切ったら
        {
            await SerifOpen("でも、記憶にない声だ…");
            await SerifOpen("暖かいのに、覚えてない…！！");
            await SerifOpen("あれ…？");
            await SerifOpen("なんで……");
            await SerifOpen("忘れちゃダメなはずなのに…");
            await SerifOpen("この声だけは、忘れたくないはずなのに…");
            await SerifOpen("(誰なのかは、全く思い出せない)");
            await SerifOpen("(なんでなの…)");
            await SerifOpen("(思い…出せないよ…)");
            await SerifOpen("(……)");

            await SerifOpen("わたしを呼んでる…行かなきゃ…");
        }
        else
        {
            await SerifOpen("もしかして、ママとパパ…？");
            await SerifOpenLast("――アイ、お願い…");
            await SerifOpenLast("お願い…起きてくれ…");
            await SerifOpen("わたし、ずっと寝てたんだ…");
            await SerifOpen("そっか、そろそろ起きないと");
            await SerifOpen("二人が、わたしを呼んでる…");
            await SerifOpen("帰らなきゃ…");
            await SerifOpen("わたしを、愛してくれる人たちの場所に");
        }

        Debug.Log($"デッキ数：{CardDataController.Instance.GetCardList(CardDataController.DeckState.DECK).Count}");
        if(CardDataController.Instance.GetCardList(CardDataController.DeckState.DECK).Count  <= 1)
        {
            await SerifOpen("……");
            await SerifOpen("あれ…");
            await SerifOpen("力が…");
        }
    }

    async public UniTask LastSerif2()
    {
        await SerifOpenLast2("……");

        if (CardDataController.Instance.GetCardList(CardDataController.DeckState.DECK).Count <= 1)
        {
            await UIController.Instance.AnimateTransitionAsyncLost(1, 0, 4);
            await SerifOpenLast2("もう、カードはこれだけ…");
            await SerifOpenLast2("わたしは、だれなのか");
            await SerifOpenLast2("どこからきたのか");
            await SerifOpenLast2("なにが、大切だったのか");
            await SerifOpenLast2("目も見えない");
            await SerifOpenLast2("音も聞こえない");
            await SerifOpenLast2("暗闇のなか");
            await SerifOpenLast2("なのに…");
            await SerifOpenLast2("何も感じない…");
            await SerifOpenLast2("もうなにも、思い出せない…");
            await SerifOpenLast2("わたしは…");
            await SerifOpenLast2("なんなんだろう");
            await SerifOpenLast2("考えることすら、今は難しい");
            await SerifOpenLast2("わたしの中は、すでにからっぽ");
            await SerifOpenLast2("これ以上、いったい何を失えるの？");
            await SerifOpenLast2("このカードは、いったいわたしからなにを奪うのだろう");

            await SerifOpenLast2("……");
            await SerifOpenLast2("愛のカードかぁ…");
            await SerifOpenLast2("あい…");
            await SerifOpenLast2("あい…");
            await SerifOpenLast2("あい…");
            await SerifOpenLast2("……");
            await SerifOpenLast2("疲れちゃったな…");
            await SerifOpenLast2("でも…");
            await SerifOpenLast2("きっと…");
            await SerifOpenLast2("きっと、これだけは…");
            await SerifOpenLast2("払ってはいけない。");

            await SerifOpenLast2("fin");
            await SerifOpenLast2("【4.愛だけは失わないで】");

            await UniTask.Delay(2200);
            await UIController.Instance.AnimateTransitionAsyncLost(0, 1, 4);
            UIController.Instance.RenderText(UIController.Instance.think2Text, "");
            GameController.Instance.nowState = GameController.GameState.LASTITIRAN;
            UIController.Instance.LastResultTransiton();
            return;
        }

        if (!CardDataController.isCardUses[18] && !CardDataController.isCardUses[29] && CardDataController.isCardUses[34])//両親切ってないかつ希望切ってないかつトラウマ消費
        {
            Debug.Log("特殊エンド");
            await SerifOpenLast2("……そっか");
            await SerifOpenLast2("わたし、今まで夢を見てたんだ");
            await SerifOpenLast2("とてもとても、怖い夢");
            await SerifOpenLast2("でも、もう怖くない");
            await SerifOpenLast2("愛も、希望も、忘れてない");
            await SerifOpenLast2("パパとママも、わたしを待ってる");
            if (!CardDataController.isCardUses[21])//妹を使っていなければ
            {
                await SerifOpenLast2("カナちゃんの顔も早く見たいな");
            }
            if (!CardDataController.isCardUses[19])//ペットを使っていなければ
            {
                await SerifOpenLast2("ミーちゃんをたくさん撫でたい");
            }
            //大切な人のことを全員覚えていたら
            if (!CardDataController.isCardUses[18] && !CardDataController.isCardUses[21] && !CardDataController.isCardUses[19])
            {
                await SerifOpenLast2("はやく、みんなに会いたい");
            }

            await UniTask.Delay(500);

            await UIController.Instance.AnimateTransitionAsyncEnding(1, 0, 4);
            await SerifOpenLast2("……");
            await SerifOpenLast2("バイバイ、黒いお化けさんたち");
            await SerifOpenLast2("わたし…");
            await SerifOpenLast2("わたし…");
            await SerifOpenLast2("生きるんだ、絶対に");
            await SerifOpenLast2("もうなにも、失いたくはないから");

            await SerifOpenLast2("fin");

            await SerifOpenLast2("【3.希望があれば、きっと】");

            await UniTask.Delay(2200);
            UIController.Instance.RenderText(UIController.Instance.think2Text, "");
            await UIController.Instance.AnimateTransitionAsyncEnding(0, 1, 3);
            GameController.Instance.nowState = GameController.GameState.LASTITIRAN;
            UIController.Instance.LastResultTransiton();
            return;
        }


        if (CardDataController.isCardUses[18])//両親切ったら
        {
            await SerifOpenLast2("ここは…？");
            await SerifOpenLast2("(知らない人が二人)");
            await SerifOpenLast2("あなたたちは…");
            await SerifOpenLast2("…だれですか？");

            await SerifOpenLast2("fin");
            await SerifOpenLast2("【1.知らない二人】");
        }
        else
        {
            await SerifOpenLast2("パパ、ママ。");
            if (!CardDataController.isCardUses[21])//妹を使っていなければ
            {
                await SerifOpenLast2("カナちゃんも…");
            }

            //大切な人のことを全員覚えていたら
            if (!CardDataController.isCardUses[18] && !CardDataController.isCardUses[21] && !CardDataController.isCardUses[19])
            {
                await SerifOpenLast2("(良かった…)");
                await SerifOpenLast2("(みんなのこと、忘れなかったよ)");
            }
            await SerifOpenLast2("おはよう");
            await SerifOpenLast2("fin");
            await SerifOpenLast2("【2.愛してくれる人】");
        }

        await UniTask.Delay(1000);
        UIController.Instance.RenderText(UIController.Instance.think2Text, "");
        GameController.Instance.nowState = GameController.GameState.LASTITIRAN;
        UIController.Instance.LastResultTransiton();
    }

    async UniTask SerifOpen(string text,string TiseiText = "")
    {
        UIController.Instance.OpenSerif(text, TiseiText);
        await GameController.Instance.WaitForClickOrTimeout();
        UIController.Instance.CloseSerif();
    }

    async UniTask SerifOpenLast(string text)
    {
        UIController.Instance.TypeText(UIController.Instance.thinkText, text,0.1f);
        await GameController.Instance.WaitForClickOrTimeout();
        UIController.Instance.thinkText.text = "";
    }

    async UniTask SerifOpenLast2(string text)
    {
        UIController.Instance.TypeText(UIController.Instance.think2Text, text, 0.1f);
        await GameController.Instance.WaitForClickOrTimeout();
        UIController.Instance.thinkText.text = "";
    }
}
