
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.codelibrary;


/// <summary>
/// 网络协议生成数据
/// 使用方法：
/// 1:使用Proto.Serialize();返回数据的byte[],用于网络发送
/// 2:使用Proto.Split_The_Parcel();将接收到的网络数据进行拆解
/// 3:将拆解出来的Proto_Package_Data数据使用Proto_Package_Data.GetData();进行解析
/// </summary>
namespace proto
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class Proto
    {
#if UNITY_EDITOR
        static Proto()
        {
            UnityEditor.EditorApplication.delayCall += DoSomethingPrepare;
        }

        static void DoSomethingPrepare()
        {
            if (!Application.isPlaying)
            {
                Init();
            }
        }
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            Init();
        }

        static Int32 sizeOfInt16;

        public static Int32 SizeOfInt16
        {
            get
            {
                return sizeOfInt16;
            }
        }

        static Int32 sizeOfInt32;

        public static Int32 SizeOfInt32
        {
            get
            {
                return sizeOfInt32;
            }
        }

        static Int32 sizeOfInt64;

        public static Int32 SizeOfInt64
        {
            get
            {
                return sizeOfInt64;
            }
        }

        static Int32 sizeOfBool;

        public static Int32 SizeOfBool
        {
            get
            {
                return sizeOfBool;
            }
        }

        static void Init()
        {
            sizeOfInt16 = sizeof(Int16);
            sizeOfInt32 = sizeof(Int32);
            sizeOfInt64 = sizeof(Int64);
            sizeOfBool = sizeof(bool);
            stringBytes.Clear();
            SetOneStringBytes("");
            SetOneStringBytes("常用字符串缓存");
        }

        /// <summary>
        /// //1:小端模式 2:大端模式
        /// </summary>
        static Int16 sysEndian = -1;

        public static Int16 SysEndian
        {
            get {
                return sysEndian;
            }
        }

        /// <summary>
        /// 强制使用的端排列顺序 1:小端模式 2:大端模式
        /// </summary>
        static Int16 endian = 1;

        public static Int16 Endian
        {
            get
            {
                return endian;
            }
        }

        static void InitEndian(){
            if (SysEndian==-1){
                Int16 num = 1;
                byte[] b= BitConverterBytes_Int16(num);
                if (b[0]==1){sysEndian = 1;}else{sysEndian = 2;}
            }
        }

        /// <summary>
        /// 将传入的结构序列化成网络数据
        /// </summary>
        /// <param name="obj">要序列化的类</param>
        /// <param name="agreement">协议号</param>
        /// <returns></returns>
        public static ByteBufferData Serialize(Proto_Base obj, Int32 agreement)
        {
            Proto.InitEndian();
            ByteBufferData objData = obj.Serialize();//已经排序 不需要进行大小端判断
            Int32 len = objData.Length;
            Int16 num_Endian = (Int16)((Proto.Endian == 1) ? 257 : 0);
            byte[] endian= null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int16(num_Endian, ref endian);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(endian);
            }
            byte[] head = null;
            ByteBufferData tempData2 = Obj_Bytes_Minus_Int32(len + SizeOfInt32, ref head);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(head);
            }
            byte[] agr = null;
            ByteBufferData tempData3 = Obj_Bytes_Minus_Int32(agreement, ref agr);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(agr);
            }
            ByteBufferData resultData = GetOneByteBufferData(len + SizeOfInt32 + SizeOfInt32 + SizeOfInt16);
            //写入大端小端标记
            Array.Copy(endian, 0, resultData.Buffer, 0, SizeOfInt16);
            //写入数据长度
            Array.Copy(head, 0, resultData.Buffer, SizeOfInt16, head.Length);
            //写入协议号
            Array.Copy(agr, 0, resultData.Buffer, SizeOfInt16 + SizeOfInt32, agr.Length);
            //写入数据
            Array.Copy(objData.Buffer, 0, resultData.Buffer, SizeOfInt16 + SizeOfInt32 + SizeOfInt32, objData.Length);
            PutBackByteBufferData(objData);
            if (tempData != null)
            {
                PutBackByteBufferData(tempData);
            }
            if (tempData2 != null)
            {
                PutBackByteBufferData(tempData2);
            }
            if (tempData3 != null)
            {
                PutBackByteBufferData(tempData3);
            }
            return resultData;
        }

        static SimpleListPool<List<Proto_Package_Data>, Proto_Package_Data> protoDataListPool = new SimpleListPool<List<Proto_Package_Data>, Proto_Package_Data>();

        static SimplePool<Proto_Package_Data> protoPackageDataPool = new SimplePool<Proto_Package_Data>();

        public static void PutBakOneProtoData(Proto_Package_Data data)
        {
            protoPackageDataPool.Recycle(data);
        }

        /// <summary>
        /// 回收网络数据包列表
        /// </summary>
        /// <param name="list"></param>
        public static void PutBackOneProtoDataListList(List<Proto_Package_Data> list)
        {
            protoDataListPool.Recycle(list);
        }

        public static SimpleListPool<List<ByteBufferData>, ByteBufferData> ByteBufferDataListPool = new SimpleListPool<List<ByteBufferData>, ByteBufferData>();

        static Dictionary<int, SimplePool<ByteBufferData>> byteBufferDataPoolDic = new Dictionary<int, SimplePool<ByteBufferData>>();

        static SimplePool<ByteBufferData> bigByteBufferDataPool = new SimplePool<ByteBufferData>();

        public static ByteBufferData GetOneByteBufferData(int lenght)
        {
            ByteBufferData byteBufferData = null;
            if (lenght>1024)
            {
                byteBufferData = bigByteBufferDataPool.Spawn();
                if (byteBufferData.Buffer==null || byteBufferData.Length!= lenght)
                {
                    byteBufferData.Buffer = new byte[lenght];
                }
            }
            else
            {
                lock (byteBufferDataPoolDic)
                {
                    SimplePool<ByteBufferData> pool;
                    if (!byteBufferDataPoolDic.TryGetValue(lenght, out pool))
                    {
                        pool = new SimplePool<ByteBufferData>();
                        byteBufferDataPoolDic.Add(lenght, pool);
                    }
                    byteBufferData = pool.Spawn();
                }
                if (byteBufferData.Buffer == null)
                {
                    byteBufferData.Buffer = new byte[lenght];
                }
            }
            byteBufferData.Length = lenght;
            return byteBufferData;
        }

        public static void PutBackByteBufferData(ByteBufferData data)
        {
            if (data.Length>1024)
            {
                data.Buffer = null;
                data.Length = 0;
                bigByteBufferDataPool.Recycle(data);
            }
            else
            {
                lock (byteBufferDataPoolDic)
                {
                    SimplePool<ByteBufferData> pool;
                    if (!byteBufferDataPoolDic.TryGetValue(data.Length, out pool))
                    {
                        pool = new SimplePool<ByteBufferData>();
                        byteBufferDataPoolDic.Add(data.Length, pool);
                    }
                    pool.Recycle(data);
                }
            }
        }

        public class ByteBufferData : ISimplePoolData
        {
            public int Length;

            public byte[] Buffer;

            bool isUsed = false;

            public bool IsUsed
            {
                get
                {
                    return isUsed;
                }
            }

            void ISimplePoolData.PutIn()
            {
                isUsed = false;
            }

            void ISimplePoolData.PutOut()
            {
                isUsed = true;
            }

            bool disposed = false;

            public bool Disposed
            {
                get
                {
                    return disposed;
                }
            }

            void IDisposable.Dispose()
            {
                disposed = true;
            }
        }

        /// <summary>
        /// 处理网络数据包
        /// </summary>
        /// <param name="data">接收到的网络包</param>
        /// <param name="len">接收的网络数据长度</param>
        /// <param name="lastLenght">剩余长度 用于粘包的处理</param>
        /// <returns></returns>
        public static List<Proto_Package_Data> Split_The_Parcel(byte[] data, Int32 len,ref bool err, ref Int32 lastLenght)
        {
            lastLenght = len;
            err = false;
            Proto.InitEndian();
            Int32 head_l = SizeOfInt32;
            Int32 agr_l = SizeOfInt32;
            Int32 int16_l = SizeOfInt16;
            List<Proto_Package_Data> res = protoDataListPool.Spawn();
            Int32 index = 0;
            while (index < len)
            {

                //网络数据粘包处理
                if (lastLenght< int16_l)
                {
                    return res;
                }


                //大小端标记
                ByteBufferData endianData = GetOneByteBufferData(int16_l);
                Array.Copy(data, index, endianData.Buffer, 0, endianData.Length);
                Int16 t_Endian= Proto.Bytes_Int16(endianData.Buffer);//解析大端小端标记
                PutBackByteBufferData(endianData);
                if (t_Endian!=257 && t_Endian!=0)
                {
                    Debug.LogError("网络数据解析失败, 大小端标记解析错误");
                    err = true;
                    return res;
                }
                Int16 te = (Int16)((t_Endian == 257) ? 1 : 2);
                lastLenght = lastLenght - SizeOfInt16;
                index = index + SizeOfInt16;


                //网络数据粘包处理
                if (lastLenght < head_l)
                {
                    lastLenght = lastLenght + SizeOfInt16;
                    return res;
                }

                ByteBufferData bufByteData = GetOneByteBufferData(head_l);
                Array.Copy(data, index, bufByteData.Buffer, 0, head_l);
                if (te != Proto.SysEndian)//大小端不同进行翻转
                {
                    Array.Reverse(bufByteData.Buffer);
                }
                Int32 head_len = Proto.Bytes_Int32(bufByteData.Buffer);
                if (head_len < (agr_l + 2 + head_l) || head_len>4096*8)
                {
                    Debug.LogError("网络数据解析失败, 长度错误");
                    err = true;
                    return res;
                }

                //if (head_len > (lastLenght - head_l))
                //{
                //    //Debug.LogError("网络数据粘包处理");
                //    lastLenght = lastLenght + SizeOfInt16;
                //    return res;
                //}

                lastLenght = lastLenght - head_l;
                index = index + head_l;

                //网络数据粘包处理
                if (lastLenght < agr_l)
                {
                    lastLenght = lastLenght + SizeOfInt16+ head_l;
                    return res;
                }

                //
                Array.Copy(data, index, bufByteData.Buffer, 0, agr_l);
                if (te != Proto.SysEndian)//大小端不同进行翻转
                {
                    Array.Reverse(bufByteData.Buffer);
                }
                Int32 agr = Proto.Bytes_Int32(bufByteData.Buffer);//协议号
                PutBackByteBufferData(bufByteData);
                Proto_Package_Data newData = protoPackageDataPool.Spawn();
                newData.need_rev = (te == Proto.SysEndian) ? false:true ;
                newData.agreement = agr;

                //网络数据粘包处理
                if (lastLenght < (head_len - agr_l))
                {
                    lastLenght = lastLenght + SizeOfInt16 + head_l;
                    return res;
                }

                //
                newData.byteBufferData = GetOneByteBufferData(head_len - agr_l);
                Array.Copy(data, index + agr_l, newData.byteBufferData.Buffer, 0, head_len - agr_l);
                lastLenght = lastLenght - head_len;
                index = index + head_len;
                res.Add(newData);
            }
            if (index != len)
            {
                Debug.LogError("网络数据解析异常");
            }
            return res;
        }

        static string Chars_String(char[] ch)
        {
            return new string(ch);
        }

        static byte[] Chars_Bytes(char[] ch)
        {
            return Encoding.Default.GetBytes(ch);
        }

        static Dictionary<string, byte[]> stringBytes = new Dictionary<string, byte[]>();
        static Dictionary<string, int> stringCounts = new Dictionary<string, int>();

        public static byte[] SetOneStringBytes(string str)
        {
            if (str == null)
            {
                str = "";
            }
            lock (stringBytes)
            {
                byte[] data = null;
                if (!stringBytes.TryGetValue(str,out data) || data==null)
                {
                    data = String_Bytes(str,false);
                    stringBytes[str] = data;
                }
                return data;
            }
        }

        public static byte[] String_Bytes(string str,bool dicFind=true)
        {
            if (str == null)
            {
                str = "";
            }
            if (dicFind)
            {
                lock (stringBytes)
                {
                    byte[] data = null;
                    if (stringBytes.TryGetValue(str, out data) || data != null)
                    {
                        return data;
                    }
                    if (str.Length<256)
                    {
                        int strCount = 0;
                        if (!stringCounts.TryGetValue(str, out strCount))
                        {
                            strCount = 1;
                            stringCounts.Add(str, strCount);
                        }
                        else
                        {
                            strCount++;
                            stringCounts[str] = strCount;
                        }
                        if (strCount > 5)
                        {
                            return SetOneStringBytes(str);
                        }
                    }
                }
            }
            return Encoding.UTF8.GetBytes(str);
        }

        static char[] Bytes_Chars(byte[] b)
        {
            return Encoding.ASCII.GetChars(b);
        }

        public static Int32 Bytes_Int32(byte[] b)
        {
            return BitConverter.ToInt32(b, 0);
        }

        static Int16 Bytes_Int16(byte[] b)
        {
            return BitConverter.ToInt16(b, 0);
        }

        static Int64 Bytes_Int64(byte[] b)
        {
            return BitConverter.ToInt64(b, 0);
        }

        static string Bytes_String(byte[] b)
        {
            return BitConverter.ToString(b, 0);
        }

        public static string BytesUTF8_String(byte[] b)
        {
            return System.Text.Encoding.UTF8.GetString(b);
        }

        static byte[] Obj_Bytes<T>(T t, bool open_minus = false)
        {
            Int16 minus = 1;
            Type typ = typeof(T);
            byte[] m;
            byte[] ac;
            byte[] res;
            if (typ == typeof(Int16))
            {
                Int16 d = Int16.Parse(t.ToString());
                minus = (Int16)((d < 0) ? 0 : 1);
                if (minus == 0) d = (Int16)(-d);
                ac = BitConverter.GetBytes(d);
            }
            else if (typ == typeof(Int32))
            {
                Int32 d = Int32.Parse(t.ToString());
                minus = (Int16)((d < 0) ? 0 : 1);
                if (minus == 0) d = -d;
                ac = BitConverter.GetBytes(d);
            }
            else if (typ == typeof(Int64))
            {
                Int64 d = Int64.Parse(t.ToString());
                minus = (Int16)((d < 0) ? 0 : 1);
                if (minus == 0) d = -d;
                ac = BitConverter.GetBytes(d);
            }
            else if (typ == typeof(bool))
            {
                bool d = bool.Parse(t.ToString());
                ac = BitConverter.GetBytes(d);
            }
            else
            {
                Debug.LogError("转换错误，不支持类型:" + typ);
                return null;
            }
            if (open_minus)
            {
                m = BitConverter.GetBytes(minus);
                res = new byte[m.Length + ac.Length];
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(m);
                    Array.Reverse(ac);
                }
                Array.Copy(m, 0, res, 0, m.Length);
                Array.Copy(ac, 0, res, m.Length, ac.Length);
                return res;
            }
            else
            {
                return ac;
            }
        }

        static Dictionary<Int16, byte[]> int16Bytes = new Dictionary<Int16, byte[]>();

        static byte[] BitConverterBytes_Int16(Int16 k)
        {
            if(k<0 || k>256) return BitConverter.GetBytes(k);
            lock (int16Bytes)
            {
                byte[] res=null;
                if (!int16Bytes.TryGetValue(k,out res) || res==null)
                {
                    res= BitConverter.GetBytes(k);
                    int16Bytes[k] = res;
                }
                return res;
            }
        }

        static Dictionary<Int32, byte[]> int32Bytes = new Dictionary<Int32, byte[]>();

        static byte[] BitConverterBytes_Int32(Int32 k)
        {
            if (k < 0 || k > 512) return BitConverter.GetBytes(k);
            lock (int32Bytes)
            {
                byte[] res = null;
                if (!int32Bytes.TryGetValue(k, out res) || res == null)
                {
                    res = BitConverter.GetBytes(k);
                    int32Bytes[k] = res;
                }
                return res;
            }
        }

        static Dictionary<Int64, byte[]> int64Bytes = new Dictionary<Int64, byte[]>();

        static byte[] BitConverterBytes_Int64(Int64 k)
        {
            if (k < 0 || k > 1024) return BitConverter.GetBytes(k);
            lock (int64Bytes)
            {
                byte[] res = null;
                if (!int64Bytes.TryGetValue(k, out res) || res == null)
                {
                    res = BitConverter.GetBytes(k);
                    int64Bytes[k] = res;
                }
                return res;
            }
        }

        static Dictionary<bool, byte[]> boolBytes = new Dictionary<bool, byte[]>();

        static byte[] BitConverterBytes_bool(bool k)
        {
            lock (boolBytes)
            {
                byte[] res = null;
                if (!boolBytes.TryGetValue(k, out res) || res == null)
                {
                    res = BitConverter.GetBytes(k);
                    boolBytes[k] = res;
                }
                return res;
            }
        }

        public static ByteBufferData Obj_Bytes_Minus_Int16(Int16 d, ref byte[] res, bool open_minus = false)
        {
            Int16 minus = 1;
            byte[] m;
            byte[] ac;
            minus = (Int16)((d < 0) ? 0 : 1);
            if (minus == 0) d = (Int16)(-d);
            ac = BitConverterBytes_Int16(d);
            if (open_minus)
            {
                m = BitConverterBytes_Int16(minus);
                ByteBufferData tempData= GetOneByteBufferData(m.Length + ac.Length);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(m);
                    Array.Reverse(ac);
                }
                Array.Copy(m, 0, tempData.Buffer, 0, m.Length);
                Array.Copy(ac, 0, tempData.Buffer, m.Length, ac.Length);
                res = tempData.Buffer;
                return tempData;
            }
            else
            {
                res = ac;
                return null;
            }
        }

        public static ByteBufferData Obj_Bytes_Minus_Int32(Int32 d, ref byte[] res, bool open_minus = false)
        {
            Int16 minus = 1;
            byte[] m;
            byte[] ac;
            minus = (Int16)((d < 0) ? 0 : 1);
            if (minus == 0) d = -d;
            ac = BitConverterBytes_Int32(d);
            if (open_minus)
            {
                m = BitConverterBytes_Int16(minus);
                ByteBufferData tempData = GetOneByteBufferData(m.Length + ac.Length);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(m);
                    Array.Reverse(ac);
                }
                Array.Copy(m, 0, tempData.Buffer, 0, m.Length);
                Array.Copy(ac, 0, tempData.Buffer, m.Length, ac.Length);
                res = tempData.Buffer;
                return tempData;
            }
            else
            {
                res = ac;
                return null;
            }
        }

        public static ByteBufferData Obj_Bytes_Minus_Int64(Int64 d, ref byte[] res, bool open_minus = false)
        {
            Int16 minus = 1;
            byte[] m;
            byte[] ac;
            minus = (Int16)((d < 0) ? 0 : 1);
            if (minus == 0) d = -d;
            ac = BitConverterBytes_Int64(d);
            if (open_minus)
            {
                m = BitConverterBytes_Int16(minus);
                ByteBufferData tempData = GetOneByteBufferData(m.Length + ac.Length);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(m);
                    Array.Reverse(ac);
                }
                Array.Copy(m, 0, tempData.Buffer, 0, m.Length);
                Array.Copy(ac, 0, tempData.Buffer, m.Length, ac.Length);
                res = tempData.Buffer;
                return tempData;
            }
            else
            {
                res = ac;
                return null;
            }
        }

        public static ByteBufferData Obj_Bytes_Minus_Bool(bool d, ref byte[] res, bool open_minus = false)
        {
            Int16 minus = 1;
            byte[] m;
            byte[] ac;
            ac = BitConverterBytes_bool(d);
            if (open_minus)
            {
                m = BitConverterBytes_Int16(minus);
                ByteBufferData tempData = GetOneByteBufferData(m.Length + ac.Length);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(m);
                    Array.Reverse(ac);
                }
                Array.Copy(m, 0, tempData.Buffer, 0, m.Length);
                Array.Copy(ac, 0, tempData.Buffer, m.Length, ac.Length);
                res = tempData.Buffer;
                return tempData;
            }
            else
            {
                res = ac;
                return null;
            }
        }

        /// <summary>
        /// 转换常规单体变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] SerializeT_Normal<T>(T t)
        {
            Type typ = typeof(T);
            if (typ == typeof(string))
            {
                MCustom_String data = new MCustom_String();
                data.str = t.ToString();
                ByteBufferData objData= data.Serialize();
                byte[] objBytes = new byte[objData.Length];
                Array.Copy(objData.Buffer, objBytes, objData.Length);
                PutBackByteBufferData(objData);
                return objBytes;
            }
            byte[] b = Proto.Obj_Bytes<T>(t, true);
            Int32 len = b.Length + 2 + SizeOfInt32;
            byte[] head = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int32(b.Length, ref head);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(head);
            }
            byte[] res = new byte[len];
            res[0] = 126;
            Array.Copy(head, 0, res, 1, SizeOfInt32);
            res[1 + SizeOfInt32] = 126;
            Array.Copy(b, 0, res, 2 + SizeOfInt32, b.Length);
            if (tempData!=null)
            {
                PutBackByteBufferData(tempData);
            }
            return res;
        }

        static ByteBufferData SerializeT_Normal(byte[] b)
        {
            Int32 len = b.Length + 2 + SizeOfInt32;
            byte[] head = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int32(b.Length, ref head);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(head);
            }
            ByteBufferData resultData = GetOneByteBufferData(len);
            resultData.Buffer[0] = 126;
            Array.Copy(head, 0, resultData.Buffer, 1, SizeOfInt32);
            resultData.Buffer[1 + SizeOfInt32] = 126;
            Array.Copy(b, 0, resultData.Buffer, 2 + SizeOfInt32, b.Length);
            return resultData;
        }

        public static ByteBufferData SerializeT_Normal_Int16(Int16 t)
        {
            byte[] b = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int16(t, ref b,true);
            //
            ByteBufferData res = SerializeT_Normal(b);
            //
            if (tempData!=null)
            {
                PutBackByteBufferData(tempData);
            }
            return res;
        }

        public static ByteBufferData SerializeT_Normal_Int32(Int32 t)
        {
            byte[] b = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int32(t, ref b, true);
            //
            ByteBufferData res = SerializeT_Normal(b);
            //
            if (tempData != null)
            {
                PutBackByteBufferData(tempData);
            }
            return res;
        }

        public static ByteBufferData SerializeT_Normal_Int64(Int64 t)
        {
            byte[] b = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int64(t, ref b, true);
            //
            ByteBufferData res = SerializeT_Normal(b);
            //
            if (tempData != null)
            {
                PutBackByteBufferData(tempData);
            }
            return res;
        }

        public static ByteBufferData SerializeT_Normal_bool(bool t)
        {
            byte[] b = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Bool(t, ref b, true);
            //
            ByteBufferData res = SerializeT_Normal(b);
            //
            if (tempData != null)
            {
                PutBackByteBufferData(tempData);
            }
            return res;
        }

        public static ByteBufferData SerializeT_Normal_string(string t)
        {
            MCustom_String data = GetOneProtoData<MCustom_String>();
            data.str = t;
            return data.Serialize();
        }

        /// <summary>
        /// 转换常规单体变量列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] SerializeT_Normal_List<T>(List<T> list)
        {
            Proto.ClearListNull(list);
            if (list.Count == 0)
            {
                byte[] res = new byte[2 + SizeOfInt32];
                byte[] head = null;
                ByteBufferData tempData = Obj_Bytes_Minus_Int32(0, ref head);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(head);
                }
                res[0] = 94;
                Array.Copy(head, 0, res, 1, SizeOfInt32);
                res[1 + SizeOfInt32] = 94;
                if (tempData!=null)
                {
                    PutBackByteBufferData(tempData);
                }
                return res;
            }
            List<byte[]> b_lst = new List<byte[]>();
            Int32 size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                byte[] b = Proto.SerializeT_Normal<T>(list[i]);//已经排序 不需要大小端判断
                b_lst.Add(b);
                size = size + b.Length;
            }
            //整合
            {
                byte[] res = new byte[size + 2 + SizeOfInt32];
                byte[] head = null;
                ByteBufferData tempData = Obj_Bytes_Minus_Int32(size, ref head);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(head);
                }
                res[0] = 94;
                Array.Copy(head, 0, res, 1, SizeOfInt32);
                res[1 + SizeOfInt32] = 94;
                Int32 index = 2 + SizeOfInt32;
                for (int i = 0; i < b_lst.Count; i++)
                {
                    byte[] b = b_lst[i];
                    Array.Copy(b, 0, res, index, b.Length);
                    index = index + b.Length;
                }
                if (tempData!=null)
                {
                    PutBackByteBufferData(tempData);
                }
                return res;
            }
        }

        static ByteBufferData SerializeT_Normal_List_Zero()
        {
            ByteBufferData resultData = GetOneByteBufferData(2 + SizeOfInt32);
            byte[] head = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int32(0, ref head);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(head);
            }
            resultData.Buffer[0] = 94;
            Array.Copy(head, 0, resultData.Buffer, 1, SizeOfInt32);
            resultData.Buffer[1 + SizeOfInt32] = 94;
            if (tempData != null)
            {
                PutBackByteBufferData(tempData);
            }
            return resultData;
        }

        static ByteBufferData SerializeT_Normal_ByteList(List<ByteBufferData> b_lst, Int32 size)
        {
            ByteBufferData resultData = GetOneByteBufferData(size + 2 + SizeOfInt32);
            byte[] head = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int32(size, ref head);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(head);
            }
            resultData.Buffer[0] = 94;
            Array.Copy(head, 0, resultData.Buffer, 1, SizeOfInt32);
            resultData.Buffer[1 + SizeOfInt32] = 94;
            Int32 index = 2 + SizeOfInt32;
            for (int i = 0; i < b_lst.Count; i++)
            {
                ByteBufferData b = b_lst[i];
                Array.Copy(b.Buffer, 0, resultData.Buffer, index, b.Length);
                index = index + b.Length;
                PutBackByteBufferData(b);
            }
            ByteBufferDataListPool.Recycle(b_lst);
            if (tempData != null)
            {
                PutBackByteBufferData(tempData);
            }
            return resultData;
        }

        public static ByteBufferData SerializeT_Normal_List_Int16(List<Int16> list)
        {
            if (list.Count==0)
            {
                return SerializeT_Normal_List_Zero();
            }
            List<ByteBufferData> b_lst = ByteBufferDataListPool.Spawn();
            Int32 size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                ByteBufferData b = Proto.SerializeT_Normal_Int16(list[i]);//已经排序 不需要大小端判断
                b_lst.Add(b);
                size = size + b.Length;
            }
            return SerializeT_Normal_ByteList(b_lst, size);
        }

        public static ByteBufferData SerializeT_Normal_List_Int32(List<Int32> list)
        {
            if (list.Count == 0)
            {
                return SerializeT_Normal_List_Zero();
            }
            List<ByteBufferData> b_lst = ByteBufferDataListPool.Spawn();
            Int32 size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                ByteBufferData b = Proto.SerializeT_Normal_Int32(list[i]);//已经排序 不需要大小端判断
                b_lst.Add(b);
                size = size + b.Length;
            }
            return SerializeT_Normal_ByteList(b_lst, size);
        }

        public static ByteBufferData SerializeT_Normal_List_Int64(List<Int64> list)
        {
            if (list.Count == 0)
            {
                return SerializeT_Normal_List_Zero();
            }
            List<ByteBufferData> b_lst = ByteBufferDataListPool.Spawn();
            Int32 size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                ByteBufferData b = Proto.SerializeT_Normal_Int64(list[i]);//已经排序 不需要大小端判断
                b_lst.Add(b);
                size = size + b.Length;
            }
            return SerializeT_Normal_ByteList(b_lst, size);
        }

        public static ByteBufferData SerializeT_Normal_List_bool(List<bool> list)
        {
            if (list.Count == 0)
            {
                return SerializeT_Normal_List_Zero();
            }
            List<ByteBufferData> b_lst = ByteBufferDataListPool.Spawn();
            Int32 size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                ByteBufferData b = Proto.SerializeT_Normal_bool(list[i]);//已经排序 不需要大小端判断
                b_lst.Add(b);
                size = size + b.Length;
            }
            return SerializeT_Normal_ByteList(b_lst, size);
        }

        public static ByteBufferData SerializeT_Normal_List_string(List<string> list)
        {
            if (list.Count == 0)
            {
                return SerializeT_Normal_List_Zero();
            }
            List<ByteBufferData> b_lst = ByteBufferDataListPool.Spawn();
            Int32 size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                ByteBufferData b = Proto.SerializeT_Normal_string(list[i]);//已经排序 不需要大小端判断
                b_lst.Add(b);
                size = size + b.Length;
            }
            return SerializeT_Normal_ByteList(b_lst, size);
        }

        /// <summary>
        /// 转换自定义变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ByteBufferData SerializeT_Self<T>(T t)
        {
            Proto_Base obj = (Proto_Base)((object)t);
            return obj.Serialize();
        }
        /// <summary>
        /// 转换自定义变量列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ByteBufferData SerializeT_Self_list<T>(List<T> list)
        {
            Proto.ClearListNull(list);
            if (list.Count == 0)
            {
                ByteBufferData resultData = GetOneByteBufferData(2 + SizeOfInt32);
                byte[] head = null;
                ByteBufferData tempData = Obj_Bytes_Minus_Int32(0, ref head);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(head);
                }
                resultData.Buffer[0] = 94;
                Array.Copy(head, 0, resultData.Buffer, 1, SizeOfInt32);
                resultData.Buffer[1 + SizeOfInt32] = 94;
                if (tempData!=null)
                {
                    PutBackByteBufferData(tempData);
                }
                return resultData;
            }
            //
            List<ByteBufferData> b_lst = ByteBufferDataListPool.Spawn();
            Int32 size = 0;
            for (int i = 0; i < list.Count; i++)
            {
                Proto_Base da = (Proto_Base)((object)list[i]);
                ByteBufferData b = da.Serialize();
                b_lst.Add(b);
                size = size + b.Length;
            }
            //整合
            {
                ByteBufferData resultData = GetOneByteBufferData(size + 2 + SizeOfInt32);
                byte[] head = null;
                ByteBufferData tempData = Obj_Bytes_Minus_Int32(size, ref head);
                if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
                {
                    Array.Reverse(head);
                }
                resultData.Buffer[0] = 94;
                Array.Copy(head, 0, resultData.Buffer, 1, SizeOfInt32);
                resultData.Buffer[1 + SizeOfInt32] = 94;
                Int32 index = 2 + SizeOfInt32;
                for (int i = 0; i < b_lst.Count; i++)
                {
                    ByteBufferData b = b_lst[i];
                    Array.Copy(b.Buffer, 0, resultData.Buffer, index, b.Length);
                    index = index + b.Length;
                    PutBackByteBufferData(b);
                }
                ByteBufferDataListPool.Recycle(b_lst);
                if (tempData!=null)
                {
                    PutBackByteBufferData(tempData);
                }
                return resultData;
            }
        }

        /// <summary>
        /// 解析常规变量 单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ByteBufferData Deserialize_Normal<T>(ByteBufferData b, ref T t, ref bool err, bool need_rev)
        {
            if (err) return b;
            if (b.Buffer[0] != 126 || b.Buffer[0 + 1 + SizeOfInt32] != 126)
            {
                Debug.LogError("数据解析错误");
                err = true;
                return b;
            }
            //
            int index = 0;
            index++;
            ByteBufferData len_ar = GetOneByteBufferData(SizeOfInt32);
            Array.Copy(b.Buffer, index, len_ar.Buffer, 0, SizeOfInt32);
            if (need_rev)//进行大小端排序
            {
                Array.Reverse(len_ar.Buffer);
            }
            Int32 len = Proto.Bytes_Int32(len_ar.Buffer);
            PutBackByteBufferData(len_ar);
            Int32 old_len = b.Length;;
            ByteBufferData res = GetOneByteBufferData(old_len - len - 2 - SizeOfInt32);
            Array.Copy(b.Buffer, 2 + SizeOfInt32 + len, res.Buffer, 0, res.Length);

            //Int32 l = b.Length;
            Type typ = typeof(T);
            if (typ == typeof(string))
            {
                MCustom_String data = GetOneProtoData<MCustom_String>();
                data.Deserialize(b, need_rev);
                t = (T)((object)data.str);
                PutBackOneProtoData<MCustom_String>(data);
            }else
            if (typ == typeof(Int16))
            {
                if (old_len < (2 + SizeOfInt32 + SizeOfInt16 + SizeOfInt16))
                {
                    Debug.LogError("数据解析错误");
                    err = true;
                    PutBackByteBufferData(res);
                    return b;
                }
                if (need_rev)//进行大小端排序
                {
                    Array.Reverse(b.Buffer, 2 + SizeOfInt32, SizeOfInt16);
                }
                Int16 minus = BitConverter.ToInt16(b.Buffer, 2 + SizeOfInt32);
                minus = (Int16)((minus > 0) ? 1 : -1);
                if (need_rev)//进行大小端排序
                {
                    Array.Reverse(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16, SizeOfInt16);
                }
                Int16 d = BitConverter.ToInt16(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16);
                d = (Int16)(d * minus);
                t = (T)((object)d);
                PutBackByteBufferData(b);
            }
            else if (typ == typeof(Int32))
            {
                if (old_len < (2 + SizeOfInt32 + SizeOfInt32 + SizeOfInt16))
                {
                    Debug.LogError("数据解析错误");
                    err = true;
                    PutBackByteBufferData(res);
                    return b;
                }
                if (need_rev)//进行大小端排序
                {
                    Array.Reverse(b.Buffer, 2 + SizeOfInt32, SizeOfInt16);
                }
                Int16 minus = BitConverter.ToInt16(b.Buffer, 2 + SizeOfInt32);
                minus = (Int16)((minus > 0) ? 1 : -1);
                if (need_rev)//进行大小端排序
                {
                    Array.Reverse(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16, SizeOfInt32);
                }
                Int32 d = BitConverter.ToInt32(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16);
                d = d * minus;
                t = (T)((object)d);
                PutBackByteBufferData(b);
            }
            else if (typ == typeof(Int64))
            {
                if (old_len < (2 + SizeOfInt32 + SizeOfInt64 + SizeOfInt16))
                {
                    Debug.LogError("数据解析错误");
                    err = true;
                    PutBackByteBufferData(res);
                    return b;
                }
                if (need_rev)//进行大小端排序
                {
                    Array.Reverse(b.Buffer, 2 + SizeOfInt32, SizeOfInt16);
                }
                Int16 minus = BitConverter.ToInt16(b.Buffer, 2 + SizeOfInt32);
                minus = (Int16)((minus > 0) ? 1 : -1);
                if (need_rev)//进行大小端排序
                {
                    Array.Reverse(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16, SizeOfInt64);
                }
                Int64 d = BitConverter.ToInt64(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16);
                d = d * minus;
                t = (T)((object)d);
                PutBackByteBufferData(b);
            }
            else if (typ == typeof(bool))
            {
                if (old_len < (2 + SizeOfInt32 + SizeOfBool + SizeOfInt16))
                {
                    Debug.LogError("数据解析错误");
                    err = true;
                    PutBackByteBufferData(res);
                    return b;
                }
                if (need_rev)//进行大小端排序
                {
                    Array.Reverse(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16, SizeOfBool);
                }
                bool d = BitConverter.ToBoolean(b.Buffer, 2 + SizeOfInt32 + SizeOfInt16);
                t = (T)((object)d);
                PutBackByteBufferData(b);
            }
            return res;
        }

        /// <summary>
        /// 解析常规变量 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ByteBufferData Deserialize_Normal_list<T>(ByteBufferData b, ref List<T> t_list, ref bool err, bool need_rev)
        {
            if (err) return b;
            if (b.Buffer[0] != 94 || b.Buffer[0 + 1 + SizeOfInt32] != 94)
            {
                Debug.LogError("数据列表解析错误");
                err = true;
                return b;
            }
            //
            int index = 0;
            index++;
            ByteBufferData len_ar = GetOneByteBufferData(SizeOfInt32);
            Array.Copy(b.Buffer, index, len_ar.Buffer, 0, SizeOfInt32);
            if (need_rev)//进行大小端排序
            {
                Array.Reverse(len_ar.Buffer);
            }
            Int32 len = Proto.Bytes_Int32(len_ar.Buffer);
            PutBackByteBufferData(len_ar);
            Int32 old_len = b.Length;
            if (old_len < (len + 2 + SizeOfInt32))
            {
                Debug.LogError("数据列表解析错误");
                err = true;
                return b;
            }
            ByteBufferData res = GetOneByteBufferData(old_len - len - 2 - SizeOfInt32);
            Array.Copy(b.Buffer, 2 + SizeOfInt32 + len, res.Buffer, 0, res.Length);
            ByteBufferData des_res = GetOneByteBufferData(len);
            Array.Copy(b.Buffer, 2 + SizeOfInt32, des_res.Buffer, 0, len);
            while (des_res.Length > 0)
            {
                T t = default(T);
                bool is_err = false;
                des_res = Proto.Deserialize_Normal<T>(des_res, ref t, ref is_err, need_rev);
                if (is_err)
                {
                    Debug.LogError("数据列表解析错误");
                    err = true;
                    PutBackByteBufferData(res);
                    PutBackByteBufferData(des_res);
                    return b;
                }
                t_list.Add(t);
            }
            PutBackByteBufferData(des_res);
            return res;
        }

        /// <summary>
        /// 解析自定义变量 单
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ByteBufferData Deserialize_Self<T>(ByteBufferData b, ref T t, ref bool err, bool need_rev)
        {
            if(err) return b;
            if (b.Buffer[0] != 126 || b.Buffer[0 + 1 + SizeOfInt32] != 126)
            {
                Debug.LogError("自定义数据解析错误");
                err = true;
                return b;
            }
            //
            int index = 0;
            index++;
            ByteBufferData len_ar = GetOneByteBufferData(SizeOfInt32);
            Array.Copy(b.Buffer, index, len_ar.Buffer, 0, SizeOfInt32);
            if (need_rev)//进行大小端排序
            {
                Array.Reverse(len_ar.Buffer);
            }
            Int32 len = Proto.Bytes_Int32(len_ar.Buffer);
            PutBackByteBufferData(len_ar);
            Int32 old_len = b.Length;
            if (old_len < (2 + SizeOfInt32 + len))
            {
                Debug.LogError("自定义数据解析错误");
                err = true;
                return b;
            }
            ByteBufferData res = GetOneByteBufferData(old_len - len - 2 - SizeOfInt32);
            Array.Copy(b.Buffer, 2 + SizeOfInt32 + len, res.Buffer, 0, res.Length);
            //
            ByteBufferData res_c = GetOneByteBufferData(len + 2 + SizeOfInt32);
            Array.Copy(b.Buffer, 0, res_c.Buffer, 0, res_c.Length);
            Proto_Base obj = (Proto_Base)((object)t);
            obj.Deserialize(res_c, need_rev);
            //
            return res;
        }

        /// <summary>
        /// 解析自定义变量 列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ByteBufferData Deserialize_Self_list<T>(ByteBufferData b, ref List<T> t_list, T x, ref bool err, bool need_rev)
        {
            if (err) return b;
            if (b.Buffer[0] != 94 || b.Buffer[0 + 1 + SizeOfInt32] != 94)
            {
                Debug.LogError("自定义数据列表解析错误");
                err = true;
                return b;
            }
            //
            int index = 0;
            index++;
            ByteBufferData len_ar = GetOneByteBufferData(SizeOfInt32);
            Array.Copy(b.Buffer, index, len_ar.Buffer, 0, SizeOfInt32);
            if (need_rev)//进行大小端排序
            {
                Array.Reverse(len_ar.Buffer);
            }
            Int32 len = Proto.Bytes_Int32(len_ar.Buffer);
            PutBackByteBufferData(len_ar);
            Int32 old_len = b.Length;
            if (old_len < (2 + SizeOfInt32 + len))
            {
                Debug.LogError("自定义数据列表解析错误");
                err = true;
                return b;
            }
            ByteBufferData res = GetOneByteBufferData(old_len - len - 2 - SizeOfInt32);
            Array.Copy(b.Buffer, 2 + SizeOfInt32 + len, res.Buffer, 0, res.Length);

            ByteBufferData des_res = GetOneByteBufferData(len);
            Array.Copy(b.Buffer, 2 + SizeOfInt32, des_res.Buffer, 0, len);
            while (des_res.Length > 0)
            {
                Proto_Base obj = (Proto_Base)((object)x);
                T t = (T)obj.GetNew();
                obj = (Proto_Base)((object)t);
                bool iserr = false;
                des_res = Proto.Deserialize_Self(des_res, ref obj, ref iserr, need_rev);
                if (iserr)
                {
                    Debug.LogError("自定义数据列表解析错误");
                    err = true;
                    PutBackByteBufferData(res);
                    PutBackByteBufferData(des_res);
                    return b;
                }
                t_list.Add((T)((object)obj));
            }
            PutBackByteBufferData(des_res);
            return res;
        }

        public static void ClearListNull<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        public static ByteBufferData Unity_Bytes(List<ByteBufferData> b_lst, Int32 size = 0)
        {
            if (size == 0)
            {
                for (int i = 0; i < b_lst.Count; i++)
                {
                    size = size + b_lst[i].Length;
                }
            }
            //整合
            ByteBufferData resultData = GetOneByteBufferData(size + 2 + SizeOfInt32);
            byte[] head = null;
            ByteBufferData tempData = Obj_Bytes_Minus_Int32(size, ref head);
            if (Proto.SysEndian != Proto.Endian)//大端小端不匹配的时候进行翻转
            {
                Array.Reverse(head);
            }
            resultData.Buffer[0] = 126;
            Array.Copy(head, 0, resultData.Buffer, 1, SizeOfInt32);
            resultData.Buffer[1 + SizeOfInt32] = 126;
            Int32 index = 2 + SizeOfInt32;
            for (int i = 0; i < b_lst.Count; i++)
            {
                ByteBufferData data = b_lst[i];
                Array.Copy(data.Buffer, 0, resultData.Buffer, index, data.Buffer.Length);
                index = index + data.Buffer.Length;
                PutBackByteBufferData(data);
            }
            ByteBufferDataListPool.Recycle(b_lst);
            if (tempData!=null)
            {
                PutBackByteBufferData(tempData);
            }
            return resultData;
        }

        public static Proto.ByteBufferData DesInt(Proto.ByteBufferData bytes, ref bool err, bool need_rev)
        {
            err = false;
            Int32 byteLen = bytes.Length;
            Int32 dsize = SizeOfInt32;
            //char[] chars = Proto.Bytes_Chars(bytes);
            int index = 0;
            if (bytes.Buffer[index] != 126 || bytes.Buffer[index + 1 + dsize] != 126)
            {
                Debug.LogError("结构体MCustom_String反序列化失败");
                err = true;
                return bytes;
            }
            index++;
            ByteBufferData len_ar = GetOneByteBufferData(dsize);
            Array.Copy(bytes.Buffer, index, len_ar.Buffer, 0, dsize);
            if (need_rev)
            {
                Array.Reverse(len_ar.Buffer);
            }
            Int32 len = Proto.Bytes_Int32(len_ar.Buffer);
            PutBackByteBufferData(len_ar);
            if (len > (byteLen - 2 - dsize) || len < 0)
            {
                Debug.LogError("结构体test01反序列化失败");
                err = true;
                return bytes;
            }
            else if (len == 0)
            {
                Debug.LogError("结构体test01反序列化失败");
                err = true;
                return bytes;
            }
            index = index + dsize + 1;
            ByteBufferData b = GetOneByteBufferData(len);
            Array.Copy(bytes.Buffer, index, b.Buffer, 0, len);//在下一步进行大小端排序
            PutBackByteBufferData(bytes);
            return b;
        }

        static Dictionary<Type, object> protoBaseDataPools = new Dictionary<Type, object>();

        static SimplePool<T> FindProtoBasePool<T>() where T : Proto_Base, new()
        {
            Type t = typeof(T);
            lock (protoBaseDataPools)
            {
                object poolObj = null;
                SimplePool<T> poolRes = null;
                if (!protoBaseDataPools.TryGetValue(t,out poolObj) || poolObj == null)
                {
                    poolRes = new SimplePool<T>();
                    protoBaseDataPools[t] = poolRes;
                }
                else
                {
                    poolRes = (SimplePool<T>)poolObj;
                }
                return poolRes;
            }
        }

        public static T GetOneProtoData<T>() where T : Proto_Base, new()
        {
            SimplePool<T> pool = FindProtoBasePool<T>();
            T data = pool.Spawn();
            return data;
        }

        public static void PutBackOneProtoData<T>(T data) where T : Proto_Base, new()
        {
            SimplePool<T> pool = FindProtoBasePool<T>();
            pool.Recycle(data);
        }
    }

    /// <summary>
    /// 结构体网络数据切割结果
    /// </summary>
    public class Proto_Package_Data : ISimplePoolData
    {
        public bool need_rev;//是否需要转序（处理大小端数据）
        public Int32 agreement;//协议号
        public Proto.ByteBufferData byteBufferData;//数据(根据协议号进行下一步的数据反序列化)
        public bool GetData(Proto_Base obj)
        {
            return obj.Deserialize(byteBufferData, need_rev);
        }

        bool isUsed = false;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
        }

        void ISimplePoolData.PutIn()
        {
            Proto.PutBackByteBufferData(byteBufferData);
            isUsed = false;
        }

        void ISimplePoolData.PutOut()
        {
            isUsed = true;
        }

        bool disposed = false;

        public bool Disposed
        {
            get
            {
                return disposed;
            }
        }

        void IDisposable.Dispose()
        {
            disposed = true;
        }
    };

    /// <summary>
    /// 数据基类
    /// </summary>
    public class Proto_Base : ISimplePoolData
    {
        public virtual Proto.ByteBufferData Serialize() { return null; }
        public virtual bool Deserialize(Proto.ByteBufferData bytes,bool need_rev) { return false; }
        public virtual object GetNew() { return new Proto_Base() ; }

        bool isUsed = false;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
        }

        public virtual void PutIn()
        {
            isUsed = false;
        }

        void ISimplePoolData.PutOut()
        {
            isUsed = true;
        }

        bool disposed = false;

        public bool Disposed
        {
            get
            {
                return disposed;
            }
        }

        void IDisposable.Dispose()
        {
            disposed = true;
        }
    }
}
