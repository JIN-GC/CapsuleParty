using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

public class Capsules : MonoBehaviour
{
    /// <summary>
    /// Capsules Prefab 配列
    /// </summary>
    [SerializeField] private GameObject[] _prefabList;
    /// <summary>
    /// Capsules Prefab 生成座標配列
    /// </summary>
    [SerializeField] private Vector3[] _vectorList;
    private static int _prefabsCnt = 0;

    /// <summary>
    /// 終了判定カウント Setter関数(CapsuleCoverクラス呼出し用)
    /// </summary>
    public static void SetPrefabsCheckCnt()
    {
        _prefabsCnt++;
    }
    private void Awake()
    {
        CapsulePrefabList();
        CapsuleInstantiate();
    }
    private void Start()
    {
        StartCoroutine(CheckStatus());
    }

    /// <summary>
    /// 終了判定カウント＆シーン切替え 
    /// </summary>
    private IEnumerator CheckStatus()
    {
        int chkCnt = 0;
        yield return new WaitForSeconds(30.0f);
        while (_prefabsCnt < _prefabList.Length)
        {
            chkCnt = 0; // リセット
            for (int i = 0; i < _prefabList.Length; i++)
            {
                if (!gameObject.transform.GetChild(i).gameObject.activeSelf)
                {
                    chkCnt++;
                    // Debug.Log($"[@ CheckStatusD] [chkCnt]: {chkCnt}, [_prefabsCnt]: {_prefabsCnt} [_prefabList.Length]: {_prefabList.Length} [gameObject.tag]: {gameObject.transform.GetChild(i).gameObject.tag.ToString()}");
                }
            } 
            yield return new WaitForSeconds(10.0f);
        }
        // Debug.Log($"[@ END] [chkCnt]: {chkCnt} [_prefabsCnt]: {_prefabsCnt}");
        SceneManager.LoadScene("End");
    }

    /// <summary>
    /// CapsulePrefab設定
    /// </summary>
    private void CapsulePrefabList()
    {
        _prefabList = new GameObject[]
        {
            // Resourcesフォルダから読み込む方法
            // (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Case"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Goryokaku"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Louve"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Sakura"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_ArcTriomphe"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_PyramidAndCamel"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_StoneHenge"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_TokyoTower")
            // 任意のフォルダから読み込む方法
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Case.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Goryokaku.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Louve.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Sakura.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_ArcTriomphe.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_PyramidAndCamel.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_StoneHenge.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_TokyoTower.prefab")
        };
        _vectorList = new Vector3[]                                 // _prefabList生成座標配列設定
            {
                // new Vector3(30, 700, 0),
                new Vector3(-30, 700, 10),
                new Vector3(10, 800, -30),
                new Vector3(0, 900, 30),
                new Vector3(-20, 900, -30),
                new Vector3(30, 950, -10),
                new Vector3(-30, 950, 20),
                new Vector3(10, 1000, -20)
            };
    }
    /// <summary>
    /// CapsulePrefab生成
    /// </summary>
    private void CapsuleInstantiate()
    {   
        GameObject prefabTemp;                                      // _prefabList配列シャッフル用変数
        Vector3 vectorTemp;                                         // _vectorList配列シャッフル用変数
        int randNum;                                                // シャッフル用ランダム変数
        for (int i=0; i < _prefabList.Length; i++)
        {
            randNum = UnityEngine.Random.Range(0, i+1);             // Fisher-Yatesシャッフルアルゴリズム
            prefabTemp = _prefabList[i];                            // _prefabList配列内順序シャッフル
            _prefabList[i] = _prefabList[randNum];
            _prefabList[randNum] = prefabTemp;

            vectorTemp = _vectorList[randNum];                      // _vectorList配列内順序シャッフル
            _vectorList[randNum] = _vectorList[i];
            _vectorList[i] = vectorTemp;
        }
        randNum = UnityEngine.Random.Range(0, _prefabList.Length);  // シャッフル後にPrefab生成
        var parentTransform = this.transform;
        for (int i = 0; i < _prefabList.Length; i++) Instantiate(_prefabList[i], _vectorList[i], Quaternion.identity, parentTransform);
    }
}
