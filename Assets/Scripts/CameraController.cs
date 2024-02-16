using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UniRx;

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
public class CameraController : MonoBehaviour
{    
    /// <summary>
    /// Skybox配列
    /// </summary>
    [SerializeField] private Material[] _skyboxList;
    /// <summary>
    /// カメラ移動先位置座標
    /// </summary> <remarks>
    /// ※ ContextMenuItemによりInspector上のExecuteMoveを実行可
    /// </remarks>
    [SerializeField, ContextMenuItem("Move", "ExecuteMove")] private Transform _targetTransform;
    /// <summary>
    /// 対処座標とカメラ座標間の距離
    /// </summary>
    [SerializeField] private int _cameraDistance = 400;
    /// <summary>
    /// 移動速度のフレーム間隔
    /// </summary>
    [SerializeField] private float _lerpTime = 5.0f;
    /// <summary>
    /// カメラの移動開始位置座標
    /// </summary>
    private Vector3 _startPosition;
    /// <summary>
    /// カメラの移動開始回転座標
    /// </summary>
    private Quaternion _startRotation;
    /// <summary>
    /// 継続処理の参照
    /// </summary>
    private IDisposable _trigger;
    /// <summary>
    /// ランダム整数
    /// </summary>
    private int _randNum; 
    /// <summary>
    /// 追跡移動
    /// </summary>
    /// <remarks /> [ref] カメラを現在の位置から特定のポイントまでスムーズに移動させる https://bluebirdofoz.hatenablog.com/entry/2022/02/01/123532
    private IEnumerator Move()
    {
        /// <summary>
        /// // 移動経路配列
        /// </summary>
        /// <typeparam name="string">移動先オブジェクト</typeparam>
        /// <typeparam name="float">移動・待機時間</typeparam>
        var _routePatternList = new Dictionary<string, float>()
        {
            ["Top"] = 5.0f,
            ["CameraPointA"] = 7.0f,
            ["Top"] = 2.0f,
            ["CameraPointB"] = 10.0f,
            ["CameraPointC"] = 10.0f,
        };
        foreach (KeyValuePair<string, float> route in _routePatternList)        // 移動経路イテレータ
        {
            yield return new WaitForSeconds(route.Value);                       // 待機時間
            _targetTransform = GameObject.Find(route.Key).transform;            // 移動先対象オブジェクト
            ExecuteMove();                                                      // 追跡カメラ移動実行
        }
    }
    /// <summary>
    /// 追跡カメラ移動実行
    /// </summary>
    private void ExecuteMove()
    {
        _trigger?.Dispose();                                                    // 前回トリガー終了
        _startPosition = Camera.main.transform.position;                        // カメラ移動開始位置座標の保存
        _startRotation = Camera.main.transform.rotation;                        // カメラ移動開始回転座標の保存
        _trigger = Observable                                                   // 移動処理開始(UniRx.Observable関数)
            .IntervalFrame(1, FrameCountType.FixedUpdate)                       // 1フレーム毎更新
            .TimeInterval()                                                     // フレーム間の経過時間取得
            .Select(intervalTime => intervalTime.Interval)                      // TimeSpan型のデータ抽出
            .Scan((last, current) => last + current)                            // 前回までの経過時間の加算
            .TakeWhile(intervalTimeSpan => (float)intervalTimeSpan.TotalSeconds < _lerpTime)    // イテラブル上限値(Lerp時間)
            .SubscribeOnMainThread()                                            // メインスレッド処理
            .Subscribe(intervalTimeSpan =>{
                float totalInterval = (float)intervalTimeSpan.TotalSeconds;     // イージング計算式によるスムージング化(加速と減速)の補間演算？
                float frameTime = Mathf.Min(totalInterval / _lerpTime, 1.0f);
                float lerpFactor  = (frameTime * frameTime) * (3.0f - (2.0f * frameTime));                
                Camera.main.transform.position = Vector3.Lerp(_startPosition, _targetTransform.position, lerpFactor ); // Leap関数による減速接近演算
                Camera.main.transform.rotation = Quaternion.Lerp(_startRotation, _targetTransform.rotation, lerpFactor ); // Leap関数による減速回転演算
                Camera.main.transform.LookAt(_targetTransform);                 // 元の回転情報を保持
                Camera.main.transform.rotation *= Quaternion.Euler(0f, 0f, 45f);//  注視点の回転を変更
                Camera.main.transform.position = Vector3.Lerp(_startPosition, _targetTransform.position, lerpFactor ) - transform.forward * _cameraDistance;    // 注視点との間隔設定(後方に距離を置く)
            },() =>{
                Camera.main.transform.position = _targetTransform.position - transform.forward * _cameraDistance;    // 最終位置座標を到達値として設定
                Camera.main.transform.rotation = _targetTransform.rotation;     // 最終回転座標を到達値として設定
                // Camera.main.transform.rotation = _startRotation;             // 前回の回転座標を到達地点として設定(元に戻す)
            })
            .AddTo(this);
            // _trigger = Observable                                            // 追加移動処理
            //     .Timer(TimeSpan.FromSeconds(8))                              // 待機秒数
            //     .Subscribe( _ =>{   
            //         // transform.position += new Vector3(0f, 100f, 0f);      // Y軸を100移動
            //         transform.rotation = Quaternion.Euler(0f, 45f, 100f);    // カメラ画角の調整
            //         transform.position = _targetTransform.position - transform.forward * 400.0f; // 注視点との間隔設定(後方に距離を置く)
            //         ExecuteMove();   //  移動開始
            //     })
            //     .AddTo(this);
    }


    /// <summary>
    /// ターゲットのTransform
    /// </summary>
    [SerializeField] private Transform _target = null;
    /// <summary>
    /// 移動速度, 回転速度, ブースト倍率
    /// </summary>
    [SerializeField] private float
    _moveSpeed = 10f,
    _rotationSpeed = 20f, 
     _boost = 2f;
    /// <summary>
    /// 移動モードフラグ
    /// </summary>
    private bool _isMoveMode = false;
    /// <summary>
    /// 前フレームのマウス位置
    /// </summary>
    private Vector3 _prevPos = Vector3.zero;
    /// <summary>
    /// 追跡回転速度
    /// </summary>
    private float _trackingRotationSpeed => _rotationSpeed * Time.deltaTime;
    /// <summary>
    /// マウス位置の初期化
    /// </summary>
    /// <remarks>[ref]  シーンビューライクなカメラ操作 https://edom18.hateblo.jp/entry/2021/03/18/155726 </remarks>
    private void StartMove()
    {
        _isMoveMode = true;
        _prevPos = Input.mousePosition;
    }
    private void EndMove()
    {
        _isMoveMode = false;
    }
    /// <summary>
    /// キー入力に応じてターゲットを移動
    /// </summary>
    private void TryMove()
    {
        if (Input.GetKey(KeyCode.W)) _target.position += _target.forward * _trackingSpeed; 
        if (Input.GetKey(KeyCode.A)) _target.position += -_target.right * _trackingSpeed;
        if (Input.GetKey(KeyCode.S)) _target.position += -_target.forward * _trackingSpeed;
        if (Input.GetKey(KeyCode.D)) _target.position += _target.right * _trackingSpeed;
        if (Input.GetKey(KeyCode.Q)) _target.position += -_target.up * _trackingSpeed;
        if (Input.GetKey(KeyCode.E)) _target.position += _target.up * _trackingSpeed;
    }
    /// <summary>
    /// マウス移動量に応じて回転
    /// </summary>
    private void TryRotate()
    {
        Vector3 delta = Input.mousePosition - _prevPos;
        transform.Rotate(Vector3.up, delta.x * _trackingRotationSpeed, Space.World);
        Vector3 axisRight = Vector3.Cross(transform.forward, Vector3.up);
        transform.Rotate(axisRight.normalized, delta.y * _trackingRotationSpeed, Space.World);
        _prevPos = Input.mousePosition;
    }
    /// <summary>
    /// 追跡速度
    /// </summary>
    private float _trackingSpeed
    {
        get
        {
            float speed = _moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift)) speed *= _boost;               // ブースト時は速度を増加
            return speed;
        }
    }
    /// <summary>
    /// ターゲットを自身にリセット
    /// </summary>
    private void Reset()
    {
        _target = transform;
    }
    /// <summary>
    /// Skybox 回転速度
    /// </summary>
    // [Range(0.01f,0.1f)] private float rotateSpeed;
    private void ReplaceSkybox()
    {
        // _skyboxList = new GetComponent<Material>[]();
        _skyboxList = new Material[]    	                                    // Material読込
        {
            // ResourcesフォルダからPrefab読込
            // (GameObject)Resources.Load("Skyboxes/StarryNight Skybox"),_skyboxMaterial
            // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/StarryNight Skybox.mat"),
            UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/MoonShine Skybox.mat"),
            // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/Overcast1 Skybox.mat"),
            // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/Overcast2 Skybox.mat"),
            UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/Eerie Skybox.mat"),
            // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/Sunny1 Skybox.mat")
            // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/Sunny2 Skybox.mat")
            // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Skyboxes/Sunny3 Skybox.mat")
        };

        _randNum = UnityEngine.Random.Range(0, _skyboxList.Length);
        RenderSettings.skybox = _skyboxList[_randNum];                          // Skybox変更
        // float _rotationValue;                                                // Skybox 回転角度
        // _rotationValue = Mathf.Repeat(_skyboxList[_randNum].GetFloat("_Rotation") + rotateSpeed , 360f);
        // _skyboxList[_randNum].SetFloat("_Rotation",_rotationValue);
        _skyboxList[_randNum].SetFloat("_Rotation", 120f);
    }



    private void Awake()
    {
        ReplaceSkybox();
    }

    private void Start()
    {
        StartCoroutine(Move());                                                 // 移動処理
    }

    // private void Update(){Debug.DrawLine(Camera.main.transform.position, _targetTransform.position, Color.red);}    // カメラ注視点の表示

    private void Update()
    {
        // Debug.DrawLine(Camera.main.transform.position, _targetTransform.position, Color.red);
        if (Input.GetMouseButtonDown(1)) StartMove();	                        // 右クリックで移動モード開始
        if (Input.GetMouseButtonUp(1)) EndMove();	                            // 右クリックを離すと移動モード終了
        if (_isMoveMode)
        {
            TryMove();	                                                        // 移動処理
            TryRotate();	                                                    // 回転処理
        }
        if (_targetTransform != null) Debug.DrawLine(Camera.main.transform.position, _targetTransform.position, Color.red); // カメラ移動経路の視覚化 DrawLine(LineStart, lineEnd, Color) ※ 線描画が細い為、Scene Viewでバックグラウンドを暗くした場合確認可能
    }
}
