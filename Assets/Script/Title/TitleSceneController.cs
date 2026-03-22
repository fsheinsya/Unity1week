using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;
using System;
using System.Threading;

public class TitleSceneController : MonoBehaviour
{
    [Header("使用UI")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI click;

    [Header("SE")]
    [SerializeField] private AudioClip s;

    float waitTime = 1f;

    // 連打防止
    private bool isTransitioning = false;

    // キャンセル管理（超重要）
    private CancellationTokenSource cts;

    void Start()
    {
        cts = new CancellationTokenSource();

        title.alpha = 0f;
        click.alpha = 0f;

        TitleUIAnim(cts.Token).Forget();
    }

    void OnDestroy()
    {
        // シーン破棄時に全部止める
        cts.Cancel();
        cts.Dispose();

        DOTween.KillAll(); // Tween強制停止（重要）
    }

    void Update()
    {
        if (isTransitioning) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            OnClickStartGame().Forget();
        }
    }

    public async UniTask OnClickStartGame()
    {
        isTransitioning = true;

        //  クリックSE再生
        if (s != null)
        {
            AudioSource.PlayClipAtPoint(s, Camera.main.transform.position);
        }

        // 点滅停止
        click.DOKill();

        // フェードアウト
        await title.DOFade(0f, 0.5f).AsyncWaitForCompletion();
        await click.DOFade(0f, 0.5f).AsyncWaitForCompletion();

        // GameSession安全チェック
        if (GameSession.Instance != null)
        {
            GameSession.Instance.StartNewGame();
        }
        else
        {
            Debug.LogWarning("GameSessionが存在しません");
        }

        SceneManager.LoadScene(SceneNames.Bet);
    }

    public async UniTask TitleUIAnim(CancellationToken token)
    {
        try
        {
            // タイトルフェードイン
            await title.DOFade(1f, 1f)
                        .AsyncWaitForCompletion();


            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

            // クリック点滅開始
            click.alpha = 1f;

            click.DOFade(0.3f, 0.8f)
                .SetLoops(-1, LoopType.Yoyo);
        }
        catch (OperationCanceledException)
        {
            // シーン遷移時にここに来る（正常）
        }
    }
}