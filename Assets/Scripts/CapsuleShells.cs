using System;
using UnityEngine;
using UnityEngine.UIElements;

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
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]

public class CapsuleShells : MonoBehaviour
{
    /// <summary>
    /// MeshRendererプロパティの初期化(付加)
    /// </summary>
    private MeshRenderer _rendererComponent;
    /// <summary>
    /// MeshRendererプロパティの初期化(付加)後、コンポーネントを取得し_rendererComponentに設定内容をキャッシュ(再利用が可能)
    /// </summary>
    private MeshRenderer _renderer => 
    _rendererComponent != null ? _rendererComponent : (_rendererComponent = GetComponent<MeshRenderer>());
    /// <summary>
    /// MeshFilterプロパティの初期化(付加)
    /// </summary>
    private MeshFilter _filterComponent;
    /// <summary>
    /// MeshFilterプロパティの初期化(付加)後、コンポーネントを取得し_filterComponentに設定内容をキャッシュ(再利用が可能)
    /// </summary>
    private MeshFilter _filter => 
    _filterComponent != null ?  _filterComponent : (_filterComponent = GetComponent<MeshFilter>());
    /// <summary>
    /// MeshColliderプロパティの初期化(付加)
    /// </summary>
    private MeshCollider  _colliderComponent;
    /// <summary>
    /// MeshColliderプロパティの初期化(付加)後、コンポーネントを取得し_colliderComponentに設定内容をキャッシュ(再利用が可能)
    /// </summary>
    private MeshCollider _collider =>  
    _colliderComponent != null ?  _colliderComponent : (_colliderComponent = GetComponent<MeshCollider>());
    /// <summary>
    /// Rigidbodyプロパティの初期化(付加)
    /// </summary>
    private Rigidbody _rigidbodyComponent;
    /// <summary>
    /// Rigidbodyプロパティの初期化(付加)後、コンポーネントを取得し_rigidbodyComponentに設定内容をキャッシュ(再利用が可能)
    /// </summary>
    private Rigidbody _rigidbody => 
    _rigidbodyComponent != null ? _rigidbodyComponent : (_rigidbodyComponent = GetComponent<Rigidbody>());
    /// <summary>
    /// _radius: 半径, _thickness: 厚さ, _height: 高さ
    /// </summary>  
    [SerializeField] private float 
    _radius = 5.0f, 
    _thickness = 1.0f, 
    _height = 0.0f;
    /// <summary>
    /// 横(水平線・緯線)と縦(経線)の分割数(法線数・頂点数)
    /// </summary>  
    [SerializeField] private Vector2Int _divide = new Vector2Int(22, 22);
    /// <summary>
    /// _divideH: 横分割数(水平線・緯線), _divideV: 縦分割数(経線)
    /// </summary>  
    private int 
    _divideH, 
    _divideV;
    /// <summary>
    /// 構造体として
    /// 頂点座標と頂点のインデックス情報を元に同様のオブジェクトを小規模で生成する為、インスタンスクラス型(参照型)にせず、構造体型(値型)で生成する
    /// </summary><remarks>
    /// 他のオブジェクトに埋め込まれることが多く、論理的に単一の値を表し、配列が不変的で頻繁にボックス化する必要がない為、クラス型ではなく構造体型を利用
    /// ※ メモリの使用効率が向上の為、メモリはスタック領域を利用
    /// </remarks>
    private struct MeshData
    {
        public Vector3[] VerticesList;
        public int[] IndicesList;
    }

    /// <summary>
    /// 窪みを含む半球型メッシュ形成(長細いカプセル型メッシュ形成の変形版)
    /// </summary><remarks>
    /// [ref] カプセルメッシュの作り方 https://shibuya24.info/entry/unity-mesh-dynamic-capsule
    /// カプセルメッシュは上半球、円柱、下半球を結合した構成で、三体それぞれの表面を縦横分割し、各頂点を三角形で連結してメッシュ形成
    /// 半球メッシュは上半球と反転させた下半球、高さ0の円柱を結合した構成で、二体それぞれの表面を縦横分割し、各頂点を三角形で連結してメッシュ形成
    /// </remarks>
    void Create()
    {
        _divideH = _divide.x;                                               // 横分割数(水平線・緯線)
        _divideV = _divide.y;                                               // 縦分割数(経線)
        var data = CreateCapsule(_divideH, _divideV, _height, _radius);     // 分割数に応じたポリゴン頂点情報の生成
        Mesh mesh = new Mesh();                                             // Mesh構造体インスタンスの生成(頂点、法線、UV座標、三角形などの3Dモデルのジオメトリデータを設定)
        mesh.SetVertices(data.VerticesList);                               // Meshの頂点座標を設定
        mesh.SetIndices(data.IndicesList, MeshTopology.Triangles, 0);      // Meshのインデックス設定(三角形を構成・描画する頂点のインデックスを設定、また、第三引数として、単一のMeshを作成する0を指定)
        _filter.mesh = mesh;                                                // MeshFilterに頂点座標と頂点のインデックス情報を適用
        // mesh.RecalculateNormals();                                       // Meshの各頂点(法線)を補間し再演算(メッシュ全体のなめらかな外観形成用) ※今回は劣化したのでコメントアウト
        // _renderer.material.doubleSidedGI = true;                         // MeshRendererの両面描画(間接光計算)有効化
        _collider.sharedMesh = mesh;                                        // MeshColliderにMesh情報を共有(メモリ使用効率向上の為、同じメッシュ情報を複数のコライダーで共有し利用)
        // _collider.convex = true;                                         // MeshColliderのConvexの有効化
        _collider.enabled = false;                                          // MeshColliderの無効化
        _rigidbody.isKinematic = true;                                      // Kinematicの有効化
        _rigidbody.useGravity = true;                                       // Gravityの有効化
        _rigidbody.mass = 10.0f;                                            // Gravityの重さを指定
        // int layerLv = gameObject.layer;                                  // レイヤー取得
        // gameObject.layer = layerLv;                                      // 衝突判定制御の為にレイヤー変更   
    }

    /// <summary>
    /// 半球型(元カプセル型)メッシュデータの生成
    /// </summary>
    /// <param name="divideH">横分割数(水平線・緯線)</param>
    /// <param name="divideV">縦分割数(経線)</param>
    /// <param name="height">カプセル筒部部の高さ</param>
    /// <param name="radius">カプセル半球外殻部の半径</param>
    /// <returns>生成されたメッシュデータ</returns>
    private MeshData CreateCapsule(int divideH, int divideV, float height, float radius)// 横分割数、縦分割数、高さ、半径のパラメータを受け取り、カプセルメッシュデータを生成
    {
        divideH = divideH < 4 ? 4 : divideH;                                // 横分割数の最小値を4に設定
        divideV = divideV < 4 ? 4 : divideV;                                // 縦分割数の最小値を4に設定
        radius = radius <= 0 ? 0.001f : radius;                             // Inspector上で半径のパラメータ値が不適切な場合、半径の値を最小値0.001fに設定
        if (divideV % 2 != 0) divideV++;                                    // 縦分割数が奇数値となる分割数を回避する為、偶数分割数に調整 

        /// 半球型(元カプセル型)の頂点座標配列作成
        // int vertCnt = divideH * divideV + 2;                             // 分割数より頂点総合計数を算出
        int idxCnt = 0;                                                     // 頂点座標配列および頂点インデックス配列のIndex変数
        var VerticesList = new Vector3[divideH * divideV + 2];             // 頂点座標配列を生成
        float centerEulerRadianH = 2f * Mathf.PI / (float)divideH;          // 横方向の中心角算出
        float centerEulerRadianV = 2f * Mathf.PI / (float)divideV;          // 縦方向の中心角算出
        float offsetHeight = height * 0.5f;                                 // 円柱部の中央を基準に両底辺のオフセット上下幅(高さ)を設定
        /// 天面(元カプセル型上部)の頂点座標作成
        VerticesList[idxCnt++] = new Vector3(0, radius + offsetHeight, 0); // 天面の頂点座標情報追加
        /// 半球外殻部(元カプセル型上部)の頂点座標作成
        for (int vvv = 0; vvv < divideV / 2; vvv++)                         // 半球外殻部(元カプセル型上部)の縦分割数に応じ、各頂点毎に分割角度を加算し、頂点情報を生成するイテレータ
        {
            var radianV = (float)(vvv + 1) * centerEulerRadianV / 2f;       // 半球(外殻)部を縦分割した各頂点角度の算出
            var tmpLenV = Mathf.Abs(Mathf.Sin(radianV) * radius);           // 極座標系による弧長の算出(半球内殻部半径radiusと分割角度radianvを掛算し弧長を算出)
            var tmpYV = Mathf.Cos(radianV) * radius;                        // 半球(外殻)部Y軸の各分割座標を算出(縦分割された各ポリゴン中心角度に基づきcosを算出後、半径を掛算し算出)
            for (int vhv = 0; vhv < divideH; vhv++)                         // 縦横分割した各頂点の座標を算出し、半球(外殻)部の頂点座標を構成(形状を生成)するイテレータ
            {
                var posV = new Vector3                                      // 分割ポリゴンの各頂点座標を生成
                ( 
                    tmpLenV * Mathf.Sin((float)vhv * centerEulerRadianH),   // 半球(外殻)部を縦横分割した各頂点X座標の算出(横分割した各ポリゴン中心角度に基づきsinを算出後、縦分割された弧長tmpLenVを掛算し算出)
                    tmpYV + offsetHeight,                                   // 同じくY座標の算出(カプセル型筒部の中央点から半球の底辺部までの長さoffsetHeightと半球部底辺の中心から上端(上部頂点)までの長さtmpYVを加算し算出)
                    tmpLenV * Mathf.Cos((float)vhv * centerEulerRadianH)    // 同じくZ座標の算出(横分割された各ポリゴン中心角度に基づきcosを算出後、縦分割された弧長tmpLenVを掛算し算出)
                );
                VerticesList[idxCnt++] = posV;                             // 分割ポリゴンの頂点座標追加
            }
        }
        ///  半球内殻部(元カプセル型下部)の頂点座標作成
        // int offset = (divideV / 2) - (int)_thickness;                    // 半球内殻部(元カプセル型下部)の縦分割数の半分から、カプセルの厚さ _thickness を差し引き算出(本来、下部の半カプセルは、厚さを考慮して下方に凸部の形状となるような位置に頂点を設定)
        int offset = 1;                                                     // 元カプセル型下半球部を上半球部(半球外殻部)に対する内殻部となるようにoffset値を変更(凸型の元下半球を凹型内殻半球に変形させる為の変更)
        for (int vvy = 0; vvy < divideV / 2; vvy++)                         // 半球(内殻)部の縦分割数に応じ、各頂点毎に分割角度を加算し、頂点情報を生成するイテレータ
        {
            var radianY = (float)(vvy + offset) * centerEulerRadianV / 2f;  // 半球(内殻)部を縦分割した各頂点角度の算出
            var tmpLenY = Mathf.Abs(Mathf.Sin(radianY) * (radius - _thickness));// 極座標系による弧長の算出(半球外殻部の半径から厚さを差引き半球内殻部の半径を算出後、分割角度radianYを掛算し弧長を算出)
            var tmpYY = Mathf.Cos(radianY) * (radius - _thickness);         // 半球(内殻)部Y軸の各分割座標を算出(縦分割された各ポリゴン中心角度に基づきcosを算出後、半球外殻部の半径から厚さを差引き半球内殻部の半径を掛算し算出) 
            for (int vhy = 0; vhy < divideH; vhy++)                         // 縦横分割した各頂点の座標を算出し、半球(内殻)部の頂点座標を構成(形状を生成)するイテレータ
            {
                var posY = new Vector3(                                     // 分割ポリゴンの各頂点座標を生成
                    tmpLenY * Mathf.Sin((float)vhy * centerEulerRadianH),   // 半球(内殻)部を縦横分割した各頂点X座標の算出(横分割した各ポリゴン中心角度に基づきsinを算出後、縦分割された弧長tmpLenYを掛算し算出)
                    tmpYY + offsetHeight,                                   // 同じくY座標の算出(カプセル型筒部の中央点から半球の底辺部までの長さoffsetHeightと半球部底辺の中心から上端(上部頂点)までの長さtmpYYを加算し算出)
                    tmpLenY * Mathf.Cos((float)vhy * centerEulerRadianH)    // 同じくZ座標の算出(横分割された各ポリゴン中心角度に基づきcosを算出後、縦分割された弧長tmpLenYを掛算し算出)
                );
                VerticesList[idxCnt++] = posY;                             // 分割ポリゴンの頂点座標追加 
            }
        }
        /// 底面(元カプセル型下部)の頂点座標作成 ※不要 
        // VerticesList[idxCnt] = new Vector3(0, -radius - offsetHeight, 0);　// 半球(内殻)部底面の頂点座標情報追加 ※不要


        /// 半球型(元カプセル型)の頂点インデックス配列作成
        int topBtmTriCnt = divideH * 2;                                     // 半球(外殻部と内殻部)の縦横分割による頂点数 
        int aspectTriCnt = divideH * (divideV - 2 + 1) * 2;                 // 筒部の縦横分割による頂点数
        // int triCnt = (topBtmTriCnt + aspectTriCnt) * 3;                  // 三角形の数を掛算し、頂点総合計数を算出
        int[] IndicesList = new int[(topBtmTriCnt + aspectTriCnt) * 3];    // 頂点インデックス配列の生成
        /// 天面(カプセル型上部の半球底辺)の頂点インデックス作成
        int offsetIdx = 0;                                                  // 頂点オフセットのIndex変数
        idxCnt = 0;                                                         // 頂点インデックス配列のIndex変数
        for (int i = 0; i < divideH * 3; i++)                               // 半球(外殻)部底辺の各頂点が三角形を構成する為のインデックスを算出するイテレータ
        {
            if (i % 3 == 0)                                                 // 3で割り余りの値0をインデックス値として設定
            {
                IndicesList[idxCnt++] = 0;                                 // 各三角形の各頂点に0のインデックスを設定
            }
            else if (i % 3 == 1)                                            // 3で割り余りの値1をインデックス値として設定
            {
                IndicesList[idxCnt++] = 1 + offsetIdx;                     // 各三角形の各頂点に1のインデックスを設定
            }
            else if (i % 3 == 2)                                            // 3で割り余りの値2をインデックス値として設定
            {
                var idx = 2 + offsetIdx++;                                  // 各三角形の各頂点に2のインデックスを設定
                idx = idx > divideH ? IndicesList[1] : idx;                // 周回した際(idxがdivideHを超えた時)に半球外殻部(元カプセル上部)の先端座標の頂点インデックスIndicesList[1]と結合・クランプ
                IndicesList[idxCnt++] = idx;                               // 次の横分割頂点インデックス設定の為の加算
            }
        }
        // 半球型(元カプセル型)円柱部の頂点インデックス作成
        /* 側面の頂点インデックスを繋ぐイメージ
         * 1 - 2 
         * | / |  
         * 0 - 3  
         *  
         * 0 >> 1 >> 2 >> 0 >> 2 >> 3 >> (0) 
         * 注意: 円柱部の側面を縦横分割し、それらの頂点を結んで1周した時に形成された周回ポリゴンを結合・Clampする  
         */  

        int startIdx = IndicesList[1];                                     // 円柱部における頂点インデックス変数の開始値に半球部の開始インデックス値を代入
        int sideIdxLen = aspectTriCnt * 3;                                  // 両底面を含まない円柱部側面の頂点数
        int lap1Idx = 0;                                                    // 各周回のの最後から1番目の頂点インデックス値
        int lap2Idx = 0;                                                    // 各周回のの最後から2番目の頂点インデックス値
        int lapDiv = divideH * 2 * 3;                                       // 円柱部側面の頂点数(円柱部側面を縦横分割し、各頂点を連結して円周・横方向に1周したポリゴン)
        int squareFaceCrCnt = 0;                                            // 周回ポリゴン数nのカウント
        for (int i = 0; i < sideIdxLen; i++)                                // 頂点インデックスを生成するイテレータ
        {
            if (i % lapDiv == 0)                                            // 周回判定(横分割数で割切れる回を1周と判定)
            {
                lap1Idx = startIdx;                                         // 各周回ポリゴンの開始インデックス値を設定
                lap2Idx = startIdx + divideH;                               // 各周回ポリゴンの終了インデックス値を設定
                squareFaceCrCnt++;                                          // 周回ポリゴン数カウントのインクリメント(1周毎にインクリメント)
            }
            if (i % 6 == 0 || i % 6 == 3)                                   // 四角形の0・3番目(三角形/逆三角形の0番目)となる頂点かを判定 
            {
                IndicesList[idxCnt++] = startIdx;                           // 頂点インデックス配列への追加(四角形の0・3番目の頂点となるインデックス番号を設定)
            }
            else if (i % 6 == 1)                                            // 四角形の1番目(三角形の1番目)となる頂点を判定
            {
                IndicesList[idxCnt++] = startIdx + divideH;                 // 頂点インデックス配列への追加(四角形の0番目となる開始インデックスカウントと横分割数を加算した番号を設定)
            }
            else if (i % 6 == 2 || i % 6 == 4)                              // 四角形の2・4番目(三角形/逆三角形の2番目)となる頂点を判定
            {
                if (i > 0 && (i % (lapDiv * squareFaceCrCnt - 2) == 0 || i % (lapDiv * squareFaceCrCnt - 4) == 0)) // 連結・Clamp処理の為の周回判定(周回ポリゴンの最後から2番目の頂点を判定)
                {                                                                
                    IndicesList[idxCnt++] = lap2Idx;                        // 頂点インデックス配列への追加(最後から2番目のインデックス番号を設定)
                }
                else                                                        // 各周回の最後から2番目の頂点ではない場合(横方向に連結中のポリゴン上で、四角形の2・4番目(三角形/逆三角形の2番目)となる頂点かを判定)
                {
                    IndicesList[idxCnt++] = startIdx + divideH + 1;         // 頂点インデックス配列への追加(四角形の0番目となる開始インデックスカウントと横分割数と1を加算した番号:四角形の2・4番目となるインデックス番号を設定)
                }
            }
            else if (i % 6 == 5)                                            // 四角形の5番目(逆三角形の2番目)となる頂点を判定
            {
                if (i > 0 && i % (lapDiv * squareFaceCrCnt - 1) == 0)       // 連結・Clamp処理の為の周回判定(周回ポリゴンの最後から1番目の頂点を判定)
                {
                    IndicesList[idxCnt++] = lap1Idx;                        // 頂点インデックス配列への追加(連結・Clamp処理の為、各周回ポリゴンの最終インデックス番号: 四角形の5番目となるインデックス番号を設定)
                }
                else                                                        // 各周回の最後から1番目の頂点ではない場合(横方向に連結中のポリゴン上で、四角形の5番目となる頂点かを判定)
                {
                    IndicesList[idxCnt++] = startIdx + 1;                   // 頂点インデックス配列への追加(四角形の0番目となる開始インデックスカウントに1を加算した番号: 四角形の5番目となるインデックス番号を設定)
                }
                startIdx++;                                                 // 開始インデックスの更新
            }
            else                                                            // エラー判定
            {
                Debug.LogError("Invalid : " + i);                           // 予期しないケースに対するエラーログ
            }
        }

        return new MeshData()                                               // 戻り値 List<int>およびList<Vector3>のMeshData型
        {
            IndicesList = IndicesList,                                      // 半球型メッシュのインデックス保持配列
            VerticesList = VerticesList                                     // 半球型メッシュの頂点座標保持配列
        };
    }

    
    private void Start()
    {
        Create();   
    }

}