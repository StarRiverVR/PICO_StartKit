using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    public class DictionaryPool : CSingleton<DictionaryPool>, IDisposable
    {

        #region//Dictionary<stirng,string>

        SimpleDictionaryPool<Dictionary<string, string>, string, string> dicStringStringPool = new SimpleDictionaryPool<Dictionary<string, string>, string, string>();

        public Dictionary<string, string> GetOneStringStringDic()
        {
            return dicStringStringPool.Spawn();
        }

        public void PutBackOneStringStringDic(Dictionary<string, string> dic)
        {
            dicStringStringPool.Recycle(dic);
        }

        #endregion

        #region//Dictionary<stirng,object>

        SimpleDictionaryPool<Dictionary<string, object>, string, object> dicStringObjectPool = new SimpleDictionaryPool<Dictionary<string, object>, string, object>();

        public Dictionary<string, object> GetOneStringObjectDic()
        {
            return dicStringObjectPool.Spawn();
        }

        public void PutBackOneStringObjectDic(Dictionary<string, object> dic)
        {
            dicStringObjectPool.Recycle(dic);
        }

        #endregion

        public void Dispose()
        {
            dicStringStringPool.Clear();
            dicStringObjectPool.Clear();
        }
    }

}


