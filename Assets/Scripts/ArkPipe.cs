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

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ArkPipe : MonoBehaviour
{
	/// <summary>
	/// _height: 弧(円)集合体の高さ, _arcAngle: 弧度, _startAngle: 開始角度, _arcQuality: 円の分割数(360度), _arkMax: 弧(円)単体の弧度分割数
	/// </summary>
	[SerializeField] private int _height = 10, _arcAngle = 360, _startAngle = 0, _arcQuality = 100, _arkMax;
	/// <summary>
	/// _isOutward: 内向き/外向き, _isMeshRenderer: 描画表示/非表示
	/// </summary>
	[SerializeField] private bool _isOutward = false, _isMeshRenderer = true;
	/// <summary>
	/// 弧単体のRGBA
	/// </summary>
	[SerializeField] private Color _color = new Color(0.0f, 0.75f, 0.75f, 0.15f);
	/// <summary>
	/// 弧(円)集合体のサイズ
	/// </summary>
	[SerializeField] private Vector3 _scale = new Vector3(10, 1, 10);
	/// <summary>
	/// 弧(円)集合体の頂点座標用2次元配列(弧集合体および単体の頂点座標情報)
	/// </summary>
	private Vector3[] _verticesList;
	/// <summary>
	/// 弧(円)集合体のMesh形成用2次元配列(弧集合体および単体の三角形Index情報)
	/// </summary>
	private int[] _trianglesList;
	/// <summary>
	/// _deg(degree): 度数法の角度, _hyp(Hypotenuse): 斜辺の長さ, _adj(Adjacent): 隣辺の長さ
	/// </summary>
	private float _deg, _hyp, _adj;
	/// <summary>
	/// 筒型メッシュ生成
	/// </summary><remarks>
	/// [ref] Unity C# スクリプトで円弧と筒を作成 https://blog.narumium.net/2016/11/21/unity-c-スクリプトで円弧と筒を作成する/
	/// [ref] unityでrayをcollider裏面に当てる方法 https://qiita.com/NEGO/items/448ea07f91fb9d4ef5e5
	/// </remarks>
	private void makeParams(){
		List<Vector3> _verticesSubList = new List<Vector3>();
		List<int> _trianglesSubList = new List<int>();
		_arkMax = (int)_arcQuality * _arcAngle / 360;

		for (int i = 0; i <= _arkMax; i++){
			_deg = i *_arcAngle/_arkMax + _startAngle;
			_hyp = Mathf.Sin(_deg * Mathf.Deg2Rad);
			_adj = Mathf.Cos(_deg * Mathf.Deg2Rad);
			_verticesSubList.Add(new Vector3(_hyp,0,_adj));
			_verticesSubList.Add(new Vector3(_hyp,_height,_adj));
			if(i <= _arkMax - 1){
				if(_isOutward){
					_trianglesSubList.Add(2*i);_trianglesSubList.Add(2*i+3);_trianglesSubList.Add(2*i+1);
					_trianglesSubList.Add(2*i);_trianglesSubList.Add(2*i+2);_trianglesSubList.Add(2*i+3);
				}else{
					_trianglesSubList.Add(2*i);_trianglesSubList.Add(2*i+1);_trianglesSubList.Add(2*i+3);
					_trianglesSubList.Add(2*i);_trianglesSubList.Add(2*i+3);_trianglesSubList.Add(2*i+2);
				}
			}
			// }
		}
		_verticesList  = _verticesSubList.ToArray();
		_trianglesList = _trianglesSubList.ToArray();
	}
	private void setParams(){
		Mesh mesh = new Mesh();										// mesh生成
		mesh.vertices = _verticesList;								// 弧(円)集合体の頂点座標配列情報代入
		mesh.triangles = _trianglesList;							// 弧(円)集合体の頂点インデックス配列情報代入
		mesh.RecalculateNormals();									// 法線算出
		mesh.RecalculateBounds();									// バウンディング算出
		mesh.name = "TubeMesh";										// 動的生成されたmesh名の設定
		transform.localScale = _scale;								// 弧(円)集合体形状のサイズ設定
		GetComponent<MeshFilter>().sharedMesh = mesh;				// MeshFilter取得
		GetComponent<MeshCollider>().sharedMesh = mesh;				// MeshCollider取得
		GetComponent<MeshCollider>().convex.Equals(true);			// Convexの無効化
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();	// MeshRenderer取得
		meshRenderer.material.doubleSidedGI = true;					// queriesHitBackfacesの有効化
		Material[] mats = meshRenderer.materials;					// Material設定
		Material glassMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/ClearGlass.mat");	// Mesh Renderer: Materials: Element0にマテリアル追加
		mats[0] = glassMaterial;									// Mesh Renderer: Materials: Element[0]のマテリアル設定
		meshRenderer.materials = mats;								// 色設定
		meshRenderer.material.color = _color;						// 色指定
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;	// 両面シャドウ投影化
		if (!_isMeshRenderer) GetComponent<MeshRenderer>().enabled = false;	// Inspector上の設定によりMeshRendererの無効化
	}

	void Start()
	{
		makeParams();
		setParams();
	}
}
