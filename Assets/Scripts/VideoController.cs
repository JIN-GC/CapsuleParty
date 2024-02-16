using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
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

[RequireComponent(typeof(RenderTexture))]
public class VideoController : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] Button _playButton, _stopButton, _pauseButton;
    [SerializeField] Slider _slider;
    [SerializeField] Text _timeText;

    // [ref] https://docs.unity3d.com/ja/2018.4/Manual/class-VideoPlayer.html
    // [ref] https://qiita.com/broken55/items/f235497c7b4b2a46b926

    void Start()
    {
        _playButton.onClick.AddListener(() => _videoPlayer.Play());                       // 再生/一時停解除再生
        _stopButton.onClick.AddListener(() => _videoPlayer.Stop());                       // 停止
        _pauseButton.onClick.AddListener(() => _videoPlayer.Pause());                     // 一時停止
        _slider.onValueChanged.AddListener(value => _videoPlayer.time = _videoPlayer.length * value); //スライダーの場所に合わせて再生する場所を変える
    }
    void Update()
    {
        /// <summary>
        /// スライダーを再生時間に合わせて進行させる
        /// SetValueWithoutNotifyを使うとonValueChangedのコールバックが呼ばれない
        /// UIで操作したイベントだけを取得することができる
        /// </summary>
        _slider.SetValueWithoutNotify((float)(_videoPlayer.time / _videoPlayer.length));   // スライダー
        _timeText.text = $"{(int)_videoPlayer.time / 60:D2}:{(int)_videoPlayer.time % 60:D2}"; // 現在再生時間の表示
    }
}
