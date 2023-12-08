using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private AudioSource p_Audios; // AudioSource利用
    [SerializeField] private AudioType MimeType;  // MIMEタイプ変数
    [SerializeField] public string[] UrlList;  // URLリスト配列

    [Obsolete] // 旧式 System.ObsoleteAttribute 利用
    private void Awake()
    {   
        StartCoroutine(Connect());
        // [記述補足] 旧式 Connect()
        // [取扱注意] 将来サポート対象外の可能性が有り。CS0612 here: method is deprecated /W:1 
        // [暫定処置] クイックフィックスで追加[廃止]を選択
    }

    [Obsolete] // 旧式 System.ObsoleteAttribute 利用
    private IEnumerator Connect()
    {
        UrlList = new String[]  // WEBサイトよりメディアをロード（サポート対象外＆時間都合によりMP3形式のみ動作確認済み）
        {            
            "https://www1.morexlusive.com/wp-content/uploads/2021/10/Camila_Cabello_-_Dont_Go_Yet.mp3",
            "https://classix.sitefactory.info/mp3classic/bizet/2464.mp3"

            // [取扱注意] 著作権は各サイト各自確認後に利用ください。
            // https://morexlusive.com/camila-cabello-dont-go-yet-3/
            // "https://kamatamago.com/sozai/classic/c00001-c00100/C00016_kamatamago_Prelude-to-Act-1(Bullfighter).mp3"
            // https://classix.sitefactory.info/downmp3.html
        };
        // string url = string.Join("", UrlList);  // 個別抽出
        int randNum = UnityEngine.Random.Range(0, UrlList.Length);  // 配列indxのランダム指定
        WWW www = new WWW(UrlList[randNum]);    // 簡易WEBアクセス(配列index内のURL参照
        yield return www;   // www イベント待機
        p_Audios = GetComponent<AudioSource>();   // AudioSourceコンポーネント設定
        p_Audios.clip = www.GetAudioClip(false, true, MimeType);  // AudioClipへのメディア設定 // 第２引数はtrueで読込中の再生可能
        p_Audios.Play();  // AudioClip再生
        DontDestroyOnLoad(p_Audios);  // Scene遷移時の音楽再生継続処理

        // [記述補足] 旧式 WWW
        // [取扱注意] 廃止されサポート外です。 CS0618.cs compile with: /W:2 
        // [暫定処置]:クイックフィックスで追加[廃止]を選択
        // [取扱注意] 廃止されサポート外です。 廃止されている為、'new' 式を簡素化できますIDE0090 /W:2 
        // [暫定処置] クイックフィックスで追加[廃止]を選択
    }
}
