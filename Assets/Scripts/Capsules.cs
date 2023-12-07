using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]
public class Capsules : MonoBehaviour
{
    AudioSource audios;

    public AudioType mp3;
    public string[] urls;

    public GameObject[] Prefabs;

    public List<Vector3> Vector3s = new List<Vector3>();
    private int number;
    int PrefabsExistCnt;


    void Awake()
    {
        CapsulePrefabPack();
        CapsuleInstantiate();
    }

    void Start()
    {
        StartCoroutine(CheckStatus());
    }


    IEnumerator CheckStatus()
    {
        yield return new WaitForSeconds(120.0f);
        PrefabsExistCnt = Prefabs.Length;
        while (PrefabsExistCnt > 0)
        {
            for (int i = 0; i < Prefabs.Length; i++)
            {
                // Debug.Log($"PrefabsExistCnt {PrefabsExistCnt} Prefabs[i].transform.childCount : {Prefabs[i].transform.childCount}");
                if (Prefabs[i].transform.childCount == 0) PrefabsExistCnt--;
                if (Prefabs[i].transform.childCount != 0 && Prefabs[i].transform.position.y < 50) PrefabsExistCnt--;
                // foreach (Transform child in Prefabs[i].transform) Destroy(child.gameObject);
            }
            yield return new WaitForSeconds(1.0f);
        }
        SceneManager.LoadScene("End");
    }


    private void CapsulePrefabPack()
    {
        Prefabs = new GameObject[]
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

        Vector3s = new List<Vector3>()
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

    public void CapsuleInstantiate()
    {
        var parent = this.transform;
        number = UnityEngine.Random.Range(0, Prefabs.Length);
        for (int i = 0; i < Prefabs.Length; i++)
        {
            if (number + i < Prefabs.Length)
            {
                Instantiate(Prefabs[number + i], Vector3s[number + i], Quaternion.identity, parent);
            }
            else
            {
                Instantiate(Prefabs[(i + number) - Prefabs.Length], Vector3s[(i + number) - Prefabs.Length], Quaternion.identity, parent);
            }

        }
    }
}
