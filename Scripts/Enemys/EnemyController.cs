using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("敵")]
    [SerializeField] Transform enemy_rt;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] enemySprite;
    [SerializeField] RectTransform[] heartposRT;
    private Vector2 originalPos;

    [Header("表情差分")]
    [SerializeField] Sprite[] enemy_sprite;

    [Header("UI")]
    [SerializeField] RectTransform HeartRT;
    [SerializeField] Image HeartImg;
    [SerializeField] Image HeartImgHide;
    [SerializeField] TextMeshProUGUI lifeText;

    public int enemyHaert = 20;

    public float duration = 2f;         // 消えるまでの時間
    public float shakeAmount = 0.05f;   // 揺れの強さ


    private Vector3 originalPosition;
    private CancellationTokenSource cts;

    // Start is called before the first frame update
    void Start()
    {
        UIController.Instance.RenderText(lifeText, enemyHaert.ToString());
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ダメージを受けたときの関数
    /// </summary>
    public void Damage(int damage)
    {
        SetDamageText(damage);
        enemyHaert = Mathf.Max(0, enemyHaert - damage);
        //敵を揺らしてる
        SoundController.Instance.PlaySFX(5);
        UIController.Instance.CameraShake();
        if(GameController.Instance.isSmaile)
        {
            GameController.Instance.isSmaile = false;
        }

    }

    public void Health(int health)
    {
        SetHealthText(health);
        SoundController.Instance.PlaySFX(9);
        enemyHaert = Mathf.Max(0, enemyHaert + health);
    }

    public void SetDamageText(int damage)
    {
        UIController.Instance.TakeDamegeView(lifeText, enemyHaert, damage, 0.5f);
    }

    public void SetHealthText(int health)
    {
        UIController.Instance.TakeDamegeView(lifeText, enemyHaert, -health, 0.5f,true);
    }


    public void ResetEnemy()
    {
        enemyHaert = 20;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        HeartImg.color = new Color(1, 1, 1, 1);
        HeartImgHide.color = new Color(1, 1, 1, 1);
        lifeText.color = new Color(1, 1, 1, 1);
        lifeText.gameObject.SetActive(true);

        spriteRenderer.sprite = enemySprite[StageController.StageIndex];
        HeartRT.anchoredPosition = heartposRT[StageController.StageIndex].anchoredPosition;

        UIController.Instance.RenderText(lifeText, enemyHaert.ToString());
    }

    public async UniTask StartDisappear()
    {
        cts = new CancellationTokenSource();
        await DisappearRoutine(cts.Token);
    }

    private async UniTask DisappearRoutine(CancellationToken token)
    {
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;
        lifeText.gameObject.SetActive(false);

        while (elapsed < duration)
        {
            if (token.IsCancellationRequested) return;

            // ランダムに小刻みに揺らす
            float offsetX = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            float offsetY = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

            // 徐々に透明にする
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            HeartImg.color = spriteRenderer.color;
            HeartImgHide.color = spriteRenderer.color;
            lifeText.color = spriteRenderer.color;

            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        // 最後に完全透明＆位置リセットして削除
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        HeartImg.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        HeartImgHide.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        lifeText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        transform.position = originalPosition;
    }

    public void EndAttack()
    {
        if (GameController.Instance.isBaria)
        {
            GameController.Instance.isBaria = false;
            return;
        }
        int seed = DateTime.Now.Millisecond;
        UnityEngine.Random.InitState(seed);

        if (UnityEngine.Random.Range(0, 100) >= 90)
        {
            GameController.Instance.enemyController.Health(1 + StageController.StageIndex);
        }
        else
        {
            GameController.Instance.playerController.Damage(2 + StageController.StageIndex);
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
