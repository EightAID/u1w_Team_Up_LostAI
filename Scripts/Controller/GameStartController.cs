using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStartController : MonoBehaviour
{
    [SerializeField] RectTransform startTextRT;
    public Image targetImage;
    public float fadeDuration = 1f;

    [SerializeField] GameObject TwitterPanel;

    [Header("クレジットUI")]
    [SerializeField] GameObject creditobj;

    // Start is called before the first frame update
    async void Start()
    {
        StartFloating();



        if (PrologController.isClear)//クリア済みなら
        {
            await UIController.Instance.AnimateTransitionAsyncFlash(0, 1, 5);
            await UniTask.Delay(1500);
            FadeIn(1);
            await UniTask.Delay(3000);
            TwitterPanel.SetActive(true);
        }
    }

    public void CloseX()
    {
        TwitterPanel.SetActive(false);
    }

    public void StartFloating()
    {
        startTextRT.DOAnchorPosY(startTextRT.anchoredPosition.y + 30, 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopFloating()
    {
        startTextRT.DOKill();
    }

    public async void StartGame()
    {
        //初期化
        StageController.StageIndex = 0;

        for(int i = 0; i < CardDataController.isCardUses.Length; i++)
        {
            CardDataController.isCardUses[i] = false;
        }

        SoundController.Instance.PlaySFX(0);
        await UIController.Instance.AnimateTransitionAsync(1, 0, 3);
        SceneController.Instance.LoadSceneIndex(3);
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeImage(0f, duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeImage(1f, duration));
    }

    IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color color = targetImage.color;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            targetImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        // 最終的な値をしっかりセット
        color.a = endAlpha;
        targetImage.color = color;
    }

    public void SetCredit(bool iscredit)
    {
        creditobj.SetActive(iscredit);
    }
}
