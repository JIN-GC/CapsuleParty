using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;


public class CameraController : MonoBehaviour
{
    // https://bluebirdofoz.hatenablog.com/entry/2022/02/01/123532
    /// カメラの移動先
    /// (ContextMenuItemによりInspectorからExecuteMoveを実行できる)
    [SerializeField, ContextMenuItem("Move", "ExecuteMove")]
    private Transform p_TargetTransform;

    /// カメラの移動開始地点
    private Vector3 p_StartPosition;
    private Quaternion p_StartRotation;

    /// 継続処理の参照
    private IDisposable p_Trigger;
    public int distance = 400;


    [SerializeField] private float p_LerpTime = 5.0f;


    private void Start()
    {

        Debug.Log("Start");
        // p_TargetTransform = GameObject.FindGameObjectWithTag("Finish").transform;
        StartCoroutine(Move());
        // StartCoroutine(StartCompress());

    }


    private void Update()
    {
        // カメラの注視点を表示
        // Debug.DrawLine(Camera.main.transform.position, p_TargetTransform.position, Color.red);
    }

    private IEnumerator Move()
    {
        Debug.Log("Camera Start-S7");
        yield return new WaitForSeconds(7.0f);
        // p_TargetTransform = GameObject.FindGameObjectWithTag("Finish").transform;
        p_TargetTransform = GameObject.Find("Top").transform;
        ExecuteMove();
        yield return new WaitForSeconds(5.0f);
        ExecuteMove();
        Debug.Log("Camera Start-E");

        Debug.Log("Camera Move-S30");
        p_TargetTransform = GameObject.Find("Ground").transform;
        // p_TargetTransform = GameObject.FindGameObjectWithTag("Finish").transform;
        yield return new WaitForSeconds(20.0f);
        ExecuteMove();
        Debug.Log("Camera Move-E");
    }

    public void ExecuteMove()
    {
        // 前回トリガーを終了する
        p_Trigger?.Dispose();

        // カメラの移動開始地点を保存する
        p_StartPosition = Camera.main.transform.position;
        p_StartRotation = Camera.main.transform.rotation;

        // 移動処理を開始する
        p_Trigger = Observable
            .IntervalFrame(1, FrameCountType.FixedUpdate)    // 1フレーム毎に呼び出す
            .TimeInterval()                                  // フレーム間の経過時間を取得する
            .Select(timeInterval => timeInterval.Interval)   // TimeSpan型のデータを抽出する
            .Scan((last, current) => last + current)         // 前回までの経過時間を加算する
            .TakeWhile(intervalTimeSpan => (float)intervalTimeSpan.TotalSeconds < p_LerpTime) // Lerp時間を超えるまで実行する
            .SubscribeOnMainThread()                         // メインスレッドで実行する
            .Subscribe(
            intervalTimeSpan =>
            {
                float totalInterval = (float)intervalTimeSpan.TotalSeconds;
                // Ease-in, Ease-Out の計算式で徐々に加速して徐々に減速する補間を行う
                float t = Mathf.Min(totalInterval / p_LerpTime, 1.0f);
                float lerpFactor = (t * t) * (3.0f - (2.0f * t));
                // Leap関数を使って徐々にターゲットに近づいていく
                // Camera.main.transform.rotation = Quaternion.Euler(0f, 100f, 0f);
                Camera.main.transform.position = Vector3.Lerp(p_StartPosition, p_TargetTransform.position, lerpFactor);
                Camera.main.transform.rotation = Quaternion.Lerp(p_StartRotation, p_TargetTransform.rotation, lerpFactor);
                // 注視点の回転を変更しつつ、元の回転情報を保持
                Camera.main.transform.LookAt(p_TargetTransform);
                Camera.main.transform.rotation *= Quaternion.Euler(0f, -100f, 0f);

                // 指定の距離だけ後ろに離す
                Camera.main.transform.position = Vector3.Lerp(p_StartPosition, p_TargetTransform.position, lerpFactor) - transform.forward * distance;
 
                
            },
            () =>
            {
                // ExecuteMove();
                // 最終的に指定のトランスフォームに到達させる
                Camera.main.transform.position = p_TargetTransform.position - transform.forward * distance;
                Camera.main.transform.rotation = p_TargetTransform.rotation;
                // Camera.main.transform.rotation = p_StartRotation; // 元の回転に戻す
            }
            )
            .AddTo(this);
            Debug.Log("Camera Move1-S 8");
            // // 移動処理を開始する
            // p_Trigger = Observable
            //     .Timer(TimeSpan.FromSeconds(8))  // 秒後に実行
            //     .Subscribe(
            //     _ =>
            //     {   
            //         // Y軸を100移動させる
            //         // transform.position += new Vector3(0f, 100f, 0f);
            //         // カメラの画角を下向きに調整
            //         transform.rotation = Quaternion.Euler(0f, 45f, 100f);
            //         // 100ほど離れた場所から撮影する
            //         Vector3 newPosition = p_TargetTransform.position - transform.forward * 400.0f;
            //         transform.position = newPosition;
            //         ExecuteMove();
            //     })
            // .AddTo(this);
            // Debug.Log("Camera Move2-S");
            // // 移動処理を開始する
            // p_Trigger = Observable
            //     .Timer(TimeSpan.FromSeconds(15))  // 秒後に実行
            //     .Subscribe(
            //     _ =>
            //     {   
            //         // Y軸を100移動させる
            //         // transform.position += new Vector3(0f, 100f, 0f);
            //         transform.rotation = Quaternion.Euler(0f, 100f, 0f);
            //         // 100ほど離れた場所から撮影する
            //         Vector3 newPosition = p_TargetTransform.position - transform.forward * 200.0f;
            //         transform.position = newPosition;
            //         ExecuteMove();
            //     })
            // .AddTo(this);
            // Debug.Log("Camera Move2-E");
            }


    // https://edom18.hateblo.jp/entry/2021/03/18/155726
    // [SerializeField] private Transform _target = null;
    // [SerializeField] private float _moveSpeed = 10f;
    // [SerializeField] private float _rotateSpeed = 20f;
    // [SerializeField] private float _boost = 2f;

    // private bool _isMoveMode = false;
    // private Vector3 _prevPos = Vector3.zero;

    // private float MoveSpeed
    // {
    //     get
    //     {
    //         float speed = _moveSpeed * Time.deltaTime;

    //         if (Input.GetKey(KeyCode.LeftShift))
    //         {
    //             speed *= _boost;
    //         }

    //         return speed;
    //     }
    // }

    // private float RotateSpeed => _rotateSpeed * Time.deltaTime;

    // #region ### MonoBehaviour ###

    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         StartMove();
    //     }

    //     if (Input.GetMouseButtonUp(1))
    //     {
    //         EndMove();
    //     }

    //     if (_isMoveMode)
    //     {
    //         TryMove();
    //         TryRotate();
    //     }
    // }

    // private void Reset()
    // {
    //     _target = transform;
    // }

    // #endregion ### MonoBehaviour ###

    // private void StartMove()
    // {
    //     _isMoveMode = true;
    //     _prevPos = Input.mousePosition;
    // }

    // private void EndMove()
    // {
    //     _isMoveMode = false;
    // }

    // private void TryMove()
    // {
    //     if (Input.GetKey(KeyCode.W))
    //     {
    //         _target.position += _target.forward * MoveSpeed;
    //     }

    //     if (Input.GetKey(KeyCode.A))
    //     {
    //         _target.position += -_target.right * MoveSpeed;
    //     }

    //     if (Input.GetKey(KeyCode.S))
    //     {
    //         _target.position += -_target.forward * MoveSpeed;
    //     }

    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         _target.position += _target.right * MoveSpeed;
    //     }

    //     if (Input.GetKey(KeyCode.Q))
    //     {
    //         _target.position += -_target.up * MoveSpeed;
    //     }

    //     if (Input.GetKey(KeyCode.E))
    //     {
    //         _target.position += _target.up * MoveSpeed;
    //     }
    // }

    // private void TryRotate()
    // {
    //     Vector3 delta = Input.mousePosition - _prevPos;

    //     transform.Rotate(Vector3.up, delta.x * RotateSpeed, Space.World);

    //     Vector3 rightAxis = Vector3.Cross(transform.forward, Vector3.up);
    //     transform.Rotate(rightAxis.normalized, delta.y * RotateSpeed, Space.World);

    //     _prevPos = Input.mousePosition;
    // }


    private IEnumerator StartCompress()
    {
        yield return new WaitForSeconds(60.0f);
        // メインカメラにアタッチされた場合にのみ処理を行う
        Camera mainCamera = GetComponent<Camera>();
        Debug.Log("StartCompress");
        if (mainCamera != null)
        {
            Debug.Log("S StartCompress");
            // カメラとライトを除外するためのレイヤーマスクの設定
            int excludeLayerMask = ~(1 << LayerMask.NameToLayer("MainCamera") | 1 << LayerMask.NameToLayer("Light"));

            // シェーダーをロードしてマテリアルに適用
            Shader flattenShader = Shader.Find("Custom/Flatten");
            if (flattenShader != null)
            {
                Debug.Log("Compress");
                Material flattenMaterial = new Material(flattenShader);
                mainCamera.SetReplacementShader(flattenShader, "");
                mainCamera.cullingMask = excludeLayerMask;
                
            }
            else
            {
                Debug.LogError("Flatten shader not found!");
            }
        }
        else
        {
            Debug.LogError("This script should be attached to the main camera!");
        }
    }

    // void OnDisable()
    // {
    //     // シーンがアンロードされるか、スクリプトが無効になったときに元のシェーダーに戻す
    //     if (flattenMaterial != null)
    //     {
    //         Camera.main.ResetReplacementShader();
    //         Destroy(flattenMaterial);
    //     }
    // }

}
