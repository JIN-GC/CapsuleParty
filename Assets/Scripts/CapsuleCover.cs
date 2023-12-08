using UnityEngine;

[RequireComponent(typeof(AudioClip))]
[RequireComponent(typeof(AudioSource))]
public class CapsuleCover : MonoBehaviour
{
    [SerializeField] private AudioClip[] p_AudioClipList;  // AudioClip配列
    private AudioSource p_AudioSource;    // AudioSource変数
    private GameObject[] p_PrefabList;
    private int p_RandNum;
    private void Awake()
    {
        
        p_AudioClipList = new AudioClip[]
        {
            // Resourcesフォルダからメディア読込
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs00"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs01"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs02"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs03"),
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs04"),
            (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs05")
            // (AudioClip)Resources.Load("Sounds/GlassStairs/GlassStairs06")
        };

        p_PrefabList = new GameObject[]
        {
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 1"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 2"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 3"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 4"),
            (GameObject)Resources.Load("Prefabs/Fireworks/Rocket 5")
            // // 任意のフォルダからPrefab読込
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 1/Prefab/Rocket 1.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 2/Prefab/Prefab 2.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 3/Prefab/Prefab 3.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 4/Prefab/Prefab 4.prefab"),
            // AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fireworks/Rocket 5/Prefab/Prefab 5.prefab")
        };
        Hanabi();
    }


    // public float Gravity = 9.8f; // 重力の強さ

    private void Update()
    {
        // // 重力の方向を設定
        // Vector3 gravityDirection = (transform.position - Camera.main.transform.position).normalized;
        // Physics.gravity = Gravity * gravityDirection;

        // if (transform.childCount == 0) Sound_Open();
    }
    private void PlayClipSound()
    {
        // AudioSource コンポーネントの取得
        p_AudioSource = GetComponent<AudioSource>();
        // p_AudioClipList配列のセットMusicからランダムに選択
        p_AudioSource.clip = p_AudioClipList[UnityEngine.Random.Range(0, p_AudioClipList.Length)];
        // ボリューム
        p_AudioSource.volume = 0.1f;
        // 再生
        p_AudioSource.Play();
    }
    private void OnTriggerEnter(){PlayClipSound();} // ColiderのisTriggerのチェックをONにし衝突判定を無効化(すり抜ける時)
    private void OnCollisionEnter(){PlayClipSound();}   // ColiderのisTriggerのチェックを外し接触判定有効化(対RigiBodyオブジェクト用)
    public void Hanabi()
    {
        // Instantiate(p_PrefabList[p_RandNum], new Vector3(0, 0, 0), Quaternion.identity);
        p_RandNum = UnityEngine.Random.Range(0, p_PrefabList.Length);
        for (int i=0; i<p_RandNum; i++) Instantiate(p_PrefabList[i], new Vector3(i, i, i), Quaternion.identity, transform.parent);
    }

    private void Sound_Open()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/OpenCapsule"), 1.0f);
    }
    public void Sound_0()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(p_AudioClipList[1]);
    }

    public void Sound_1()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(p_AudioClipList[1]);
    }
    public void Sound_2()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(p_AudioClipList[2]);
    }
    public void Sound_3()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(p_AudioClipList[3]);
    }
    public void Sound_4()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(p_AudioClipList[4]);
    }
    public void Sound_5()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(p_AudioClipList[5]);
    }
    public void Sound_6()
    {
        p_AudioSource = GetComponent<AudioSource>();
        p_AudioSource.PlayOneShot(p_AudioClipList[6]);
    }
}
