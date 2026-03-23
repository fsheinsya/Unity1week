using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public abstract class BattleCharacterView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected RectTransform rect;
    [SerializeField] protected Image image;

    [Header("SE（任意）")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip attackSE;
    [SerializeField] protected AudioClip hitSE;
    [SerializeField] protected AudioClip healSE;
    [SerializeField] protected AudioClip buffSE;

    protected Vector2 originalPos;
    protected Color originalColor;

    protected virtual void Awake()
    {
        originalPos = rect.anchoredPosition;
        originalColor = image.color;
    }

    protected void PlaySE(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // =========================
    // 抽象メソッド
    // =========================
    public abstract UniTask PlayAttack();

    // =========================
    // 共通処理
    // =========================
    public virtual async UniTask PlayHit()
    {
        PlaySE(hitSE);
        await rect.DOShakeAnchorPos(0.2f, 20f).AsyncWaitForCompletion();
    }

    public virtual async UniTask PlayHeal()
    {
        PlaySE(healSE);
        await image.DOColor(Color.lightSkyBlue, 0.2f).AsyncWaitForCompletion();
        await image.DOColor(originalColor, 0.3f).AsyncWaitForCompletion();
    }

    public virtual async UniTask PlayBuff()
    {
        PlaySE(buffSE);
        await image.DOColor(Color.orange, 0.2f).AsyncWaitForCompletion();
        await image.DOColor(originalColor, 0.3f).AsyncWaitForCompletion();
    }

    public virtual void ApplyPoison()
    {
        image.DOColor(Color.purple, 0.3f);
    }
}