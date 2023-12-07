using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ArkPanel : MonoBehaviour
{
    public int areaAngle = 60;  // 弧度
    public int startAngle = 0;  // 開始角度
    public float height = 50;   // 高さ
    public int divideCount = 3; // 分割係数
    public int quality = 100;   // 360deg時のTriangle数
    public Color color = new Color(0.75f, 0.0f, 0.0f, 0.5f);  // RGBA
    public Vector3 scale = new Vector3(20, 1, 20);  // 大きさ
    private List<List<Vector3>> vertices = new List<List<Vector3>>();   // 円形座標管理用2次元配列
    private List<List<int>> triangles = new List<List<int>>();  // 重複した三角部削除用2次元配列
    void makeParams(int cnt)
    {
        // 円形座標生成用2次元配列
        List<Vector3> vertSubList = new List<Vector3>();
        // 重複した三角部削除用2次元配列
        List<int> triSubList = new List<int>();

        // 弧度の基準座標を設定
        vertSubList.Add(new Vector3(0, 0, 0));

        float th, v1, v2;
        // th用 弧度座標の最大値を算出
        int max = (int)quality * areaAngle / 360;

        for (int j = 0; j <= max; j++)
        {
            // 弧部分の座標を計算
            th = j * areaAngle / max + startAngle;
            v1 = Mathf.Sin(th * Mathf.Deg2Rad);
            v2 = Mathf.Cos(th * Mathf.Deg2Rad);

            vertSubList.Add(new Vector3(v1, 0, v2));
            vertSubList.Add(new Vector3(v1, height, v2));

            if (j <= max - 1)
            {
                triSubList.Add(0);
                triSubList.Add(j * 2 + 1);
                triSubList.Add((j + 1) * 2 + 1);
            }
        }

        // vertices と triangles に追加
        vertices.Add(new List<Vector3>(vertSubList));
        triangles.Add(new List<int>(triSubList));
    }

    private void setParams(int cnt)
    {
        List<Mesh> meshList = new List<Mesh>();
        // Mesh mesh = new Mesh();

        // Mesh mesh = new Mesh();
        // mesh.vertices = vertices[cnt].ToArray();

        // Check if meshList has enough elements
        while (meshList.Count <= cnt)
        {
            meshList.Add(new Mesh());
        }

        // Assign mesh to the current index
        Mesh mesh = meshList[cnt];
        mesh.vertices = vertices[cnt].ToArray();
        mesh.triangles = triangles[cnt].ToArray();

        // 法線とバウンディングの計算
        // Calculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshList[cnt].name = "arcMesh";
        // Create an empty GameObject for each panel
        GameObject arkPanelObject = new GameObject("arkPanel" + cnt);

        arkPanelObject.transform.localScale = scale;

        //  colliderObjectの位置を上方向 (Vector3.up) に height 分だけ移動
        arkPanelObject.transform.position += Vector3.up * height; 

        // arkPanelObject.transform.SetParent(null); // 親オブジェクトをnullに設定（親オブジェクトなしになる）
        arkPanelObject.transform.SetParent(transform);

        // Set the new mesh for MeshFilter and MeshCollider of the panel
        arkPanelObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        arkPanelObject.AddComponent<MeshCollider>().sharedMesh = mesh;

        // arkPanelObject.AddComponent<MeshRenderer>().material.color = color;  // 色指定
        // MeshRenderer を colliderObject に追加
        MeshRenderer meshRenderer = arkPanelObject.AddComponent<MeshRenderer>();
        // MeshRendererを取得
        // MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // 色指定
		// meshRenderer.material.color = color;

        // マテリアルをResourcesフォルダからロード
        Material[] mats = meshRenderer.materials;
        // Mesh Renderer: Materials: Element0 にマテリアル追加 (ファイルパスからCustom/Glass05シェードが適用されたマテリアルをロード)
        // mats[0] = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/Glass.mat");
        mats[0] = new Material(Shader.Find("Custom/Glass05"));
        // Mesh Renderer: Materials: Element[] のマテリアル配列を設定
        meshRenderer.materials = mats;
        
    }

    void Start()
    {

        int cntEnd = Mathf.RoundToInt(height/divideCount);
        // スタート角度基準
        int baseAngle = startAngle;

        for (int cnt = 0; cnt < cntEnd; cnt++)
        {
            // 開始角度と高さ更新
            startAngle = baseAngle + (cnt * 15);
            height = height - (height / cntEnd * cnt /(divideCount * 4));
            makeParams(cnt);
            setParams(cnt);
        }
    }

    internal void CreateArkPanel()
    {
        throw new NotImplementedException();
    }
}
