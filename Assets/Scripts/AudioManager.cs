using System;
using UnityEngine;

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

namespace AudioManager
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;                                             // インスタンス
        public static T Instance                                                // インスタンスを外部から参照する用(getter)
        {
            get {
                if (_instance == null)                                          //インスタンスがまだ作られていない
                {
                    Type t = typeof(T);                                         // シーン内からインスタンスを取得
                    _instance = (T)FindObjectOfType(t);
                    if (_instance == null) Debug.LogError(t + "is Not Found");  // シーン内に存在しない場合はエラー
                }
                return _instance;
            }
        }

        virtual protected void Awake(){CheckInstance();}
        protected bool CheckInstance()
        {
            if (_instance == null)
            {
                _instance = this as T;
                return true;
            }
            else if (_instance == this)
            {
                return true;
            }
            Destroy(this);
            return false;
        }
    }
}