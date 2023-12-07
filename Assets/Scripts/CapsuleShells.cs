using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEditor;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(NavMeshAgent))]
// [RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]

public class CapsuleShells : MonoBehaviour
{
    private MeshRenderer _renderer;
    private MeshRenderer Renderer => _renderer != null ? _renderer : (_renderer = GetComponent<MeshRenderer>());

    private MeshFilter _filter;
    private MeshFilter Filter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

    private MeshCollider _collider;
    private MeshCollider Collider => _collider != null ? _collider : (_collider = GetComponent<MeshCollider>());

    private Rigidbody _rigidbody;
    private Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>());


    [SerializeField] private Vector2Int _divide = new Vector2Int(20, 20);

    /// <summary>高さ</summary>  
    [SerializeField] public float _height = 0.0f;
    /// <summary>厚さ</summary>  
    [SerializeField] public float _thickness = 2.0f;

    /// <summary>半径</summary>  
    [SerializeField] float _radius = 10.0f;

    // 停止場所判定  
    public Vector3 RangeOA = new Vector3(150, 35, 150);

    // 停止速度判定  
    public Vector3 RangeOS = new Vector3(10, 4.5f, 10);

    private readonly int[] indices;

    // void Start() => Create();
    // public Vector3 m_Center;
    // public Vector3 m_Size, m_Min, m_Max;


    void Start()
    {

        Create();
        // 初期位置を保持
        _prevPosition = transform.position;
    }


    void Create()
    {
        int divideH = _divide.x;
        int divideV = _divide.y;

        var data = CreateCapsule(divideH, divideV, _height, _radius);
        Mesh mesh = new Mesh();
        mesh.SetVertices(data.vertices);
        mesh.SetIndices(data.indices, MeshTopology.Triangles, 0);
        Filter.mesh = mesh;

        // gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        // // Convexを有効にする
        // gameObject.GetComponent<MeshCollider>().convex = true;

        // MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        // meshCollider.sharedMesh = mesh;
        // meshCollider.convex = false;
        Collider.sharedMesh = mesh;
        // Collider.convex = false;
        // Colliderを無効にする
        Collider.enabled = false;

        // //Fetch the center of the Collider volume
        // m_Center = meshCollider.bounds.center;
        // //Fetch the size of the Collider volume
        // m_Size = meshCollider.bounds.size;
        // //Fetch the minimum and maximum bounds of the Collider volume
        // m_Min = meshCollider.bounds.min;
        // m_Max = meshCollider.bounds.max;

        // Add Rigidbody
        // gameObject.AddComponent<Rigidbody>();
        // Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        Rigidbody.isKinematic = true;
        // rb.useGravity = true;
        Rigidbody.mass = 10.0f;

        // // MeshRendererを取得
        // MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        // // queriesHitBackfacesを有効にする
        // meshRenderer.material.doubleSidedGI = true;

        // mesh.RecalculateNormals();


    }

    // 1フレーム前の位置
    private Vector3 _prevPosition;
    private int cntVelocity = 0;


    private AudioSource audioSource;
    // public GameObject prefabFromAssets;

    private void Update()
    {
        // deltaTimeが0の場合は何もしない
        if (Mathf.Approximately(Time.deltaTime, 0))
            return;
        // 現在位置取得
        var position = transform.position;
        // 現在速度計算
        var velocity = (position - _prevPosition) / Time.deltaTime;
        // 現在速度をログ出力
        // Debug.Log($"velocity = {velocity}  position = {position}");
        // Debug.Log($"##OPEN Before  {transform.parent.name.ToString()} cntVelocity = {cntVelocity}  Math.Abs(velocity.x) = {Math.Abs(velocity.x)}  Math.Abs(velocity.y = {Math.Abs(velocity.y)}  Math.Abs(velocity.z = {Math.Abs(velocity.z)} =====  Math.Abs(position.x) = {Math.Abs(position.x)}  Math.Abs(position.y) = {Math.Abs(position.y)}  Math.Abs(position.z) = {Math.Abs(position.z)} ");
        // if (Math.Abs(velocity.x) < 10 && Math.Abs(velocity.y) < 5 && Math.Abs(velocity.z) < 10 && Math.Abs(position.x) < 50 && Math.Abs(position.y) < 20 && Math.Abs(position.z) < 50)
        if (Math.Abs(velocity.x) < RangeOS.x && Math.Abs(velocity.y) < RangeOS.y && Math.Abs(velocity.z) < RangeOS.z && Math.Abs(position.x) < RangeOA.x && Math.Abs(position.y) < RangeOA.y && Math.Abs(position.z) < RangeOA.z)
        {
            ++cntVelocity;
            if (cntVelocity > 30)
            {
                // openNav();
                GameObject parentObject = null;

                if (gameObject.transform.parent != null && (gameObject.name == "Capsule_Right" || gameObject.transform.Find("Capsule_Right") != null))
                {
                    parentObject = transform.parent.gameObject;
                    // 親子関係を解除
                    Transform[] childTransforms = parentObject.GetComponentsInChildren<Transform>();
                    // 親オブジェクトの子供オブジェクトの親子関係を無効にする
                    foreach (Transform childTransform in childTransforms)
                    {
                        if (childTransform != parentObject.transform)
                        {
                            // 子関係を解除
                            childTransform.parent = null;

                            // 親のオブジェクトのマテリアルを取得
                            Material parentMaterial = parentObject.GetComponent<MeshRenderer>().materials[0];
                            // 親のマテリアル情報を子オブジェクトのマテリアルに設定
                            MeshRenderer childMeshRenderer = childTransform.GetComponent<MeshRenderer>();
                            Material childMaterial = new Material(parentMaterial);
                            // childMeshRenderer.material = childMaterial;
                            childMeshRenderer.materials = new Material[]
                            {
                                // UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/Glass.mat"),
                                childMaterial,
                                UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/ClearGlass.mat")
                            };

                            // MeshColliderを凸に設定
                            MeshCollider meshCollider = childTransform.GetComponent<MeshCollider>();
                            if (meshCollider != null)
                            {
                                meshCollider.convex = true; // or false, depending on your needs
                                meshCollider.enabled = true;
                            }

                            // Rigidbodyをキネマティックに設定
                            Rigidbody rigidbody = childTransform.GetComponent<Rigidbody>();
                            if (rigidbody != null)
                            {
                                // audioSource = GetComponent<AudioSource>();
                                // audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/OpenCapsule"), 1.0f);
                                // GameObject prefabFromAssets = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 1/Prefab/Rocket 1.prefab"), new Vector3(0, 0, 0), Quaternion.identity);
                                GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/OpenCapsule"), 1.0f);
                                int number = UnityEngine.Random.Range(0, 10);
                                for (int i = 1; i < number + 1; i++)
                                {
                                    GameObject prefabHanabi = (GameObject)Instantiate(
                                    Resources.Load($"Prefabs/Fireworks/Rocket {i}"),
                                    // AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Fireworks/Rocket {i}/Prefab/Rocket {i}.prefab"), 
                                    new Vector3((number + 1) * 10, -20, (number + 1) * 10),
                                    Quaternion.identity);
                                }

                                rigidbody.isKinematic = false;

                                // 反発処理
                                if ((gameObject.name == "Capsule_Right" || gameObject.transform.Find("Capsule_Right") != null))
                                {
                                    rigidbody.AddForce(transform.forward * -200.0f, ForceMode.VelocityChange);
                                    rigidbody.AddForce(transform.up * -200.0f, ForceMode.VelocityChange);
                                }
                                if ((gameObject.name == "Capsule_Left" || gameObject.transform.Find("Capsule_Left") != null))
                                {
                                    rigidbody.AddForce(-transform.forward * -200.0f, ForceMode.VelocityChange);
                                    rigidbody.AddForce(transform.up * -200.0f, ForceMode.VelocityChange);
                                }


                            }
                        }
                    }

                    // 親オブジェクトを非アクティブにする（廃棄ではなく）
                    if (parentObject != null)
                    {
                        parentObject.SetActive(false);
                    }
                }

                return;
            }
        }
        // 前フレーム位置を更新
        _prevPosition = position;
    }

    private void Destroy(object value)
    {
        throw new NotImplementedException();
    }

    struct MeshData
    {
        public Vector3[] vertices;
        public int[] indices;
    }

    /// カプセルメッシュデータを作成  
    MeshData CreateCapsule(int divideH, int divideV, float height = 1f, float radius = 0.5f)
    {
        divideH = divideH < 4 ? 4 : divideH;
        divideV = divideV < 4 ? 4 : divideV;
        radius = radius <= 0 ? 0.001f : radius;

        // 偶数のみ有効  
        if (divideV % 2 != 0)
        {
            divideV++;
        }

        int cnt = 0;

        // 頂点座標作成  
        int vertCount = divideH * divideV + 2;
        var vertices = new Vector3[vertCount];

        // 中心角  
        float centerEulerRadianH = 2f * Mathf.PI / (float)divideH;
        float centerEulerRadianV = 2f * Mathf.PI / (float)divideV;

        float offsetHeight = height * 0.5f;

        // 天面  
        vertices[cnt++] = new Vector3(0, radius + offsetHeight, 0);

        // カプセル上部  
        for (int vv = 0; vv < divideV / 2; vv++)
        {
            var vRadian = (float)(vv + 1) * centerEulerRadianV / 2f;

            // 1辺の長さ  
            var tmpLen = Mathf.Abs(Mathf.Sin(vRadian) * radius);

            var y = Mathf.Cos(vRadian) * radius;
            for (int vh = 0; vh < divideH; vh++)
            {
                var pos = new Vector3(
                    tmpLen * Mathf.Sin((float)vh * centerEulerRadianH),
                    y + offsetHeight,
                    tmpLen * Mathf.Cos((float)vh * centerEulerRadianH)
                );
                // サイズ反映  
                vertices[cnt++] = pos;
            }
        }


        // カプセル下部  
        // int offset = (divideV / 2) - (int)_thickness;
        for (int vv = 0; vv < divideV / 2; vv++)
        {
            var yRadian = (float)(vv + 1) * centerEulerRadianV / 2f;

            // 1辺の長さ  
            var tmpLen = Mathf.Abs(Mathf.Sin(yRadian) * (radius - _thickness));

            var y = Mathf.Cos(yRadian) * (radius - _thickness);
            for (int vh = 0; vh < divideH; vh++)
            {
                var pos = new Vector3(
                    tmpLen * Mathf.Sin((float)vh * centerEulerRadianH),
                    y + offsetHeight,
                    tmpLen * Mathf.Cos((float)vh * centerEulerRadianH)
                );
                // サイズ反映  
                vertices[cnt++] = pos;
            }
        }




        // // 底面  
        // vertices[cnt] = new Vector3(0, -radius - offsetHeight, 0);
        // // インデックス配列作成  
        int topAndBottomTriCount = divideH * 2;
        // // 側面三角形の数  
        int aspectTriCount = divideH * (divideV - 2 + 1) * 2;

        int[] indices = new int[(topAndBottomTriCount + aspectTriCount) * 3];

        //天面  
        int offsetIndex = 0;
        cnt = 0;
        for (int i = 0; i < divideH * 3; i++)
        {
            if (i % 3 == 0)
            {
                indices[cnt++] = 0;
            }
            else if (i % 3 == 1)
            {
                indices[cnt++] = 1 + offsetIndex;
            }
            else if (i % 3 == 2)
            {
                var index = 2 + offsetIndex++;
                // 蓋をする  
                index = index > divideH ? indices[1] : index;
                indices[cnt++] = index;
            }
        }

        // // 側面Index  
        // 開始Index番号  
        int startIndex = indices[1];

        // 天面、底面を除いたカプセルIndex要素数  
        int sideIndexLen = aspectTriCount * 3;

        int lap1stIndex = 0;

        int lap2ndIndex = 0;

        // 一周したときのindex数  
        int lapDiv = divideH * 2 * 3;

        int createSquareFaceCount = 0;

        for (int i = 0; i < sideIndexLen; i++)
        {
            // 一周の頂点数を超えたら更新(初回も含む)  
            if (i % lapDiv == 0)
            {
                lap1stIndex = startIndex;
                lap2ndIndex = startIndex + divideH;
                createSquareFaceCount++;
            }

            if (i % 6 == 0 || i % 6 == 3)
            {
                indices[cnt++] = startIndex;
            }
            else if (i % 6 == 1)
            {
                indices[cnt++] = startIndex + divideH;
            }
            else if (i % 6 == 2 || i % 6 == 4)
            {
                if (i > 0 &&
                    (i % (lapDiv * createSquareFaceCount - 2) == 0 ||
                     i % (lapDiv * createSquareFaceCount - 4) == 0)
                )
                {
                    // 1周したときのClamp処理  
                    // 周回ポリゴンの最後から2番目のIndex  
                    indices[cnt++] = lap2ndIndex;
                }
                else
                {
                    indices[cnt++] = startIndex + divideH + 1;
                }
            }
            else if (i % 6 == 5)
            {
                if (i > 0 && i % (lapDiv * createSquareFaceCount - 1) == 0)
                {
                    // 1周したときのClamp処理  
                    // 周回ポリゴンの最後のIndex  
                    indices[cnt++] = lap1stIndex;
                }
                else
                {
                    indices[cnt++] = startIndex + 1;
                }

                // 開始Indexの更新  
                startIndex++;
            }
            else
            {
                Debug.LogError("Invalid : " + i);
            }
        }

        return new MeshData()
        {
            indices = indices,
            vertices = vertices
        };
    }

}