using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
// using (UnityWebRequest _www = UnityWebRequestMultimedia.GetAudioClip("https://classix.sitefactory.info/mp3classic/bizet/2464.mp3", AudioType.MPEG));
///	XMLドキュメントコメントのタグ一覧
/// <summary>e.g.summary contents
///     <example>e.g example
///     <para />linefeed CRLF
///     <typeparamref name="Type" />
///     <typeparam name="Type">: e.g. Type of the Elements</typeparam>
///     <param name="paramName" />e.g. Parameter Description
///     <paramref name="paramName" />
///     <see cref="OtherClass.OtherMethod" />e.g. Description
///     <seealso cref="OtherClass" />e.g. Description
///     <code>e.g. Description Code
/// <exception cref="ExceptionType" />Description
/// <remarks />e.g. Additional remarks or notes
/// <returns />e.g. Return Value Description
///	※ XMLドキュメントコメントはインテリセンスでヒントを表示機能のみではなく作業効率と品質向上に役立ちます。
///	※ バージョン管理(GitHub・Wiki/GitLabPages/Confluence)
///	※ API/ドキュメント(バージョン管理含む)の自動生成(DocFX/Doxygen/Sandcastle/GhostDoc(クラス図サポート))
///	※ メタデータによるコード品質解析や静的解析(DocFX/StyleCop/ReSharper/RoslynAnalyzer/SonarQube)
///	※ テストケースの自動生成(DocFX/Pex&Moles/AutoFixture)、コード カバレッジ(UnityTestFramework/UnityCoverlet/dotCover)

[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    /// <summary>
    /// AudioSource設定
    /// </summary>
    private AudioSource _audios;
    /// <summary>
    /// MIMEタイプ変数
    /// </summary>
    [SerializeField] private AudioType _mimeType;
    /// <summary>
    /// URLリスト配列
    /// </summary>
    [SerializeField] private string[] _urlList;
    /// <summary>
    /// 再生時間表示
    /// </summary>
    [SerializeField] private Text _timeText;
    /// <summary>
    /// 旧式 System.ObsoleteAttribute 利用
    [Obsolete]

    private void Awake()
    {   
        /// <summary>
        /// [記述補足] 旧式 Connect()
        /// [取扱注意] 将来サポート対象外の可能性が有り。CS0612 here: method is deprecated /W:1 
        /// [暫定処置] クイックフィックスで追加[廃止]を選択
        /// </summary>
        StartCoroutine(Connect());
    }

    [Obsolete]                                                          // 旧式 System.ObsoleteAttribute 利用
    private IEnumerator Connect()
    {
        _urlList = new String[]                                         // WEBサイトよりメディアをロード（サポート対象外＆時間都合によりMP3形式のみ動作確認済み）
        {   
            /// <summary>
            /// [取扱注意] 著作権は各サイト各自確認後に利用ください。
            /// https://morexlusive.com/camila-cabello-dont-go-yet-3/
            /// "https://kamatamago.com/sozai/classic/c00001-c00100/C00016_kamatamago_Prelude-to-Act-1(Bullfighter).mp3"
            /// https://classix.sitefactory.info/downmp3.html   
            /// </summary>
            "https://www1.morexlusive.com/wp-content/uploads/2021/10/Camila_Cabello_-_Dont_Go_Yet.mp3",
            "https://classix.sitefactory.info/mp3classic/bizet/2464.mp3",
        };
        // string _url = string.Join("", _urlList);                     // 個別抽出
        int _randNum = UnityEngine.Random.Range(0, _urlList.Length);    // 配列indxのランダム指定

        /// <summary>
        /// [記述補足] 旧式 WWW
        /// [取扱注意] 廃止されサポート外です。 CS0618.cs compile with: /W:2 
        /// [暫定処置]:クイックフィックスで追加[廃止]を選択
        /// [取扱注意] 廃止されサポート外です。 廃止されている為、'new' 式を簡素化できますIDE0090 /W:2 
        /// [暫定処置] クイックフィックスで追加[廃止]を選択
        /// </summary>
        WWW _www = new WWW(_urlList[_randNum]);                         // 簡易WEBアクセス(配列index内のURL参照
        if (_www.error != null)
        {
            yield return _www;   // wwwイベント待機
            _audios = GetComponent<AudioSource>();                      // AudioSourceコンポーネント設定
            _audios.clip = _www.GetAudioClip(false, true, _mimeType);   // AudioClipへのメディア設定 // 第２引数はtrueで読込中の再生可能
            _audios.Play();                                             // AudioClip再生
            DontDestroyOnLoad(_audios);                                 // Scene遷移時の音楽再生継続処理
            _timeText.text = $"{(int)_audios.time / 60:D2}:{(int)this._audios.time % 60:D2}";  // 現在再生時間表示

        }
        else
        {
            Debug.LogError($"Failed to load audio data: {_www.error}");
        }
    }
}
