using System;
using UnityEngine;

namespace AudioManager
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;  // インスタンス
        public static T Instance    // インスタンスを外部から参照する用(getter)
        {
            get {
                if (instance == null)   //インスタンスがまだ作られていない
                {
                    Type t = typeof(T); // シーン内からインスタンスを取得
                    instance = (T)FindObjectOfType(t);
                    if (instance == null) Debug.LogError(t + "is Not Found");  // シーン内に存在しない場合はエラー
                }
                return instance;
            }
        }

        virtual protected void Awake(){CheckInstance();}
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