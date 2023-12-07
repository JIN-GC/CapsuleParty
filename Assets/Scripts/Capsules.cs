using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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

    [System.Obsolete]

    void Awake()
    {
        StartCoroutine(Connect());
        CapsulePrefabPack();
        CapsuleInstantiate();
    }

    [System.Obsolete]
    private IEnumerator Connect()
    {
        urls = new String[]
        {
            // 任意のWEBサイトから読み込む方法（現在MP3形式のみ対応）
            // https://morexlusive.com/camila-cabello-dont-go-yet-3/
            "https://www1.morexlusive.com/wp-content/uploads/2021/10/Camila_Cabello_-_Dont_Go_Yet.mp3",
            // "https://kamatamago.com/sozai/classic/c00001-c00100/C00016_kamatamago_Prelude-to-Act-1(Bullfighter).mp3"
            // https://classix.sitefactory.info/downmp3.html
            "https://classix.sitefactory.info/mp3classic/bizet/2464.mp3"
        };
        // string url = string.Join("", urls);

        number = UnityEngine.Random.Range(0, urls.Length);
        WWW www = new WWW(urls[number]);
        yield return www;
        audios = GetComponent<AudioSource>();
        audios.clip = www.GetAudioClip(false, true, mp3);   // 二つ目の引数がtrueで読込中の再生可能
        audios.Play();

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
