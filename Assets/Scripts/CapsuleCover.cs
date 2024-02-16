using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]
public class CapsuleCover : MonoBehaviour
{
    /// <summary>
    /// [AudioSource] AudioSourceコンポーネント取得 
    /// </summary>
    private static AudioSource _audioSourceComponent;
    private static AudioSource _audioSource => _audioSourceComponent != null ? _audioSourceComponent : (_audioSourceComponent = FindObjectOfType<AudioSource>());
    // private static AudioSource _audioSource;
    /// <summary>
    /// AudioClip配列
    /// </summary>
    [SerializeField] private static AudioClip[] _audioClipList;
    private static AudioClip _AudioClip; 
    /// <summary>
    /// Capsule Prefab配列
    /// </summary>
    [SerializeField] private GameObject[] _prefabList;

    private void Awake()
    {
        _audioClipList = new AudioClip[]                                        // Resourcesフォルダからメディア読込
        {
            
            (AudioClip)Resources.Load("Sounds/OpenCapsule"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs00"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs01"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs02"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs03"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs04"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs05")
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs06")
        };
        _prefabList = new GameObject[]                                          // ResourcesフォルダからPrefab読込
        {
            
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 1"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 2"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 3"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 4"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 5")
            // 任意のフォルダからPrefab読込
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 1/Prefab/Rocket 1.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 2/Prefab/Prefab 2.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 3/Prefab/Prefab 3.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 4/Prefab/Prefab 4.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 5/Prefab/Prefab 5.prefab")
        };
        PlayRandomHanabiClips(new Vector3(0,0,0));
    }
    /// <summary>
    /// 重力の強さ
    /// </summary>
    /// [SerializeField] private float _gravity = 9.8f;
    private void Update()
    {
        CapsuleController();
        /// <summary>
        /// 重力の方向を設定
        /// </summary>
        /// Vector3 _gravityDirection = (transform.position - Camera.main.transform.position).normalized;
        /// Physics.gravity = _gravity * _gravityDirection;
    }
        /// <summary>
        /// 選択サウンド再生 
        /// </summary>
        /// <param name="idx"></param><summary>
        /// _audioClipList にロードされたサウンドの idx 指定
        /// </summary>
        /// <param name="idx"></param>
        public static void PlaySelectSound(int idx)
    {
        _audioSource.PlayOneShot(_audioClipList[idx]);
        _audioSource.volume = 1.0f;                                             // ボリューム設定
        _audioSource.Play();                                                    // 再生
        // _audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/OpenCapsule"), 1.0f);
        // GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/OpenCapsule"), 1.0f);        
    }
    /// <summary>
    /// ロード/セット済み Sound のランダム再生
    /// </summary>
    private void PlayRandomSoundClip()
    {
        _audioSource.clip = _audioClipList[UnityEngine.Random.Range(1, _audioClipList.Length)];  // AudioClipのランダム選択
        _audioSource.volume = 0.1f;                                             // ボリューム設定
        _audioSource.Play();                                                    // 再生
    }
    /// <summary>
    /// ロード/セット済み prefab のランダム生成
    /// </summary>
    public void PlayRandomHanabiClips(Vector3 position)
    {
        int randNum = UnityEngine.Random.Range(0, _prefabList.Length);
        // for (int i=0; i<randNum; i++) Instantiate(_prefabList[i], new Vector3((gameObject.transform.position.x+(i*5)), (gameObject.transform.position.y+y), (gameObject.transform.position.z+(i*5))), Quaternion.identity, transform);
        for (int i=0; i<randNum; i++)
        {
            Instantiate(_prefabList[i], 
            new Vector3(position.x, position.y, position.z), 
            Quaternion.identity, 
            transform.parent);
        }
        ///
        /// int randNum = UnityEngine.Random.Range(0, _prefabList.Length);
        /// for (int i=0; i<randNum; i++)
        /// {
        ///     GameObject prefabHanabi = (GameObject)Instantiate(
        ///     Resources.Load($"Prefabs/Fireworks/Rocket {i}"),                 // 打上花火Prefab選択
        ///     new Vector3((randNum+1) * 5, y, (randNum+1)*5),                  // 打上座標設定(重複回避)
        ///     // new Vector3(gameObject.transform.position.x+(i*5), gameObject.transform.position.y+y, gameObject.transform.position.z+(i*5)), // 打上座標設定(重複回避)
        ///     Quaternion.identity,
        ///     transform);
        /// }
    }
    /// <summary>
    /// ColiderのisTriggerのチェックをONにし衝突判定を無効化(すり抜ける時)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        PlayRandomSoundClip();
        // if (gameObject.activeSelf && other.gameObject.transform.parent.name == "Frame") Debug.Log($"[@ OnTriggerEnter] [parent.name]: {other.gameObject.transform.parent.name}");
    }
    /// <summary>
    /// ColiderのisTriggerのチェックをONにし衝突判定を無効化(すり抜けきる時)
    /// </summary>
    /// void OnTriggerStay(Collider other)
    /// {
    ///     // if (gameObject.activeSelf && other.gameObject.transform.parent.name == "Frame") Debug.Log($"[@ OnTriggerStay] [parent.name]: {other.gameObject.transform.parent.name}");
    /// }

    /// <summary>
    /// ColiderのisTriggerのチェックを外し接触判定有効化(対RigiBodyオブジェクト用)
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        PlayRandomSoundClip();
        // if (other.transform.parent.name == "Frame") Debug.Log($"[@ OnCollisionEnter] [parent.name]: {other.transform.parent.name}");
    }
    /// <summary>
    /// ColiderのisTriggerのチェックを外し接触判定有効化(対RigiBodyオブジェクト用) 物体接触中
    /// </summary>
    /// void OnCollisionStay(Collision collision)
    /// {
    ///     // if (collision.transform.parent.name == "Frame") SetInActive(3);   // 非アクティブ化(3)ケース#3: Ground上でOpen出来なかった場合
    /// }

    private void SetInActive(int pattern)
    {
        if(gameObject.activeSelf && transform.tag != "Finish")
        {
            Capsules.SetPrefabsCheckCnt();                                      // 終了判定変数カウント(Capsules.cs)
            transform.tag = "Finish";                                           // 終了判定用タグ設定(Capsules.cs)
            // Debug.Log($"@ SetInActive [pattern]: {pattern} [transform.name]: {transform.name.ToString()} [transform.tag.name]: {transform.tag.ToString()}");
            gameObject.SetActive(false);                                        // オブジェクトの非アクティブ化(廃棄検討)        
        }
    }

    /// <summary>[Vector3] ポリゴン形状用の頂点情報配列<para /><example>
    /// [例] Meshで三角形を形成する際の頂点情報を配列化する。四角形の場合(三角形頂点x2) 配列内容{[0]:0, [1]:1, [2]:2, [3]:0, [4]:2, [5]:3}
    /// </example></summary>
    /// <remarks>[補足] 平面上の多角形ポリゴン形状(Polygon)と物体の表面が外向きに湾曲した凸形状(Convex)とは別処理</remarks> 
    private readonly int[] p_IndicesList; 
    /// <summary>
    /// [Vector3] 停止判定エリア/速度
    /// </summary>
    [SerializeField] private Vector3 
    _areaRange = new Vector3(250f, 50f, 250f), 
    _speedRange = new Vector3(10f, 1.5f, 10f),
    _lowAreaRange = new Vector3(250f, 250f, 250f), 
    _lowSpeedRange = new Vector3(0.5f, 0.005f, 0.5f),
    _highAreaRange = new Vector3(250f, -250f, 250f), 
    _highSpeedRange = new Vector3(10f, 100f, 10f);

    /// <summary>
    /// [Vector3] 1フレーム前座標の位置, 現在座標位置, 現在速度
    /// </summary>
    private Vector3 
    _prevPosition = new Vector3(0, 0, 0), 
    _curtPosition, 
    _curtVelocity;
    
    /// <summary>
    /// _activeCnt: オブジェクト活動カウント, 
    /// _inActiveCnt: オブジェクト非活動カウント, 
    /// _lowSpeedCnt: 低速判定(フレーム)カウント,
    /// _lowSpeedRange: 低速判定基準,
    /// _highSpeedRange: 校則判定基準,
    /// _lostSpeedRate: 停止時間(フレーム)係数,
    /// _lostSpeedCnt: 停止時間判定(フレーム)カウント 10秒
    /// </summary>
    private float 
    _activeCnt = 0f, 
    _inActiveCnt = 0f,
    _lowSpeedCnt = 0f,
    // _lowSpeedRange = 0.5f,
    // _highSpeedRange = 100.0f,
    _lostSpeedRate = 35f,
    _lostSpeedCnt = 10f;

    /// <summary>
    /// [AudioSource] AudioSourceコンポーネント 
    /// </summary>
    /// private AudioSource _audioSource;

    private void CapsuleController()
    {
        if(gameObject.activeSelf)
        {
            if (Mathf.Approximately(Time.deltaTime, 0)) return;                 // deltaTime(1フレーム/秒の経過時間)が0の場合、次フレーム処理へスキップ
            _curtPosition = transform.position;                                 // 現在位置座標取得
            _curtVelocity = (_curtPosition - _prevPosition) / Time.deltaTime;   // 現在速度計算
            _prevPosition = _curtPosition;                                      // 前回位置座標の更新(次処理用)
            // Debug.Log($"##[Speed & Position] _curtVelocity = {_curtVelocity}  _curtPosition = {_curtPosition}");  // 現在速度/位置ログ
            // Debug.Log($"##[Before Capsule Open] {transform.parent.name.ToString()} _activeCnt = {_activeCnt}  Math.Abs(_curtVelocity.x) = {Math.Abs(_curtVelocity.x)}  Math.Abs(_curtVelocity.y = {Math.Abs(_curtVelocity.y)}  Math.Abs(_curtVelocity.z = {Math.Abs(_curtVelocity.z)} =====  Math.Abs(_curtPosition.x) = {Math.Abs(_curtPosition.x)}  Math.Abs(_curtPosition.y) = {Math.Abs(_curtPosition.y)}  Math.Abs(_curtPosition.z) = {Math.Abs(_curtPosition.z)} ");
            if ((Math.Abs(_curtVelocity.x) < _lowSpeedRange.x && Math.Abs(_curtVelocity.y) < _lowSpeedRange.y && Math.Abs(_curtVelocity.z) < _lowSpeedRange.z && _curtPosition.y < _lowAreaRange.y && _curtPosition.y >= _areaRange.y))
            {
                _inActiveCnt += Time.deltaTime;                                 // 低速移動検知時 _inActiveCnt カウント
                // Debug.Log($"# [低速移動時 _inActiveCnt カウント] _inActiveCnt = {_inActiveCnt} [parent.name.]: {transform.name.ToString()}");
            }
            else
            {
                _inActiveCnt = 0f;                                              // 移動速度検知時 _inActiveCnt リセット
                // Debug.Log($"# [中速移動時 _inActiveCnt リセット] _inActiveCnt = {_inActiveCnt} [parent.name.]: {transform.name.ToString()}");
            }
            if (_inActiveCnt > 0f) 
            {
                // Debug.Log($"# [スピードダウン検知 _inActiveCnt>0f] _lowSpeedCnt = {_lowSpeedCnt} [p_LowSpeedRate>=p_MinSpeedRate] {_lowSpeedCnt} >= {_lostSpeedCnt} [parent.name.]: {transform.name.ToString()}");
                _lowSpeedCnt += _inActiveCnt * _lostSpeedRate * Time.deltaTime; // 低速移動検知/低速検知頻度算出
            }
            if (_lowSpeedCnt >= _lostSpeedCnt/2 && _inActiveCnt <= 0f) _lowSpeedCnt = 0f; // 5秒(p_MinSpeedRate/2)以上の移動検知時 _lostSpeedCnt リセット
            if (_lowSpeedCnt >= _lostSpeedCnt) SetInActive(1);                  // 非アクティブ化(1) #1: Groundの手前で留まるケース (_lostSpeedCnt 10秒検知)
            if (Math.Abs(_curtVelocity.y) > _highSpeedRange.y && _curtPosition.y < _highAreaRange.y)
            {
                // Debug.Log($"# [ハイスピード検知 _curtVelocity]: {_curtVelocity} [_curtPosition>]: {_curtPosition} [parent.name.]: {transform.name.ToString()}");
                SetInActive(2);                                                 // 非アクティブ化(2) #2: Ground より落下し Open Capsule 出来ないケース
            }
            if (gameObject.activeSelf && Math.Abs(_curtVelocity.x) < _speedRange.x && Math.Abs(_curtVelocity.y) < _speedRange.y && Math.Abs(_curtVelocity.z) < _speedRange.z && _curtPosition.y < _areaRange.y)
            {
                ++_activeCnt;
                if (_activeCnt > 30)
                {
                    Transform[] childTransforms = GetComponentsInChildren<Transform>();    //  子オブジェクト全取得                    
                    if (gameObject.transform.parent != null && (gameObject.name == "Capsule_Right" || gameObject.transform.Find("Capsule_Right") != null))
                    {
                        foreach (Transform childTransform in childTransforms)   // 親子関係の全解除
                        {
                            if (childTransform != this.transform)               // 子オブジェクトの存在判定
                            // if (childTransform != parentObject.transform)
                            {
                                childTransform.parent = null;                   // 親子関係の解除
                                Material parentMaterial = this.GetComponent<MeshRenderer>().materials[0];// 親オブジェクトのマテリアル取得
                                // Material parentMaterial = parentObject.GetComponent<MeshRenderer>().materials[0];
                                MeshRenderer childMeshRenderer = childTransform.GetComponent<MeshRenderer>();   // 取得した親マテリアル情報を子のMeshRendererに設定(※ 半カプセルの現メッシュ構成では親と同様の表示できない)
                                Material childMaterial = new Material(parentMaterial);
                                // childMeshRenderer.material = parentMaterial;
                                childMeshRenderer.materials = new Material[]
                                {
                                    // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/Glass.mat"),
                                    childMaterial,
                                    UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/ClearGlass.mat")
                                };
                                MeshCollider meshCollider = childTransform.GetComponent<MeshCollider>();    // MeshCollider凸形状(Convex)取得
                                if (meshCollider != null)
                                {
                                    meshCollider.convex = true;                 // Convex の有効化 (OnTriggerEnter)
                                    meshCollider.enabled = true;                // meshCollider の有効化
                                }
                                Rigidbody childRigidbody = childTransform.GetComponent<Rigidbody>();
                                if (childRigidbody != null)
                                {
                                    childRigidbody.isKinematic = false;         // 子オブジェクト Rigidbody の isKinematic 無効化
                                    if ((gameObject.name == "Capsule_Right" || gameObject.transform.Find("Capsule_Right") != null))
                                    {
                                        childRigidbody.AddForce(transform.forward * -200.0f, ForceMode.VelocityChange); // 反発処理(後方)
                                        childRigidbody.AddForce(transform.up * -200.0f, ForceMode.VelocityChange); // 反発処理(下方)
                                    }
                                    if ((gameObject.name == "Capsule_Left" || gameObject.transform.Find("Capsule_Left") != null))
                                    {
                                        childRigidbody.AddForce(-transform.forward * -200.0f, ForceMode.VelocityChange); // 反発処理(後方)
                                        childRigidbody.AddForce(transform.up * -200.0f, ForceMode.VelocityChange); // 反発処理(下方)
                                    }
                                }
                            }
                        }
                        if (this != null)                                       // 左右の半カプセル処理後
                        {
                            PlaySelectSound(0);                                 // OpenCapsule Sound(0) の再生
                            PlayRandomHanabiClips(new Vector3(gameObject.transform.position.x, transform.position.y-70f, transform.position.z));  // Hanabi Prefab 生成
                            SetInActive(0);                                     // 非アクティブ化(0) #0: Ground上で Capsule Open 出来するケース　
                        }
                    }
                    return;                                                     // 親子関係解除後は、次のフレーム処理へスキップ
                }
            } 
        }
    }
}
