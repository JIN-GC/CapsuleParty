using System.Collections;
using UnityEngine;
using System;
using UniRx;
using System.Collections.Generic;

// https://bluebirdofoz.hatenablog.com/entry/2022/02/01/123532
public class CameraController : MonoBehaviour
{    
    [SerializeField, ContextMenuItem("Move", "ExecuteMove")]    // (ContextMenuItemによりInspector上のExecuteMoveを実行可)
    private Transform p_TargetTransform;    // カメラ移(オブジェクト)動先位置座標
    private Vector3 p_StartPosition;    // カメラの移動開始位置座標
    private Quaternion p_StartRotation;    // カメラの移動開始回転座標

    private IDisposable p_Trigger;   // 継続処理の参照
    [SerializeField] private int p_CameraDistance = 400;  //  対処座標とカメラ座標間の距離

    [SerializeField] private float p_LerpTime = 5.0f;  //  移動速度のフレーム間隔

    private void Start(){StartCoroutine(Move());} // 移動処理

    // private void Update(){Debug.DrawLine(Camera.main.transform.position, p_TargetTransform.position, Color.red);}    // カメラ注視点の表示

    private IEnumerator Move()
    {
        var routePatternList = new Dictionary<string, float>()  //  移動経路配列
        {
            ["Top"] = 5.0f,
            ["CameraPointA"] = 7.0f,
            ["Top"] = 2.0f,
            ["CameraPointB"] = 10.0f,
            ["CameraPointC"] = 10.0f,
        };

        foreach (KeyValuePair<string, float> routePoint in routePatternList)  //  移動経路イテレータ
        {
            yield return new WaitForSeconds(routePoint.Value);  // 待機時間
            p_TargetTransform = GameObject.Find(routePoint.Key).transform;    // 移動先対象オブジェクト
            ExecuteMove();    // 移動処理
        }
    }

    private void ExecuteMove()
    {
        // 前回トリガー終了
        p_Trigger?.Dispose();
        p_StartPosition = Camera.main.transform.position;    // カメラ移動開始位置座標の保存
        p_StartRotation = Camera.main.transform.rotation;    // カメラ移動開始回転座標の保存
        p_Trigger = Observable   // 移動処理開始(UniRx.Observable関数)
            .IntervalFrame(1, FrameCountType.FixedUpdate)   // 1フレーム毎更新
            .TimeInterval()                                 // フレーム間の経過時間取得
            .Select(intervalTime => intervalTime.Interval)  // TimeSpan型のデータ抽出
            .Scan((last, current) => last + current)        // 前回までの経過時間の加算
            .TakeWhile(intervalTimeSpan => (float)intervalTimeSpan.TotalSeconds < p_LerpTime)   // イテラブル上限値(Lerp時間)
            .SubscribeOnMainThread()                        // メインスレッド処理
            .Subscribe(
            intervalTimeSpan =>
            {
                float totalInterval = (float)intervalTimeSpan.TotalSeconds; // イージング計算式によるスムージング化(加速と減速)の補間演算？
                float frameTime = Mathf.Min(totalInterval / p_LerpTime, 1.0f);
                float lerpFactor  = (frameTime * frameTime) * (3.0f - (2.0f * frameTime));
                
                Camera.main.transform.position = Vector3.Lerp(p_StartPosition, p_TargetTransform.position, lerpFactor ); // Leap関数による減速接近演算
                Camera.main.transform.rotation = Quaternion.Lerp(p_StartRotation, p_TargetTransform.rotation, lerpFactor ); // Leap関数による減速回転演算
                Camera.main.transform.LookAt(p_TargetTransform); // 元の回転情報を保持
                Camera.main.transform.rotation *= Quaternion.Euler(0f, 0f, 45f);    //  注視点の回転を変更
                Camera.main.transform.position = Vector3.Lerp(p_StartPosition, p_TargetTransform.position, lerpFactor ) - transform.forward * p_CameraDistance;    // 注視点との間隔設定(後方に距離を置く)
            },
            () =>
            {
                Camera.main.transform.position = p_TargetTransform.position - transform.forward * p_CameraDistance;    // 最終位置座標を到達値として設定
                Camera.main.transform.rotation = p_TargetTransform.rotation; // 最終回転座標を到達値として設定
                // Camera.main.transform.rotation = p_StartRotation; // 前回の回転座標を到達地点として設定(元に戻す)
            }
            )
            .AddTo(this);
            // p_Trigger = Observable     // 追加移動処理
            //     .Timer(TimeSpan.FromSeconds(8))  //  待機秒数
            //     .Subscribe(
            //     _ =>
            //     {   
            //         
            //         // transform.position += new Vector3(0f, 100f, 0f);  // Y軸を100移動
            //         transform.rotation = Quaternion.Euler(0f, 45f, 100f);    // カメラ画角の調整
            //         transform.position = p_TargetTransform.position - transform.forward * 400.0f;    // 注視点との間隔設定(後方に距離を置く)
            //         ExecuteMove();   //  移動開始
            //     })
            // .AddTo(this);
    }

    // https://edom18.hateblo.jp/entry/2021/03/18/155726
    [SerializeField] private Transform p_Target = null;	// ターゲットのTransform
    [SerializeField] private float p_MoveSpeed = 10f;	// 移動速度
    [SerializeField] private float p_RotationSpeed = 20f;	// 回転速度
    [SerializeField] private float p_Boost = 2f;	// ブースト倍率
    private bool p_IsMoveMode = false;	// 移動モードフラグ
    private Vector3 p_PrevPos = Vector3.zero;	// 前フレームのマウス位置
    private float p_TrackingSpeed
    {
        get
        {
            float speed = p_MoveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift)) speed *= p_Boost;	// ブースト時は速度を増加
            return speed;
        }
    }
    private float p_TrackingRotationSpeed => p_RotationSpeed * Time.deltaTime;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) StartMove();   // 右クリックで移動モード開始
        if (Input.GetMouseButtonUp(1)) EndMove();   // 右クリックを離すと移動モード終了
        if (p_IsMoveMode)
        {
            TryMove();	// 移動処理
            TryRotate();	// 回転処理
        }
    }
    private void Reset(){p_Target = transform;}   // ターゲットを自身にリセット
    private void StartMove()
    {
        p_IsMoveMode = true;
        p_PrevPos = Input.mousePosition;	// マウス位置の初期化
    }
    private void EndMove(){p_IsMoveMode = false;}
    private void TryMove()
    {
        if (Input.GetKey(KeyCode.W)) p_Target.position += p_Target.forward * p_TrackingSpeed; // キー入力に応じてターゲットを移動
        if (Input.GetKey(KeyCode.A)) p_Target.position += -p_Target.right * p_TrackingSpeed;
        if (Input.GetKey(KeyCode.S)) p_Target.position += -p_Target.forward * p_TrackingSpeed;
        if (Input.GetKey(KeyCode.D)) p_Target.position += p_Target.right * p_TrackingSpeed;
        if (Input.GetKey(KeyCode.Q)) p_Target.position += -p_Target.up * p_TrackingSpeed;
        if (Input.GetKey(KeyCode.E)) p_Target.position += p_Target.up * p_TrackingSpeed;
    }
    private void TryRotate()
    {
        Vector3 delta = Input.mousePosition - p_PrevPos;
        transform.Rotate(Vector3.up, delta.x * p_TrackingRotationSpeed, Space.World);   // マウスの移動量に応じて回転
        Vector3 axisRight = Vector3.Cross(transform.forward, Vector3.up);
        transform.Rotate(axisRight.normalized, delta.y * p_TrackingRotationSpeed, Space.World);
        p_PrevPos = Input.mousePosition;
    }
}
