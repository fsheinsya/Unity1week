using UnityEngine;

/// <summary>
/// シーンを跨いでもBGMを再生し続けるクラス
/// シングルトン + DontDestroyOnLoad を使用
/// </summary>
public class BGMManager : MonoBehaviour
{
    //========================
    // シングルトン
    //========================
    public static BGMManager Instance;

    //========================
    // BGM用AudioSource
    //========================
    private AudioSource audioSource;

    //========================
    // 初期化
    //========================
    private void Awake()
    {
        // すでに存在している場合は破棄
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 自分をインスタンスに登録
        Instance = this;

        // シーンが変わっても破棄されない
        DontDestroyOnLoad(gameObject);

        // AudioSource取得（なければ追加）
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ループ設定
        audioSource.loop = true;
    }

    //========================
    // BGM再生
    //========================
    public void PlayBGM(AudioClip clip)
    {
        // 同じBGMなら再生しない（無駄防止）
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    //========================
    // BGM停止
    //========================
    public void StopBGM()
    {
        audioSource.Stop();
    }

    //========================
    // 音量設定
    //========================
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}