using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ArkPanel : MonoBehaviour
{
    /// <summary>
    /// _height: ArcPanel集合体の高さ
    /// </summary>
    [SerializeField] private float _height = 200, _yieldTime = 0.15f;
    /// <summary>
    /// _arcAngle: ArcPanel単体の弧度, 
    /// _startAngle: ArcPanel集合体の開始角度, 
    /// _divideCnt: ArcPanel集合体の分割係数,
    /// _arcQuality: 円の分割数(360度),
    /// _arkMax: ArcPanel単体の弧度分割数
    /// </summary>
    [SerializeField] private int 
    _arcAngle = 60, _startAngle = 0, _divideCnt = 3, _arcQuality = 100, _arkMax;
    /// <summary>
    /// _deg(degree): 度数法の角度, hyp(Hypotenuse): 斜辺の長さ, _adj(Adjacent): 隣辺の長さ
    /// </summary>
    private float _deg, _hyp, _adj;
    /// <summary>
    /// ArcPanel単体のRGB
    /// </summary>
    [SerializeField] private Color _color = new Color(0.75f, 1.0f, 1.0f);
    /// <summary>
    /// ArcPanel集合体のサイズ
    /// </summary>
    [SerializeField] private Vector3 _scale = new Vector3(50, 2, 50);
    /// <summary>
    /// ArcPanel集合体の頂点座標用2次元配列(ArcPanel集合体および単体の頂点座標情報)
    /// </summary>
    private List<List<Vector3>> _verticesList = new List<List<Vector3>>();
    /// <summary>
    /// ArcPanel集合体のMesh形成用2次元配列(ArcPanel集合体および単体の三角形ポリゴン情報)
    /// </summary>
    private List<List<int>> _trianglesList = new List<List<int>>();

    /// <summary>
    /// for文で _arcAngle(弧の描画角度)に対し_arkMax(弧の描画角度の分割)した角度を i(セグメント数のカウント分)づつ乗算(加算)しイテレート
    /// </summary>
    /// <remarks>
    /// _deg は degree(度)の略で、度数法の角度を表します。1周が360度となります。
    /// _rad は radian(弧度)の略で、弧度法の角度を表し、1周が2πラジアンとなります。
    /// 1ラジアンは円周上の弧の長さが円の半径と等しい場合の角度を意味します。
    /// _hyp(Hypotenuse 斜辺)の長さ: Mathf.Sin(_deg * Mathf.Deg2Rad) ※ _opp(Opposite 対辺)の長さは_hyp(斜辺)と同じ計算式
    /// _adj(Adjacent 隣辺)の長さ: Mathf.Cos(_deg * Mathf.Deg2Rad)
    /// </remarks>
    private void PrepareParams(int idx)
    {
        List<Vector3> _verticesSubList = new List<Vector3>();           // ArcPanel単体の底面と天面の頂点座標配列
        List<int> _trianglesSubList = new List<int>();                  // ArcPanel単体の底面と天面の頂点インデックス配列
        _arkMax = (int)_arcQuality * _arcAngle / 360;                   // 弧単体の弧度分割数算出
        _verticesSubList.Add(new Vector3(0, 0, 0));                     // 底辺の始点座標追加(弧の中心点)
        for (int i = 0; i <= _arkMax; i++)
        {
            _deg = i * _arcAngle / _arkMax + _startAngle;               // _arcAngle(弧の描画角度)に対し_arkMax(弧の描画角度の分割)した角度を i(セグメント数のカウント分)だけ乗算し、更に開始角度を加算して、描画角度を算出
            _hyp = Mathf.Sin(_deg * Mathf.Deg2Rad);                     // 斜辺の長さを算出(始点は中心0となり、長さを終点と解釈)
            _adj = Mathf.Cos(_deg * Mathf.Deg2Rad);                     // 中心から隣辺の長さを算出(始点は中心0となり、長さを終点と解釈)
            _verticesSubList.Add(new Vector3(_hyp, 0, _adj));           // 底面の(弧の円周部分にあたる)頂点座標追加
            _verticesSubList.Add(new Vector3(_hyp, _height, _adj));     // 上面の(弧の円周部分にあたる)頂点座標追加

            if (i <= _arkMax - 1)                                       // 三角形ポリゴンの頂点インデックス(最後のインデックスを追加しないように制御)
            {
                _trianglesSubList.Add(0);                               // 底辺の始点(弧の中心点)
                _trianglesSubList.Add(i * 2 + 1);                       // 底面の(弧の円周部分にあたる)頂点
                _trianglesSubList.Add((i + 1) * 2 + 1);                 // 上面の(弧の円周部分にあたる)次の頂点
            }
        }
        _verticesList.Add(new List<Vector3>(_verticesSubList));         // 各ArcPanelの底面と天面の頂点座標配列情報を追加
        _trianglesList.Add(new List<int>(_trianglesSubList));           // 各ArcPanelの底面と天面の頂点インデックス配列情報を追加
    }

    /// <summary>
    /// ArcPanel単体のmesh生成
    /// </summary>
    /// <param name="idx">ArcPanel生成インデックス</param>
    private void GenerateMesh(int idx)
    {
        List<Mesh> meshList = new List<Mesh>();
        // Mesh mesh = new Mesh();
        // mesh.vertices = _verticesList[idx].ToArray();
        while (meshList.Count <= idx) meshList.Add(new Mesh());         // meshList配列の格納件数分mesh生成
        Mesh mesh = meshList[idx];                                      // meshList配列のidx番目のmesh情報代入
        mesh.vertices = _verticesList[idx].ToArray();                   // ArcPanel単体の頂点座標配列情報代入
        mesh.triangles = _trianglesList[idx].ToArray();                 // ArcPanel単体の頂点インデックス配列情報代入
        mesh.RecalculateNormals();                                      // 法線算出
        mesh.RecalculateBounds();                                       // バウンディング算出
        meshList[idx].name = "ArcMesh";                                 // 動的生成されたmesh名の設定 
        GameObject arkPanelObject = new GameObject("ArkPanel" + idx);   // 動的生成されたオブジェクト名の設定
        arkPanelObject.transform.localScale = _scale;                   // ArcPanel形状のサイズ設定
        arkPanelObject.transform.position += Vector3.up * _height;      // 動的生成されたオブジェクトの上方移動 ※ _height値の加算移動
        // arkPanelObject.transform.SetParent(null);                    // 親オブジェクト無効化(null)
        arkPanelObject.transform.SetParent(transform);                  // 親子関係設定
        arkPanelObject.AddComponent<MeshFilter>().sharedMesh = mesh;    // MeshFilter追加
        arkPanelObject.AddComponent<MeshCollider>().sharedMesh = mesh;  // MeshCollider追加
        MeshRenderer meshRenderer = arkPanelObject.AddComponent<MeshRenderer>();  // MeshRenderer追加        
        Material[] mats = meshRenderer.materials;
        // mats[0] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/ClearGlass.mat");    // Mesh Renderer: Materials: Element[0] へのマテリアル追加 (ファイルパスよりロード)
        // mats[0] = new Material(Shader.Find("Custom/Glass04"));
        mats[0] = new Material(Shader.Find("Custom/ClearGlass"));       // MeshRenderer: Materials: Element[0] へのマテリアル追加
        meshRenderer.materials = mats;
		meshRenderer.material.color = _color;                           // MeshRenderer: Materialsの色指定
    }

    void Start()
    {
        StartCoroutine(Generate());
    }
    private IEnumerator Generate()
    {
        int idxEnd = Mathf.RoundToInt(_height/_divideCnt);              // ArcPanel生成量(分割)算出
        int baseAngle = _startAngle;                                    // ArcPanelsの開始角度設定
        for (int idx = 0; idx < idxEnd; idx++)
        {
            yield return new WaitForSeconds(_yieldTime);
            _startAngle = baseAngle + (idx * 15);                       // ArcPanel単体の開始角度設定
            _height = _height - (_height / idxEnd * idx / (_divideCnt * 4));  // ArcPanel単体の高さ位置設定
			/// 60フレームに1度処理を実行する
			// if(Time.frameCount % 2 == 0)
            PrepareParams(idx);
            GenerateMesh(idx);
        }
    }
}
