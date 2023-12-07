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

public class AudioController : MonoBehaviour
{
    AudioSource audios;

    public AudioType mp3;
    public string[] urls;

    private int number;

    void Awake()
    {
        StartCoroutine(Connect());
    }

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
        // Sceneを遷移しても音楽が消えないようにする
        DontDestroyOnLoad(audios);
    }

}
