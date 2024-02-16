using System.Collections;
using System.Collections.Generic;
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

public class Resource
{

    [SerializeField] private static AudioClip[] p_AudioClipList;  // AudioClip配列
    /// <summary>
    /// AudioSourceコンポーネント変数 
    /// </summary>
    private static AudioSource p_AudioSource;
    private static GameObject[] p_PrefabList;

    static Resource()
    {
             p_AudioClipList = new AudioClip[]
        {
            // Resourcesフォルダからメディア読込
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs00"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs01"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs02"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs03"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs04"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs05")
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs06")
        };

        p_PrefabList = new GameObject[]
        {
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 1"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 2"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 3"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 4"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 5")
            // // 任意のフォルダからPrefab読込
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 1/Prefab/Rocket 1.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 2/Prefab/Prefab 2.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 3/Prefab/Prefab 3.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 4/Prefab/Prefab 4.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 5/Prefab/Prefab 5.prefab")
        };
    }
}
