using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioManager
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        // インスタンス
        private static T instance;
        // インスタンスを外部から参照する用(getter)
        public static T Instance
        {
            get
            {
                //インスタンスがまだ作られていない
                if (instance == null)
                {
                    // シーン内からインスタンスを取得
                    Type t = typeof(T);
                    instance = (T)FindObjectOfType(t);
                    // シーン内に存在しない場合はエラー
                    if (instance == null)
                    {
                        Debug.LogError(t + "is Not Found");
                    }
                }
                return instance;
            }
        }

        virtual protected void Awake()
        {
            CheckInstance();
        }
        protected bool CheckInstance()
        {
            if (instance == null)
            {
                instance = this as T;
                return true;
            }
            else if (Instance == this)
            {
                return true;
            }
            Destroy(this);
            return false;
        }
    }
}