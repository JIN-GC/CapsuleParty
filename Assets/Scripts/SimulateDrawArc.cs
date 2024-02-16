using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(LineRenderer))]
public class SimulateDrawArc : MonoBehaviour
{

    /// <summary>
    /// 弧集合体の高さ *インスペクター上で設定する変数
    /// </summary>
    [HideInInspector] private float _arcHeight = 1.0f;
    // public class ArcHeight
    // {
    //     [SerializeField] private float _arcHeight
    //     {
    //         get 
    //         { 
    //             return _arcHeight;
    //         }
    //         set 
    //         {
    //             _arcHeight = value;
    //         }
    //     }
    // }

    /// <summary>
    /// _arkAngle: 弧度, _startAngle: 開始角度, _arkQuality: 円の分割数(360度), _arkMax: 弧単体の弧度分割数
    /// </summary>
    [HideInInspector] private int _arcAngle = 90, _startAngle = 0;
    [SerializeField] private int _arcQuality = 100, _arkMax;

    private string _showClicked = "Click Me!";

    private string ArcHeight = "ArcHeight", ArcAngle = "ArcAngle", StartAngle = "StartAngle", ShowClicked = "ShowClicked";

    /// <summary>
	/// _deg(degree): 度数法の角度, _hyp(Hypotenuse): 斜辺の長さ, _adj(Adjacent): 隣辺の長さ
    /// </summary>
    private float _deg, _hyp, _adj;

    private int _clickCnt = 0;                                                  // クリック回数
    [SerializeField] private List<Dictionary<string, string>> _subList = new List<Dictionary<string, string>>();  // 演算情報用配列
    [SerializeField] private List<Vector3> _verticesSubList = new List<Vector3>();  // ArcPanel単体の頂点座標用配列
    [SerializeField] private List<int> _trianglesSubList = new List<int>();     // ArcPanel単体のmesh形成用配列

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
    void ArkPanel()
    {
        _arkMax = (int)_arcQuality * _arcAngle / 360;                           // 弧単体の弧度分割数算出
        int m = 0, n = 0;
        for (int i = 0; i <= _arkMax; i++)
        {
            _deg = i * _arcAngle / _arkMax + _startAngle;                       // _arcAngle(弧の描画角度)に対し_arkMax(弧の描画角度の分割)した角度を i(セグメント数のカウント分)だけ乗算し、更に開始角度を加算して、描画角度を算出
            _hyp = Mathf.Sin(_deg * Mathf.Deg2Rad);                             // 斜辺の長さを算出(始点は中心0となり、長さを終点と解釈)
            _adj = Mathf.Cos(_deg * Mathf.Deg2Rad);                             // 中心から隣辺の長さを算出(始点は中心0となり、長さを終点と解釈)

            _verticesSubList.Add(new Vector3(0, 0, 0)); m++;                    // 底辺の始点座標追加(弧の中心点)
            _verticesSubList.Add(new Vector3(_hyp, 0, _adj)); m++;              // 底面の(弧の円周部分にあたる)頂点座標追加
            _verticesSubList.Add(new Vector3(_hyp, _arcHeight, _adj)); m++;        // 上面の(弧の円周部分にあたる)頂点座標追加
            _verticesSubList.Add(new Vector3(0, _arcHeight, 0)); m++;              // 上面の終点座標追加(弧の中心点)
            
            /// 三角形ポリゴンの頂点インデックスの追加 
            _trianglesSubList.Add(0); n++;                                      // 底辺の始点(弧の中心点)
            _trianglesSubList.Add(i * 2 + 1); n++;                              // 底面の(弧の円周部分にあたる)頂点
            _trianglesSubList.Add((i + 1) * 2 + 1); n++;                        // 上面の(弧の円周部分にあたる)次の頂点

            if (i <= _arkMax - 1)                                               // 最後の三角形ポリゴンの頂点インデックスを追加しないように制御
            {
                _trianglesSubList.Add(0); n++;                                  // 底辺の始点(弧の中心点)
                _trianglesSubList.Add((i + 1) * 2 + 1); n++;                    // 上面の(弧の円周部分にあたる)次の頂点
                _trianglesSubList.Add((i + 2) * 2 + 1); n++;                    // 底面の(弧の円周部分にあたる)次の次の頂点
            }
            _subList.Add(new Dictionary<string, string>()                       // UI表示出力用配列
            {
                {"i", i.ToString()}, 
                {"_deg", _deg.ToString()}, 
                {"_hyp", _hyp.ToString()}, 
                {"_adj", _adj.ToString()},
                {"idx0", (m-4).ToString()},
                {"_vSL[idx0]", _verticesSubList[m-4].ToString()},
                {"idx1", (n-6).ToString()},
                {"_tSL[idx1]", _trianglesSubList[n-6].ToString()},
                {"idx2", (n-3).ToString()},
                {"_tSL[idx2]", _trianglesSubList[n-3].ToString()}
            });
            _subList.Add(new Dictionary<string, string>()
            {
                {"i", i.ToString()}, 
                {"_deg", _deg.ToString()}, 
                {"_hyp", _hyp.ToString()}, 
                {"_adj", _adj.ToString()},
                {"idx0", (m-3).ToString()},
                {"_vSL[idx0]", _verticesSubList[m-3].ToString()},
                {"idx1", (n-5).ToString()},
                {"_tSL[idx1]", _trianglesSubList[n-5].ToString()},
                {"idx2", "NA"},
                {"_tSL[idx2]", "NA"}
            });
            _subList.Add(new Dictionary<string, string>()
            {
                {"i", i.ToString()}, 
                {"_deg", _deg.ToString()}, 
                {"_hyp", _hyp.ToString()}, 
                {"_adj", _adj.ToString()},
                {"idx0", (m-2).ToString()},
                {"_vSL[idx0]", _verticesSubList[m-2].ToString()},
                {"idx1", (n-4).ToString()},
                {"_tSL[idx1]", _trianglesSubList[n-4].ToString()},
                {"idx2", (n-2).ToString()},
                {"_tSL[idx2]", _trianglesSubList[n-2].ToString()}
            });
            _subList.Add(new Dictionary<string, string>()
            {
                {"i", i.ToString()}, 
                {"_deg", _deg.ToString()}, 
                {"_hyp", _hyp.ToString()}, 
                {"_adj", _adj.ToString()},
                {"idx0", (m-1).ToString()},
                {"_vSL[idx0]", _verticesSubList[m-2].ToString()},
                {"idx1", (n-1).ToString()},
                {"_tSL[idx1]", _trianglesSubList[n-1].ToString()},
                {"idx2", "NA"},
                {"_tSL[idx2]", "NA"}
            });
        }
        _verticesSubList.Add(new Vector3(0, 0, 0));                             //  最終INDEXエラー回避
        _subList.Add(new Dictionary<string, string>(){{"i", "DONE"}, {"_deg", "_deg"}, {"_hyp", "_hyp"}, {"_adj", "_adj"}, {"idx0", "idx0"}, {"_vSL[idx0]", "_vSL[idx0]"}, {"idx1", "idx1"}, {"_tSL[idx1]", "_tSL[idx1]"}, {"idx2", "idx2"}, {"_tSL[idx2]", "_tSL[idx2]"}});
        // foreach (var item in _subList) foreach (var pair in item) Debug.Log($"{pair.Key}: {pair.Value}");   // _subList配列内容のDebug表示
    }

    private LineRenderer _lineRendererComponent;
    private LineRenderer _lineRenderer => _lineRendererComponent != null ? _lineRendererComponent : (_lineRendererComponent = GetComponent<LineRenderer>());
    private Gradient _gradient;                                                 // 線描画のグラデーション
    private AnimationCurve _animationCurve;                                     // 線描画のアニメーションカーブ
    void DrawLine(int idxEnd)
    {
        // Destroy(_lineRenderer);                                              // LineRendererコンポーネント廃棄
        _lineRendererComponent = gameObject.GetComponent<LineRenderer>();       // LineRendererコンポーネントをゲームオブジェクトにアタッチ
        _lineRenderer.startWidth = 0.01f;                                       // 線描画の始点線幅
        _lineRenderer.endWidth = 0.03f;                                         // 線描画の終点線幅
        // _lineRenderer.widthCurve = _animationCurve;                          // 線描画のアニメーションカーブ幅
        // _lineRenderer.colorGradient = _gradient;                             // 線描画のグラデーション
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // 線描画のマテリアル
        // _lineRenderer.material.SetColor("_Color", Color.green);              // 線描画のマテリアルカラー
        _lineRenderer.startColor = Color.red;                                   // 線描画色追加
        _lineRenderer.startColor = Color.white;                                 // 線描画色追加
        // _lineRenderer.loop = true;                                           // 始点と終点間の連結線表示
        // _lineRenderer.useWorldSpace = true;                                  // ワールド座標
        // _lineRenderer.useWorldSpace = false;                                 // ローカル座標
        // _lineRenderer.numCapVertices = 10;                                   // 線両端の角丸化
        // _lineRenderer.numCornerVertices = 10;                                // 中間点の角丸化

        var positions = new Vector3[idxEnd];
        for (int idx=0; idx < idxEnd; idx++) positions[idx] = _verticesSubList[idx];  // 頂点座標追加
        _lineRenderer.positionCount = idxEnd;                                   // 頂点数指定
        _lineRenderer.SetPositions(positions);                                  // 頂点座標指定
    }

    //スクロールビュー内のコンテンツの数
    /// <summary>
    /// MENU_IDX: MENUコンテンツ数, MENU_MARGIN: MENUマージン, MENU_WIDTH: MENU幅, MENU_HEIGHT: MENU高さ
    /// </summary>
	private const int 
    MENU_IDX = 13,
	MENU_WIDTH = 150,
	MENU_HEIGHT = 25;
	private int _menuMarginX()
	{
		return (Screen.width - MENU_WIDTH) / 2 ;
	}
	private int _menuMarginY()
	{
		return Screen.height - (MENU_HEIGHT * (MENU_IDX + 1));
	}
    /// <summary>
    /// コンテンツ規格サイズ
    /// </summary>
	/// <remarks>
	/// Rect 引数(矩形左上のx, y, 幅, 高さ)
	/// </remarks>
	private Rect _contSize()
	{
		return new Rect(_menuMarginX(), MENU_HEIGHT, MENU_WIDTH, MENU_HEIGHT);
	} 
	// Rect _contSize = new Rect(MENU_MARGIN_X, MENU_HEIGHT, MENU_WIDTH, MENU_HEIGHT);
    /// <summary>
    /// コンテンツ配列
    /// </summary>
	/// <remarks>
	/// Rect 引数(矩形左上のx, y, 幅, 高さ)
	/// </remarks>
	private Rect[] _contList = new Rect[MENU_IDX];
    /// <summary>
    /// スクロールの位置更新
    /// </summary>
	private Vector2 _scrollPosition = Vector2.zero;
	/// <summary>
	/// _viewPosition: スクロールビュー表示範囲, _wholePosition: スクロールビュー全体の範囲
	/// </summary>
	private Rect _viewPosition()
	{
		return new Rect(_menuMarginX(), _menuMarginY() + (MENU_HEIGHT * 9.5f), MENU_WIDTH + _menuMarginX(), MENU_HEIGHT * 3);
	}
	private Rect _wholePosition()
	{
		return new Rect(_menuMarginX(), _menuMarginY() + (MENU_HEIGHT * 2), MENU_WIDTH, MENU_HEIGHT * (MENU_IDX - 1));
	}
	/// <summary>
	/// スクロールビュー表示切替え
	/// </summary>
	private bool _showMenu;

    public void OnGUI()
    {           
        Rect rect2 = new Rect(0, 10, 250, 110);                                  // 引数(矩形左上のx, y, 幅, 高さ)
        _styleState.textColor = Color.white;
        _style.normal = _styleState;
        GUI.Label(rect2, _uiInfo2, _style);                                     // GUIテキスト表示

        Rect rect1 = new Rect(0, Screen.height - 110, 250, 110);                // 引数(矩形左上のx, y, 幅, 高さ)
        _styleState.textColor = Color.blue;
        _style.normal = _styleState;
        GUI.Label(rect1, _uiInfo1, _style);                                     // GUIテキスト表示

        _style.fontSize = 10;

        if (_clickCnt >= _verticesSubList.Count)
        {   
            Rect rect3 = new Rect((Screen.width/2)-80, (Screen.height/2)-160, 40, 20);  //  引数(矩形左上のx, y, 幅, 高さ)
            _styleStateFinal.textColor = Color.magenta;
            _styleFinal.normal = _styleStateFinal;
            _styleFinal.fontStyle = FontStyle.Bold;
            _styleFinal.fontSize = 24;
            GUI.Label(rect3, "COMPLETED", _styleFinal);                         // GUIテキスト表示
        }

        GUIStyle _styleButton = GUI.skin.GetStyle("button"); 		            // ボタンとボックスがGUIStyleになるように設定
		GUIStyle _styleBox = GUI.skin.GetStyle("box");
		
		_styleButton.fontSize = 14;									            // ボタンとボックスの fontSize変更
		_styleBox.fontSize = 16;
		int idx = 0;

		if(GUI.Button
			(new Rect(
				_menuMarginX(), 
				_menuMarginY() + (MENU_HEIGHT * (MENU_IDX-5)), 
				MENU_WIDTH, 
				MENU_HEIGHT * 1.5f), 
			$"{_showClicked}"))                               // スクロールビュー表示切替えボタン
			// $"_arcHeight:{_arcHeight}\nClick Count: {_clickCnt} of {_subList.Count}"))                // スクロールビュー表示切替えボタン
		{
			_showMenu = !_showMenu;
            // Debug.Log ("_showMenu!");
            if (_showMenu)
            {
                ScrollContents();                                               // スクロールコンテンツ描画
            }
		}
		if(!_showMenu) return;

		_scrollPosition = GUI.BeginScrollView(_viewPosition(), _scrollPosition, _wholePosition());  //スクロールビューの開始位置

		if(GUI.Button(_contList[++idx], "PLAY  >  "))                           // 進むボタン
		{
            if (_clickCnt <= _subList.Count)                // クリック回数によって赤線の描画を制御
            {
    			_clickCnt++;
                if (_clickCnt < _subList.Count)                // クリック回数によって赤線の描画を制御
                {
                    DrawLine(_clickCnt);
                    DispUI(_clickCnt, 1);
                    DispUI(_clickCnt, 0);
                    _showClicked = $"PLAY\nClick Count: {_clickCnt} of {_subList.Count}";
                }
            }
		}

		if(GUI.Button(_contList[++idx], "REVERSE  <"))                                   // 戻るボタン
		{
            if (_clickCnt >= 0)                // クリック回数にる線描画の制御
            {
    			_clickCnt--;
                if (_clickCnt > 0)                // クリック回数にる線描画の制御
                {
                    DrawLine(_clickCnt);
                    DispUI(_clickCnt, 0);
                    DispUI(_clickCnt, 1);
                    _showClicked = $"REVERSE\nClick Count: {_clickCnt} of {_subList.Count}";
                }
            }
		}

		if(GUI.Button(_contList[++idx], "RESET"))                               // カウントリセットボタン
		{
            _showClicked = $"Click Me!";
            SaveSettings();                                                     // JSONファイルに保存
            SceneManager.LoadScene("Simulation");
            // DrawLine(_clickCnt);
            // DispUI(_clickCnt, 0);
            // DispUI(_clickCnt, 1);
            // Start();            
		}

		if(GUI.Button(_contList[++idx], "AUTO-PLAY >>"))                           // 進むボタン
		{
            if (_clickCnt <= _subList.Count)                // クリック回数によって赤線の描画を制御
            {
                StartCoroutine(AutoPlay());   //繰り返し処理を呼び出す
            }
		}

		if(GUI.Button(_contList[++idx], "AUTO-REV-PLAY <<"))                   // 戻るボタン
		{
            if (_clickCnt >= 0)                // クリック回数にる線描画の制御
            {
                StartCoroutine(AutoReversePlay());   //繰り返し処理を呼び出す
            }
		}

		if(GUI.Button(_contList[++idx], "HEIGHT: +0.1"))					        // 高さ+1ボタン
        {
            if(_arcHeight < 2.5f)
            {
                _arcHeight += 0.1f;
                _showClicked = $"++ HEIGHT\nArc Height: {_arcHeight}";
                SaveSettings();                                                 // JSONファイルに保存
                SceneManager.LoadScene("Simulation");
            }
        }

		if(GUI.Button(_contList[++idx], "HEIGHT: -0.1"))					        // 高さ-1ボタン
        {   
            if(_arcHeight > 0)
            {
                _arcHeight -= 0.1f;
                _showClicked = $"-- HEIGHT\nArc Height: {_arcHeight}";
                SaveSettings();                                                 // JSONファイルに保存
                SceneManager.LoadScene("Simulation");
            }
        }

		if(GUI.Button(_contList[++idx], "ARC-ANGLE: +30°"))					        // 高さ-1ボタン
        {   
            if(_arcAngle < 360)
            {
                _arcAngle += 30;
                _showClicked = $"++ ARC-ANGLE\nArc Angle: {_arcAngle}";
                SaveSettings();                                                 // JSONファイルに保存
                SceneManager.LoadScene("Simulation");
            }
        }

		if(GUI.Button(_contList[++idx], "ARC-ANGLE: -30°"))					        // 高さ-1ボタン
        {   
            if(_arcAngle > 30)
            {
                _arcAngle -= 30;
                _showClicked = $"-- ARC-ANGLE\nArc Angle: {_arcAngle}";
                SaveSettings();                                                 // JSONファイルに保存
                SceneManager.LoadScene("Simulation");
            }
        }

		if(GUI.Button(_contList[++idx], "START-ANGLE: +30°"))					        // 高さ-1ボタン
        {   
            if(_startAngle < 360)
            {
                _startAngle += 30;
                _showClicked = $"++ START-ANGLE\nStart Angle: {_startAngle}";
                SaveSettings();                                                 // JSONファイルに保存
                SceneManager.LoadScene("Simulation");
            }
        }

		if(GUI.Button(_contList[++idx], "START-ANGLE: -30°"))					        // 高さ-1ボタン
        {   
            if(_startAngle > 0)
            {
                _startAngle -= 30;
                _showClicked = $"-- START-ANGLE\nStart Angle: {_startAngle}";
                SaveSettings();                                                 // JSONファイルに保存
                SceneManager.LoadScene("Simulation");
            }
        }

		if(GUI.Button(_contList[++idx], "HOME"))					            // BACK to MAIN ボタン
		{
            SaveSettings();   
			SceneManager.LoadScene("Start");
		}
		
		GUI.EndScrollView();								                    // スクロールビューの終了位置

    }

    /// <summary>
	/// 描画の自動再生
	/// </summary>
    private IEnumerator AutoPlay()
    {
        while (_clickCnt < _subList.Count)
        {
            yield return new WaitForSeconds(0.25f); //5秒待つ
            _clickCnt++;
            if (_clickCnt < _subList.Count)
            {
                _showClicked = $"AUTO-PLAY\nClick Count: {_clickCnt} of {_subList.Count}";
                DrawLine(_clickCnt);
                DispUI(_clickCnt, 1);
                DispUI(_clickCnt, 0);

            }
        }
    }

    /// <summary>
	/// 描画の自動逆再生
	/// </summary>
    private IEnumerator AutoReversePlay()
    {
        while (_clickCnt > 0)
        {
            yield return new WaitForSeconds(0.25f); //5秒待つ
            _clickCnt--;
            if (_clickCnt > 0)
            {
                _showClicked = $"AUTO-REV-PLAY\nClick Count: {_clickCnt} of {_subList.Count}";
                DrawLine(_clickCnt);
                DispUI(_clickCnt, 0);
                DispUI(_clickCnt, 1);

            }
        }
    }

    /// <summary>
	/// JSONファイルへの設定書き出し
	/// </summary>
    private void SaveSettings()
    {
        // PlayerPrefsを使って変数を保存
        PlayerPrefs.SetFloat(ArcHeight, _arcHeight);
        PlayerPrefs.SetInt(ArcAngle, _arcAngle);
        PlayerPrefs.SetInt(StartAngle, _startAngle);
        PlayerPrefs.SetString(ShowClicked, _showClicked);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // PlayerPrefsから変数を読み込む
        if (PlayerPrefs.HasKey(ArcHeight))
        {
            _arcHeight = PlayerPrefs.GetFloat(ArcHeight);
        }
        if (PlayerPrefs.HasKey(ArcAngle))
        {
            _arcAngle = PlayerPrefs.GetInt(ArcAngle);
        }
        if (PlayerPrefs.HasKey(StartAngle))
        {
            _startAngle = PlayerPrefs.GetInt(StartAngle);
        }
        if (PlayerPrefs.HasKey(ShowClicked))
        {
            _showClicked = PlayerPrefs.GetString(ShowClicked);
        }
    }
    /// <summary>
	/// スクロールコンテンツ描画
	/// </summary>
    private void ScrollContents()
    {
        int marginY = _menuMarginY();                                           // コンテンツボタン開始位置(_MenuMarginY)の再計算
        for (int i = 0; i < MENU_IDX; i++)
        {
            _contList[i] = _contSize();
 			_contList[i].y = marginY + (MENU_HEIGHT * (i + 1));
        }
    }

    [SerializeField] private string _uiInfo, _uiInfo1, _uiInfo2;
    
    private GUIStyle _style, _styleFinal;
    private GUIStyleState _styleState, _styleStateFinal;
    private void DispUI(int idx, int end)
    {
        
        Dictionary<string, string> curtPoint = _subList[idx];                   // UIに表示するテキストを生成
        Dictionary<string, string> prevPoint = _subList[idx-1];
        _uiInfo = 
            $"[idx  {idx}] of [分割数 _arkMax  {_arkMax}] ([_arcHeight] {(float)_arcHeight} && [_startAngle] {_startAngle} ) = \n"
            + $"[_arcAngle] {_arcAngle} * [_arcQuality] {(int)_arcQuality}\n"
            + $"[_deg {curtPoint["_deg"]}] = [描画列 i {curtPoint["i"]}] * [_arcA {_arcAngle}] / [_arkM {_arkMax}] + [_startA {_startAngle}]\n"
            + $"[_hyp {curtPoint["_hyp"]}] = Mathf.Sin([_deg]  {curtPoint["_deg"]})\n"
            + $"[_adj {curtPoint["_adj"]}] = Mathf.Cos([_deg]  {curtPoint["_deg"]})\n";
        if (end == 0)
        {   
            _uiInfo1 = _uiInfo
            + $"[{curtPoint["i"]}列 始点座標 _vSL[{curtPoint["idx0"]}]  {curtPoint["_vSL[idx0]"]}]\n"
            + $"[{curtPoint["i"]}列 始点IDX値#1 _tSL[{curtPoint["idx1"]}] {curtPoint["_tSL[idx1]"]}]\n"
            + $"[{curtPoint["i"]}列 始点IDX値#2 _tSL[{curtPoint["idx2"]}]  {curtPoint["_tSL[idx2]"]}]\n";
        }
        else if (end == 1)
        {
            _uiInfo2 = _uiInfo
            + $"[{curtPoint["i"]}列 終点座標 _vSL[{curtPoint["idx0"]}]  {curtPoint["_vSL[idx0]"]}]\n"
            + $"[{curtPoint["i"]}列 終点IDX値#1 _tSL[{curtPoint["idx1"]}]  {curtPoint["_tSL[idx1]"]}]\n"
            + $"[{curtPoint["i"]}列 終点IDX値#2 _tSL[{curtPoint["idx2"]}]  {curtPoint["_tSL[idx2]"]}]\n";
        }
    }
    private void Start()
    {
        
        // if (PlayerPrefs.HasKey("ArcHeight"))                                 // PlayerPrefsより保存された値の読み込み
        // {
        //     _arcHeight = PlayerPrefs.GetFloat("ArcHeight");
        // }
        
        LoadSettings();                                                         // JSONファイルの読み込み

        ArkPanel();                                                             // ArcPanel生成

        _style = new GUIStyle();
        _styleFinal = new GUIStyle();
        _styleState = new GUIStyleState();
        _styleStateFinal = new GUIStyleState();

        _lineRenderer.positionCount = 0;                                        // LineRenderer初期値パネル非表示用
		ScrollContents();										                // スクロールコンテンツ描画
        // Debug.Log("【クイックマニュアル】  ♪【左クリック】次の頂点まで線描画を進める。   ♬【右クリック】一つ前の頂点まで線描画を戻す。 \n 【オプション】描画角度/密度、開始角度、高さ、線色/線種などInspector上で設定");
    }

    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0) ||                                   // マウスの左クリックまたはシングルタップを検知
    //     (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
    //     {
    //         _clickCnt++;
    //         // Debug.Log($"[_clickCnt++]: {_clickCnt}");
    //         if ((_clickCnt < _subList.Count) && (_clickCnt > 0))             // クリック回数によって赤線の描画を制御
    //         {
    //             DrawLine(_clickCnt);
    //             DispUI(_clickCnt, 1);
    //             DispUI(_clickCnt, 0);
    //         }
    //     }
        
    //     if (Input.GetMouseButtonDown(1) ||                                   // ダブルクリックまたは右クリックを検知
    //     (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began && Input.GetTouch(0).phase == TouchPhase.Began))
    //     {   
    //         _clickCnt--;
    //         // Debug.Log($"[_clickCnt--]: {_clickCnt}");
    //         if ((_clickCnt < _subList.Count) && (_clickCnt > 0))             // クリック回数にる線描画の制御
    //         {
    //             DrawLine(_clickCnt);
    //             DispUI(_clickCnt, 0);
    //             DispUI(_clickCnt, 1);
    //         }
    //     }
    // }
}