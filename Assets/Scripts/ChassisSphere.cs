using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

public class ChassisSphere : MonoBehaviour
{

    public enum ColliderShape { Cube, Sphere, Capsule, Cylinder }
    public ColliderShape colliderShape;
    public float colliderSize = 30.0f;
    public float interval = 5.0f;
    public float impactSize = 10.0f;
    GameObject colliderObject;
    GameObject colliderSubObject;

    void Start()
    {
        SelectShape();
        // バリケード範囲サイズ(colliderSize)を数分後に段階的縮小
        // StartCoroutine(ReduceBarricadeRange());
    }

    void SelectShape()
    {
        // オブジェクト形状選択と生成
        // (colliderObject, colliderSubObject) = colliderShape switch
        (colliderObject) = colliderShape switch
        {
            ColliderShape.Cube => (
                // GameObject.CreatePrimitive(PrimitiveType.Cube),
                GameObject.CreatePrimitive(PrimitiveType.Cube)
            ),
            ColliderShape.Sphere => (
                // GameObject.CreatePrimitive(PrimitiveType.Sphere),
                GameObject.CreatePrimitive(PrimitiveType.Sphere)
            ),
            ColliderShape.Cylinder => (
                // GameObject.CreatePrimitive(PrimitiveType.Cylinder),
                GameObject.CreatePrimitive(PrimitiveType.Cylinder)
            ),
            ColliderShape.Capsule => (
                // GameObject.CreatePrimitive(PrimitiveType.Capsule),
                GameObject.CreatePrimitive(PrimitiveType.Capsule)
            ),
            _ => (
                // GameObject.CreatePrimitive(PrimitiveType.Cube),
                GameObject.CreatePrimitive(PrimitiveType.Cube)
            )
        };

        colliderObject.name = ("colliderObject");
        colliderObject.transform.position = transform.position;
        colliderObject.transform.SetParent(transform);

        // colliderSubObject.name = ("colliderSubObject");
        // colliderSubObject.transform.SetParent(transform);
        float modifyPosY = 0.0f;
        switch (colliderShape)
        {
            case ColliderShape.Cube:
                colliderObject.transform.localScale = new Vector3(colliderSize * 1.25f, colliderSize * 1.25f, colliderSize * 1.25f);
                colliderObject.transform.Rotate(135.0f, 0.0f, -135.0f);
                modifyPosY = colliderSize / 10;
                break;
            case ColliderShape.Cylinder:
                colliderObject.transform.localScale = new Vector3(colliderSize * 1.5f, colliderSize / 2.0f, colliderSize * 1.5f);
                colliderObject.transform.Rotate(90.0f, 0.0f, 90.0f);
                modifyPosY = colliderSize / 6;
                break;
            case ColliderShape.Capsule:
                colliderObject.transform.localScale = new Vector3(colliderSize * 1.25f, colliderSize / 1.05f, colliderSize * 1.25f);
                colliderObject.transform.Rotate(90.0f, 0.0f, 90.0f);
                modifyPosY = colliderSize / 4;
                break;
            case ColliderShape.Sphere:
                colliderObject.transform.localScale = new Vector3(colliderSize * 1.75f, colliderSize * 1.75f, colliderSize * 1.75f);
                modifyPosY = colliderSize / 6;
                break;
            default:
                break;
        }
        //  colliderObjectの位置を上方向 (Vector3.up) に modifyPosY 分だけ移動
        colliderObject.transform.position += Vector3.down * modifyPosY;

        InverseMesh();
        FireDistort();

        StartCoroutine(ObjectColliderInactivate(colliderObject));

    }

    void InverseMesh()
    {
        // Rendererを破棄
        // Destroy(colliderObject.GetComponent<Renderer>());

        Collider collider = colliderObject.GetComponent<Collider>();
        collider.isTrigger = true;

        Mesh mesh = colliderObject.GetComponent<MeshFilter>().mesh;

        int[] triangles = mesh.triangles;
        System.Array.Reverse(triangles);

        mesh.triangles = triangles;
        // MeshCollider を colliderObject に追加
        colliderObject.AddComponent<MeshCollider>();

    }

    void FireDistort()
    {
        // MeshRenderer を colliderObject に追加
        MeshRenderer meshRenderer = colliderObject.GetComponent<MeshRenderer>();
        // マテリアルをResourcesフォルダからロード
        Material[] mats = meshRenderer.materials;
        // Mesh Renderer: Materials: Element0 にマテリアル追加 (ファイルパスからCustom/Glass05シェードが適用されたマテリアルをロード)
        mats[0] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/ClearGlass.mat");
        // Mesh Renderer: Materials: Element0 にマテリアル追加 (Assets/Resources からロード *フォルダ内に存在している場合のみ)
        // mats[0] = Resources.Load<Material>("Glass");
        // Mesh Renderer: Materials: Element0 にマテリアル追加 (シェダーからマテリアルを生成)
        // mats[0] = new Material(Shader.Find("Custom/Glass00"));
        // Mesh Renderer: Materials: Element[] のマテリアル配列を設定
        meshRenderer.materials = mats;
        // マテリアルにシェーダーパラメータを設定する
        meshRenderer.material.SetColor("_Color", new Color(0.75f, 1.0f, 1.0f, 1.0f)); // 青色設定(ガラス系)
        // meshRenderer.material.color = new Color(1.0f, 0.2f, 0.2f, 1.0f); // 赤色設定(ガラス系)
    }

    void DisableMesh()
    {
        if (colliderObject.gameObject != null)
        {
            Destroy(colliderObject.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // バリケードに接触したアイテムの排除
        // Inboundタグのオブジェクトを数秒後に非アクティブ化
        if (other.gameObject.CompareTag("Inbound"))
        {
            StartCoroutine(ObjectInactivate(other));
        }

        // Debug.Log($"other collider name = {other.name} ");
        if (other.gameObject.CompareTag("Finish"))
        {
            StartCoroutine(OtherObjectColliderInactivate(other));
        }
    }

    IEnumerator ObjectColliderInactivate(GameObject gameObject)
    {
        // Debug.Log($" main collider name = {gameObject.name} ");
        // 指定した秒数だけ処理を待つ(15秒)
        yield return new WaitForSeconds(15.0f);

        // オブジェクトコライダーを非アクティブ化(解放)
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        // meshCollider.isTrigger = false;
        meshCollider.enabled = false;
        yield return new WaitForSeconds(2.0f);

        // オブジェクトコライダーをアクティブ化(閉鎖)
        // meshCollider.isTrigger = true;
        meshCollider.enabled = true;
        yield return new WaitForSeconds(7.0f);

        // オブジェクトコライダーを非アクティブ化(解放)
        // meshCollider.isTrigger = false;
        meshCollider.enabled = false;
        yield return new WaitForSeconds(1.8f);

        // オブジェクトコライダーをアクティブ化(閉鎖)
        // meshCollider.isTrigger = true;
        meshCollider.enabled = true;
        yield return new WaitForSeconds(10.0f);

        // オブジェクトコライダーを非アクティブ化(解放)
        // meshCollider.isTrigger = false;
        meshCollider.enabled = false;
        yield return new WaitForSeconds(2.0f);

        // オブジェクトコライダーをアクティブ化(閉鎖)
        // meshCollider.isTrigger = true;
        meshCollider.enabled = true;
        yield return new WaitForSeconds(10.0f);

        // オブジェクトコライダーを非アクティブ化(解放)
        // meshCollider.isTrigger = false;
        meshCollider.enabled = false;

    }

    IEnumerator ObjectInactivate(Collider other)
    {
        // 指定した秒数だけ処理を待つ(2.5秒)
        yield return new WaitForSeconds(2.5f);
        // オブジェクトを非アクティブ化
        other.gameObject.SetActive(false);
    }

    IEnumerator OtherObjectColliderInactivate(Collider other)
    {
        Debug.Log($"2 other collider name = {other.name} ");
        // 指定した秒数だけ処理を待つ(15秒)
        yield return new WaitForSeconds(15.0f);

        // オブジェクトコライダーを非アクティブ化
        // MeshCollider otherMeshCollider = other.gameObject.GetComponent<MeshCollider>();
        other.isTrigger = true;
        // otherMeshCollider.enabled = false;

        yield return new WaitForSeconds(2.5f);
        other.isTrigger = false;
        // otherMeshCollider.enabled = true;

    }

    IEnumerator ReduceBarricadeRange()
    {
        // 指定した秒数だけ処理を待つ
        yield return new WaitForSeconds(5.0f);
        // yield return new WaitForMinutes(3);
        while (colliderSize > impactSize)
        {
            // Debug.Log("colliderSize");
            // DisableMesh();
            colliderSize -= 0.1f;
            InverseMesh();
        }
    }

    // void OnGUI()
    // {
    //     if (hasCollided)
    //     {
    //         GUILayout.Label("Collision Object: " + collisionObjectName);
    //         GUILayout.Label("Collision Object Type: " + collisionObjectType);
    //     }
    // }

}
