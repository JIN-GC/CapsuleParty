using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]
public class Capsules : MonoBehaviour
{
    [SerializeField] private GameObject[] p_PrefabList;
    [SerializeField] private Vector3[] p_VectorList;
    private static int p_CntPrefabs = 0;
    private int p_RandNum;

    public static void SetPrefabsCheckCnt(){p_CntPrefabs++;} // 設定用関数
    private void Awake()
    {
        CapsulePrefabList();
        CapsuleInstantiate();
    }
    private void Start(){StartCoroutine(CheckStatus());}
    private IEnumerator CheckStatus()
    {
        int cnt = 1;
        yield return new WaitForSeconds(90.0f);
        // p_CntPrefabs = p_PrefabList.Length;
        while (p_CntPrefabs < p_PrefabList.Length)
        {
            for (int i=0; i < p_PrefabList.Length; i++) if (p_PrefabList[i].transform.tag == "Finish") ++cnt;
            // for (int i = 0; i < p_PrefabList.Length; i++) if (p_PrefabList[i].transform.tag == "Finish") p_CntPrefabs--;
            yield return new WaitForSeconds(5.0f);
        }
        if (p_CntPrefabs != cnt) Debug.Log($"[@Error] cnt: {cnt}, p_CntPrefabs: {p_CntPrefabs}");
        SceneManager.LoadScene("End");
    }
    
    private void CapsulePrefabList()
    {
        p_PrefabList = new GameObject[]
        {
            // (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Case"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Goryokaku"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Louve"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_Sakura"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_ArcTriomphe"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_PyramidAndCamel"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_StoneHenge"),
            (GameObject)Resources.Load("Prefabs/Capsules/Capsule_TokyoTower")

            // 任意のフォルダから読み込む方法
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Case.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Goryokaku.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Louve.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_Sakura.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_ArcTriomphe.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_PyramidAndCamel.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_StoneHenge.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Capsules/Capsule_TokyoTower.prefab")
        };

        p_VectorList = new Vector3[]
            {
                // new Vector3(30, 700, 0),
                new Vector3(-30, 700, 10),
                new Vector3(10, 800, -30),
                new Vector3(0, 900, 30),
                new Vector3(-20, 900, -30),
                new Vector3(30, 950, -10),
                new Vector3(-30, 950, 20),
                new Vector3(10, 1000, -20)
            };
    }

    private void CapsuleInstantiate()
    {
        GameObject prefabTemp; //  p_PrefabList配列シャッフル用変数
        Vector3 vectorTemp; //  p_VectorList配列シャッフル用変数
        for (int i=0; i < p_PrefabList.Length; i++)
        {
            p_RandNum = UnityEngine.Random.Range(0, i + 1); // Fisher-Yatesシャッフルアルゴリズム
            prefabTemp = p_PrefabList[i];
            p_PrefabList[i] = p_PrefabList[p_RandNum];
            p_PrefabList[p_RandNum] = prefabTemp;
            vectorTemp = p_VectorList[p_RandNum];
            p_VectorList[p_RandNum] = p_VectorList[i];
            p_VectorList[i] = vectorTemp;
        }

        p_RandNum = UnityEngine.Random.Range(0, p_PrefabList.Length); // シャッフル後にPrefab生成
        var parentTransform = this.transform;
        for (int i = 0; i < p_PrefabList.Length; i++) Instantiate(p_PrefabList[i], p_VectorList[i], Quaternion.identity, parentTransform);
    }
}
