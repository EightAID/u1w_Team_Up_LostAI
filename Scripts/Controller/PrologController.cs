using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

//プロローグエピローグ管理
public class PrologController : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TextMeshProUGUI text;

    [TextArea(3, 10)] public string prologStr;

    [Header("次に進むための処理")]
    [SerializeField] TextMeshProUGUI ClickNextText;
    [SerializeField] GameObject ClickObj;

    Coroutine typingCoroutine;

    public static bool isClear = false;

    int isAllint = 0;

    // Start is called before the first frame update
    async void Start()
    {
        UIController.Instance.RenderText(text, "");
        await UIController.Instance.AnimateTransitionAsync(0, 1, 2);
        SoundController.Instance.PlayBGM(2);
        await UniTask.Delay(500);

        TypeText(text, prologStr,0.1f);
    }


    public void TypeText(TextMeshProUGUI renderText, string text, float typeSpeed = 0.05f)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeTextCoro(renderText, text, typeSpeed));
        isAllint = 1;
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
        //クリック処理を表示
        isAllint = 2;
    }

    async private void Update()
    {
        if(Input.GetMouseButtonDown(0) && isAllint == 1)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            UIController.Instance.RenderText(text, prologStr);
            isAllint = 2;
        }

        if (isAllint == 2)
        {
            isAllint = 3;
            ClickObj.SetActive(true);
            await FadeInTextAsync(1);
        }
    }

    public async UniTask FadeInTextAsync(float duration)
    {
        Color color = ClickNextText.color;
        color.a = 0f;
        ClickNextText.color = color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = t;
            ClickNextText.color = color;
            await UniTask.Yield(); // 次のフレームまで待機
        }

        // 最後にしっかり1.0にする
        color.a = 1f;
        ClickNextText.color = color;
    }

    public async void ClickNext()
    {
        SoundController.Instance.PlaySFX(1);
        await UIController.Instance.AnimateTransitionAsync(1, 0, 2);
        SceneController.Instance.LoadSceneIndex(1);
    }
}
