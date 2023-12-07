// C:\WorkPlace\Unity\Gacya\Gacya\Assets\UltimateXR\Art\Teleport\Targets\Round
// RoundParticleGeo.fbx
// Mod00Particle

// https://blog.narumium.net/2016/11/21/unity-c-スクリプトで円弧と筒を作成する/
// Unity C# スクリプトで円弧と筒を作成

// https://qiita.com/NEGO/items/448ea07f91fb9d4ef5e5
// unityでrayをcollider裏面に当てる方法

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ArkPipe : MonoBehaviour
{
	public int areaAngle  = 360;	// 円形角度
	public int startAngle =   0;	// スタート角度
	public int height     =  10;	// 高さ
	public int quality = 100;	//360degのときのtriangle数/2
	public bool isOutward = false;	// 内向き/外向き

	public bool isMeshRenderer = true; // 描画表示/非表示
	public Color color    = new Color(0.0f, 0.75f, 0.75f, 0.25f);	// RGBA
	public Vector3 scale  = new Vector3(10,1,10);  // スケール

	private Vector3[] vertices; //頂点
	private int[] triangles;    //index

	private void makeParams(){
			List<Vector3> vertList = new List<Vector3>();
			List<int> triList = new List<int>();

			float th,v1,v2;
			int max=(int)quality*areaAngle/360;

			for (int i=0;i<=max;i++){
					// // 60フレームに1度処理を実行する
					// if(Time.frameCount % 10 == 0)
					// {
							th=i*areaAngle/max + startAngle;
							v1=Mathf.Sin(th * Mathf.Deg2Rad);
							v2=Mathf.Cos(th * Mathf.Deg2Rad);
							vertList.Add(new Vector3(v1,0,v2));
							vertList.Add(new Vector3(v1,height,v2));
							if(i<=max-1){
									if(isOutward){
											triList.Add(2*i);triList.Add(2*i+3);triList.Add(2*i+1);
											triList.Add(2*i);triList.Add(2*i+2);triList.Add(2*i+3);
									}else{
											triList.Add(2*i);triList.Add(2*i+1);triList.Add(2*i+3);
											triList.Add(2*i);triList.Add(2*i+3);triList.Add(2*i+2);
									}
							}
					// }
			}
			vertices  = vertList.ToArray();
			triangles = triList.ToArray();
	}

	private void setParams(){
			Mesh mesh = new Mesh();

			mesh.vertices = vertices;
			mesh.triangles = triangles;

			// 法線とバウンディングの計算
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			mesh.name = "tubeMesh";
			transform.localScale = scale;

			GetComponent<MeshFilter>().sharedMesh = mesh;
			GetComponent<MeshCollider>().sharedMesh = mesh;
			
			// Convexを無効にする
			GetComponent<MeshCollider>().convex.Equals(true);

			// MeshRendererを取得
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

			// queriesHitBackfacesを有効にする
			meshRenderer.material.doubleSidedGI = true;

			// 色指定
			meshRenderer.material.color = color;

			// マテリアルをResourcesフォルダからロード
			Material[] mats = meshRenderer.materials;

			// Mesh Renderer: Materials: Element0にマテリアル追加
			Material glassMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Chasses/Glass.mat");
			mats[0] = glassMaterial;

			// Mesh Renderer: Materials: Element[]のマテリアル配列を設定
			meshRenderer.materials = mats;

			// 両面にシャドウ投影
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

			// Convexを無効にする
			if (!isMeshRenderer) GetComponent<MeshRenderer>().enabled = false;
	}

	void Start()
	{
			makeParams();
			setParams();
	}
}
