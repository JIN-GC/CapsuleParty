using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEditor;

[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]

public class CapsuleCover : MonoBehaviour
{

    // 配列型：複数の音声クリップ（スクリプトアタッチ）
    [SerializeField] public AudioClip[] clips;
    // オーディオソースコンポーネント（インスペクターアタッチ）
    private AudioSource audioSource;
    public GameObject[] Prefabs;
    private int number;

    void Awake()
    {
        //Loading the items into the array
        clips = new AudioClip[]
        {
            // Resourcesフォルダから読み込む方法
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs00"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs01"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs02"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs03"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs04"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs05")
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs06")
        };

        Prefabs = new GameObject[]
        {
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 1"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 2"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 3"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 4"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 5")
            // // 任意のフォルダから読み込む方法
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 1/Prefab/Rocket 1.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 2/Prefab/Prefab 2.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 3/Prefab/Prefab 3.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 4/Prefab/Prefab 4.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 5/Prefab/Prefab 5.prefab")
        };

        Hanabi_0();
    }


    void Start()
    {
        
    }

    // public float gravity = 9.8f; // 重力の強さ

    void Update()
    {
        // // 重力の方向を設定
        // Vector3 gravityDirection = (transform.position - Camera.main.transform.position).normalized;
        // Physics.gravity = gravity * gravityDirection;

        int ObjCount = this.transform.childCount;
        if (ObjCount == 0)
        {
            Debug.Log("targetがDestroyされました");
            Sound_0();
        }

        //         this.OnDestroyed.AddListener(()=>{
        //             Debug.Log("targetがDestroyされました");
        // 　　　　　　 // ここに処理を追加
        //         });
    }
    void PlayClipSound()
    {
        // AudioSource コンポーネントの取得
        audioSource = GetComponent<AudioSource>();
        // clips配列のセットMusicからランダムに選択
        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        // ボリューム
        audioSource.volume = 0.1f;
        // 再生
        audioSource.Play();
    }
        void OnTriggerEnter()
    {
        // OnTriggerEnterで判定する場合は、Coliderコンポーネント内のisTriggerにチェックをつけて置く必要があります。 衝突した時の影響はなくなるので、すり抜ける時
        PlayClipSound();
    }


    void OnCollisionEnter()
    {
        // Coliderコンポーネント内のisTriggerにチェックは外します。 そして、反発させるためには、判定したいオブジェクトにRigiBodyコンポーネントを追加して置く必要があります。
        PlayClipSound();
    }

        public void Hanabi_0()
    {
        {
            // number = UnityEngine.Random.Range(0, Prefabs.Length);
            // GameObject stageObject = (GameObject)Instantiate(
            // Prefabs[number],
            // new Vector3(0, 0, 0),
            // Quaternion.identity);

            // Instantiate(Prefabs[number], new Vector3(0, 0, 0), Quaternion.identity);
            // number = Random.Range(0, Prefabs.Length);
            number = UnityEngine.Random.Range(0, Prefabs.Length);
            for (int i=0; i<number; i++){
                Instantiate(Prefabs[i], new Vector3(i, i, i), Quaternion.identity, transform.parent);
                // Debug.Log($"number: {number} Prefabs[number]: {Prefabs[number].name.ToSafeString()} ");
            }

        }
    }

    public void Sound_0()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/OpenCapsule"), 1.0f);
    }
    public void Sound_1()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clips[1]);
    }
    public void Sound_2()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clips[2]);
    }
    public void Sound_3()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clips[3]);
    }
    public void Sound_4()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clips[4]);
    }
    public void Sound_5()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clips[5]);
    }
    public void Sound_6()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clips[6]);
    }

}
