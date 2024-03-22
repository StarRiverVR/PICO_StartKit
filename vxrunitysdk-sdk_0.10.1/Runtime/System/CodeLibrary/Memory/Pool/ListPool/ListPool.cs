using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    public class ListPool : CSingleton<ListPool>, IDisposable
    {

        #region//List<string>

        SimpleListPool<List<string>, string> stringListPool = new SimpleListPool<List<string>, string>();

        /// <summary>
        /// string List
        /// </summary>
        /// <returns></returns>
        public List<string> GetOneStringList()
        {
            return stringListPool.Spawn();
        }

        /// <summary>
        /// string List
        /// </summary>
        /// <param name="list"></param>
        public void PutBackOneStringList(List<string> list)
        {
            stringListPool.Recycle(list);
        }

        #endregion

        #region // List<object>

        SimpleListPool<List<object>, object> objectListPool = new SimpleListPool<List<object>, object>();

        public List<object> GetOneObjectList()
        {
            return objectListPool.Spawn();
        }

        public void PutBackOneObjectList(List<object> list)
        {
            objectListPool.Recycle(list);
        }

        #endregion

        #region // List<byte[]>

        SimpleListPool<List<byte[]>, byte[]> bytesListPool = new SimpleListPool<List<byte[]>, byte[]>();

        public List<byte[]> GetOneBytesList()
        {
            return bytesListPool.Spawn();
        }

        public void PutBackOneBytesList(List<byte[]> list)
        {
            bytesListPool.Recycle(list);
        }

        #endregion

        #region // List<byte>

        SimpleListPool<List<byte>, byte> byteListPool = new SimpleListPool<List<byte>, byte>();

        public List<byte> GetOneByteList()
        {
            return byteListPool.Spawn();
        }

        public void PutBackOneByteList(List<byte> list)
        {
            byteListPool.Recycle(list);
        }

        #endregion

        #region // List<int>

        SimpleListPool<List<int>, int> intListPool = new SimpleListPool<List<int>, int>();

        public List<int> GetOneIntList()
        {
            return intListPool.Spawn();
        }

        public void PutBackOneIntList(List<int> list)
        {
            intListPool.Recycle(list);
        }

        #endregion

        #region // List<long>

        SimpleListPool<List<long>, long> longListPool = new SimpleListPool<List<long>, long>();

        public List<long> GetOneLongList()
        {
            return longListPool.Spawn();
        }

        public void PutBackOnelongList(List<long> list)
        {
            longListPool.Recycle(list);
        }

        #endregion

        #region // List<Texture2D>


        SimpleListPool<List<Texture2D>, Texture2D> texture2DListPool = new SimpleListPool<List<Texture2D>, Texture2D>();

        public List<Texture2D> GetOneTexture2DList()
        {
            return texture2DListPool.Spawn();
        }

        public void PutBackOneTexture2DList(List<Texture2D> list)
        {
            texture2DListPool.Recycle(list);
        }

        #endregion

        #region // List<Collider>


        SimpleListPool<List<Collider>, Collider> colliderListPool = new SimpleListPool<List<Collider>, Collider>();

        public List<Collider> GetOneColliderList()
        {
            return colliderListPool.Spawn();
        }

        public void PutBackOneColliderList(List<Collider> list)
        {
            colliderListPool.Recycle(list);
        }

        #endregion

        public void Dispose()
        {
            stringListPool.Clear();
            objectListPool.Clear();
            bytesListPool.Clear();
            texture2DListPool.Clear();
            byteListPool.Clear();
            intListPool.Clear();
            colliderListPool.Clear();
            longListPool.Clear();
        }

    }
}


