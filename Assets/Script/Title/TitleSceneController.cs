using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;
using System;

public class TitleSceneController : MonoBehaviour
{
    [Header("使用UI")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI click;

    float waitTime = 1f;

    // 連打防止フラグ
    private bool isTransitioning = false;

    void Start()
    {
        title.alpha = 0f;
        click.alpha = 0f;

        TitleUIAnim().Forget();
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

        await title.DOFade(0f, 0.5f).AsyncWaitForCompletion();
        await click.DOFade(0f, 0.5f).AsyncWaitForCompletion();

        GameSession.Instance.StartNewGame();
        SceneManager.LoadScene(SceneNames.Bet);
    }

    public async UniTask TitleUIAnim()
    {
        // フェードイン完了まで待つ
        await title.DOFade(1f, 1f).AsyncWaitForCompletion();

        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));

        click.alpha = 1f;
        click.DOFade(0.3f, 0.8f)
            .SetLoops(-1, LoopType.Yoyo);
    }
}