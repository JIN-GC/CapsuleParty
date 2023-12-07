using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkPanels : MonoBehaviour
{
    public GameObject ArkPanelPrefab; // ArkPanelのPrefabをアタッチ
    void Start()
    {
        for (int cnt = 0; cnt < 4; cnt++)
        {
            // スタート地点の角度と高さの更新
            int startAngle = 1 + (cnt * 45);
            float height = 200 - (cnt * 10);

            // 新しい空のオブジェクトを生成
            GameObject arkBoxObject = Instantiate(ArkPanelPrefab, Vector3.zero, Quaternion.identity);

            // アタッチされたArkBoxスクリプトのパラメータを設定
            ArkPanel arkBoxScript = arkBoxObject.GetComponent<ArkPanel>();
            arkBoxScript.startAngle = startAngle;
            arkBoxScript.height = height;

            // 新しいオブジェクトに対して描画を行う
            arkBoxScript.CreateArkPanel();
        }
    }


}
