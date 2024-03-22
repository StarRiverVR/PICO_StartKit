 
//
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/*
*关键属性
*ProtoTool::Endian //当前强制使用的端排序模式 (可与ProtoTool::SysEndian配合使用系统默认排序)
*ProtoTool.Serialize //将传入的协议结构序列化成网络传输的数据 (端序(2字节)|数据长度(4字节)|协议号(4字节)|数据字节)
*ProtoTool.Split_The_Parcel //将接收到的网络数据切割成若干Proto_Package_Data
*Proto_Package_Data.Deserialize() //将切割完的网络数据反序列化成对象
*通用数据序列化规则(int16_t,int32_t,int64_t,short,bool)->('~'+数据长度(4字节)+'~'+|+正负标记(2字节)+|+数据长度)
*数组规则('^'+数据长度(4字节)+'^'+|+数据)
*数组规则('^'+数据长度(4字节)+'^'+|+数据)
**/

/// <summary>
/// 网络协议生成数据
/// 使用方法：
/// 1:使用Proto.Serialize();返回数据的byte[],用于网络发送
/// 2:使用Proto.Split_The_Parcel();将接收到的网络数据进行拆解
/// 3:将拆解出来的Proto_Package_Data数据使用Proto_Package_Data.GetData();进行解析
/// </summary>
namespace proto
{
	/// <summary>
	/// 协议号
	/// </summary>
	public enum Proto_Agreement_Enum{
		/// <summary>
		/// //心跳 服务器
		/// <summary>
		Heart_S=1,
		/// <summary>
		/// //心跳 客户端
		/// <summary>
		Heart_C=2,
		/// <summary>
		/// //log日志
		/// <summary>
		Debug_Log=3,
		/// <summary>
		/// //log日志
		/// <summary>
		Debug_LogError=4,
		/// <summary>
		/// //log日志
		/// <summary>
		Debug_LogWarning=5,

		Debug_JsonData = 6,
	}

	/// <summary>
	/// 字节数组
	/// </summary>
	public class MCustom_Bytes : Proto_Base
    {
		public byte[] datas = null;

		public override object GetNew()
		{
			return Proto.GetOneProtoData<MCustom_Bytes>();
		}

		/// <summary>
		/// 序列化的结果不可以作为网络传输 ，网络传输数据需要在类class Proto中获取
		/// </summary>
		/// <returns></returns>
		public override Proto.ByteBufferData Serialize()
		{
			if (datas != null && datas.Length == 0) datas = null;
			byte[] b = datas;//已经排序 不需要进行大小端判断
			int lenght = b != null ? b.Length : 0;

			Int32 len = lenght + 2 + Proto.SizeOfInt32;
			byte[] head = null;
			Proto.ByteBufferData tempData = Proto.Obj_Bytes_Minus_Int32(lenght, ref head);
			if (Proto.SysEndian != Proto.Endian){//大端小端不匹配的时候进行翻转
				Array.Reverse(head);
			}
			Proto.ByteBufferData resultData = Proto.GetOneByteBufferData(len);
			resultData.Buffer[0] = 126;
			Array.Copy(head, 0, resultData.Buffer, 1, Proto.SizeOfInt32);
			resultData.Buffer[1 + Proto.SizeOfInt32] = 126;
            if (b!=null){
				Array.Copy(b, 0, resultData.Buffer, 2 + Proto.SizeOfInt32, lenght);
			}
			if (tempData != null){
				Proto.PutBackByteBufferData(tempData);
			}
			return resultData;
		}

		public override bool Deserialize(Proto.ByteBufferData bytes, bool need_rev)
		{
			Int32 byteLen = bytes.Length;
			Int32 dsize = Proto.SizeOfInt32;
			int index = 0;
			if (bytes.Buffer[index] != 126 || bytes.Buffer[index + 1 + dsize] != 126)
			{
				Debug.LogError("结构体MCustom_Bytes反序列化失败"); return false;
			}
			index++;
			Proto.ByteBufferData len_ar = Proto.GetOneByteBufferData(dsize);
			Array.Copy(bytes.Buffer, index, len_ar.Buffer, 0, dsize);
			if (need_rev)
			{
				Array.Reverse(len_ar.Buffer);
			}
			Int32 len = Proto.Bytes_Int32(len_ar.Buffer);
			Proto.PutBackByteBufferData(len_ar);
			if (len > (byteLen - 2 - dsize) || len < 0)
			{
				Debug.LogError("结构体MCustom_String反序列化失败"); return false;
			}
			else if (len == 0)
			{
				datas = null; return true;
            }
            else
            {
				datas = new byte[len];
			}
			index = index + dsize + 1;
			Array.Copy(bytes.Buffer, index, datas, 0, len);
			Proto.PutBackByteBufferData(bytes);
			return true;
		}

		public override void PutIn()
		{
			base.PutIn();
			datas = null;
		}
	}

	/// <summary>
	/// 字符串类型需要用此类替换
	/// </summary>
	public class MCustom_String: Proto_Base
    {
        public string str=string.Empty;

        public override object GetNew(){
            return Proto.GetOneProtoData<MCustom_String>();
        }

        /// <summary>
        /// 序列化的结果不可以作为网络传输 ，网络传输数据需要在类class Proto中获取
        /// </summary>
        /// <returns></returns>
        public override Proto.ByteBufferData Serialize()
        {
            if (str==null){str = string.Empty;}
            byte[] b= Proto.String_Bytes(str);//已经排序 不需要进行大小端判断
            Int32 len = b.Length+2+ Proto.SizeOfInt32;
            byte[] head = null;
            Proto.ByteBufferData tempData = Proto.Obj_Bytes_Minus_Int32(b.Length, ref head);
            if (Proto.SysEndian != Proto.Endian) {//大端小端不匹配的时候进行翻转
                Array.Reverse(head);
            }
            Proto.ByteBufferData resultData = Proto.GetOneByteBufferData(len);
            resultData.Buffer[0] = 126;
            Array.Copy(head,0, resultData.Buffer,1, Proto.SizeOfInt32);
            resultData.Buffer[1+ Proto.SizeOfInt32] = 126;
            Array.Copy(b, 0, resultData.Buffer, 2 + Proto.SizeOfInt32, b.Length);
            if (tempData!=null){
                Proto.PutBackByteBufferData(tempData);
            }
            return resultData;
        }

        public override bool Deserialize(Proto.ByteBufferData bytes,bool need_rev)
        {
            Int32 byteLen = bytes.Length;
            Int32 dsize = Proto.SizeOfInt32;
            int index = 0;
            if (bytes.Buffer[index] != 126 || bytes.Buffer[index + 1 + dsize] != 126){
                Debug.LogError("结构体MCustom_String反序列化失败");return false;
            }
            index++;
            Proto.ByteBufferData len_ar = Proto.GetOneByteBufferData(dsize);
            Array.Copy(bytes.Buffer, index, len_ar.Buffer, 0, dsize);
            if (need_rev){
                Array.Reverse(len_ar.Buffer);
            }
            Int32 len = Proto.Bytes_Int32(len_ar.Buffer);
            Proto.PutBackByteBufferData(len_ar);
            if (len > (byteLen-2- dsize) || len<0){
                Debug.LogError("结构体MCustom_String反序列化失败");return false;
            }else if (len==0){
                this.str = string.Empty;return true;
            }
            index = index + dsize + 1;
            Proto.ByteBufferData str_s = Proto.GetOneByteBufferData(len);
            Array.Copy(bytes.Buffer, index, str_s.Buffer, 0, len);
            this.str = Proto.BytesUTF8_String(str_s.Buffer);
            Proto.PutBackByteBufferData(str_s);
            Proto.PutBackByteBufferData(bytes);
            return true;
        }

        public override void PutIn(){
            base.PutIn();
            str = "";
        }

    }


	//********example********//
	//被引用的结构需要写在引用他的结构前面,两个结构体不可以相互引用
	//被temp_demo_a引用的变量
	public class temp_demo_b: Proto_Base{
		public Int16 f;//
		public string c;//

		public override object GetNew(){
			return Proto.GetOneProtoData<temp_demo_b>();
		}

		public temp_demo_b(){
			f = 0;
			c = "test";
		}

		public override void PutIn(){
			base.PutIn();
			c = "";
		}

		//生成的数据不可以直接发送，若要发送需要使用Proto.Serialize();
		public override Proto.ByteBufferData Serialize(){
			List<Proto.ByteBufferData> b_lst_s_d = Proto.ByteBufferDataListPool.Spawn();
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int16(f));
			b_lst_s_d.Add(Proto.SerializeT_Normal_string(c));
			return Proto.Unity_Bytes(b_lst_s_d);
		}

		//可以将对应的Proto_Package_Data中的数据更新到this里面
		public override bool Deserialize(Proto.ByteBufferData bytes,bool need_rev){
			bool iserr = false;
			Proto.ByteBufferData b_s_c_res = Proto.DesInt(bytes, ref iserr,need_rev);
			if (iserr){
				Proto.PutBackByteBufferData(b_s_c_res);
				return false;
			}
			b_s_c_res = Proto.Deserialize_Normal<Int16>(b_s_c_res, ref f, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<string>(b_s_c_res, ref c, ref iserr,need_rev);
			Proto.PutBackByteBufferData(b_s_c_res);
			if (iserr){
				return false;
			}
			return true;
		}
	}

	//结构体支持的定义格式
	public class temp_demo_a: Proto_Base{
		public temp_demo_b a;//自定义单变量的定义
		public List<temp_demo_b> b;//自定义结构列表的定义
		public string c;//字符串定义
		public List<string> d;//字符串列表定义
		public Int16 e;//lua协议需要大于=0的数
		public Int16 f;//lua协议需要大于=0的数
		public Int32 g;//lua协议需要大于=0的数
		public bool h;//lua协议需要大于=0的数
		public Int64 i;//lua协议需要大于=0的数
		public List<Int64> l;//通用列表的定义

		public override object GetNew(){
			return Proto.GetOneProtoData<temp_demo_a>();
		}

		public temp_demo_a(){
			a = Proto.GetOneProtoData<temp_demo_b>();
			b = new List<temp_demo_b>();
			c = "test";
			d = new List<string>();
			e = 0;
			f = 0;
			g = 0;
			h = true;
			i = 0;
			l = new List<Int64>();
		}

		public override void PutIn(){
			base.PutIn();
			for (int i=0;i<b.Count;++i){
				Proto.PutBackOneProtoData<temp_demo_b>(b[i]);
			}
			b.Clear();
			c = "";
			d.Clear();
			l.Clear();
		}

		//生成的数据不可以直接发送，若要发送需要使用Proto.Serialize();
		public override Proto.ByteBufferData Serialize(){
			List<Proto.ByteBufferData> b_lst_s_d = Proto.ByteBufferDataListPool.Spawn();
			b_lst_s_d.Add(Proto.SerializeT_Self<temp_demo_b>(a));
			b_lst_s_d.Add(Proto.SerializeT_Self_list<temp_demo_b>(b));
			b_lst_s_d.Add(Proto.SerializeT_Normal_string(c));
			b_lst_s_d.Add(Proto.SerializeT_Normal_List_string(d));
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int16(e));
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int16(f));
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int32(g));
			b_lst_s_d.Add(Proto.SerializeT_Normal_bool(h));
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int64(i));
			b_lst_s_d.Add(Proto.SerializeT_Normal_List_Int64(l));
			return Proto.Unity_Bytes(b_lst_s_d);
		}

		//可以将对应的Proto_Package_Data中的数据更新到this里面
		public override bool Deserialize(Proto.ByteBufferData bytes,bool need_rev){
			bool iserr = false;
			Proto.ByteBufferData b_s_c_res = Proto.DesInt(bytes, ref iserr,need_rev);
			if (iserr){
				Proto.PutBackByteBufferData(b_s_c_res);
				return false;
			}
			b_s_c_res = Proto.Deserialize_Self<temp_demo_b>(b_s_c_res, ref a, ref iserr,need_rev);
			b.Clear();
			temp_demo_b b_new_c_r = Proto.GetOneProtoData<temp_demo_b>();
			b_s_c_res = Proto.Deserialize_Self_list<temp_demo_b>(b_s_c_res, ref b, b_new_c_r, ref iserr,need_rev);
			Proto.PutBackOneProtoData<temp_demo_b>(b_new_c_r);
			b_s_c_res = Proto.Deserialize_Normal<string>(b_s_c_res, ref c, ref iserr,need_rev);
			d.Clear();
			b_s_c_res = Proto.Deserialize_Normal_list<string>(b_s_c_res, ref d, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<Int16>(b_s_c_res, ref e, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<Int16>(b_s_c_res, ref f, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<Int32>(b_s_c_res, ref g, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<bool>(b_s_c_res, ref h, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<Int64>(b_s_c_res, ref i, ref iserr,need_rev);
			l.Clear();
			b_s_c_res = Proto.Deserialize_Normal_list<Int64>(b_s_c_res, ref l, ref iserr,need_rev);
			Proto.PutBackByteBufferData(b_s_c_res);
			if (iserr){
				return false;
			}
			return true;
		}
	}

	//心跳包
	public class heart_data: Proto_Base{
		public Int16 res;

		public override object GetNew(){
			return Proto.GetOneProtoData<heart_data>();
		}

		public heart_data(){
			res = 0;
		}

		public override void PutIn(){
			base.PutIn();
		}

		//生成的数据不可以直接发送，若要发送需要使用Proto.Serialize();
		public override Proto.ByteBufferData Serialize(){
			List<Proto.ByteBufferData> b_lst_s_d = Proto.ByteBufferDataListPool.Spawn();
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int16(res));
			return Proto.Unity_Bytes(b_lst_s_d);
		}

		//可以将对应的Proto_Package_Data中的数据更新到this里面
		public override bool Deserialize(Proto.ByteBufferData bytes,bool need_rev){
			bool iserr = false;
			Proto.ByteBufferData b_s_c_res = Proto.DesInt(bytes, ref iserr,need_rev);
			if (iserr){
				Proto.PutBackByteBufferData(b_s_c_res);
				return false;
			}
			b_s_c_res = Proto.Deserialize_Normal<Int16>(b_s_c_res, ref res, ref iserr,need_rev);
			Proto.PutBackByteBufferData(b_s_c_res);
			if (iserr){
				return false;
			}
			return true;
		}
	}

	public class log_data: Proto_Base{
		public string logStr;
		public Int32 processId;
		public Int32 index;

		public override object GetNew(){
			return Proto.GetOneProtoData<log_data>();
		}

		public log_data(){
			logStr = "";
			processId = 0;
			index = 0;
		}

		public override void PutIn(){
			base.PutIn();
			logStr = "";
		}

		//生成的数据不可以直接发送，若要发送需要使用Proto.Serialize();
		public override Proto.ByteBufferData Serialize(){
			List<Proto.ByteBufferData> b_lst_s_d = Proto.ByteBufferDataListPool.Spawn();
			b_lst_s_d.Add(Proto.SerializeT_Normal_string(logStr));
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int32(processId));
			b_lst_s_d.Add(Proto.SerializeT_Normal_Int32(index));
			return Proto.Unity_Bytes(b_lst_s_d);
		}

		//可以将对应的Proto_Package_Data中的数据更新到this里面
		public override bool Deserialize(Proto.ByteBufferData bytes,bool need_rev){
			bool iserr = false;
			Proto.ByteBufferData b_s_c_res = Proto.DesInt(bytes, ref iserr,need_rev);
			if (iserr){
				Proto.PutBackByteBufferData(b_s_c_res);
				return false;
			}
			b_s_c_res = Proto.Deserialize_Normal<string>(b_s_c_res, ref logStr, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<Int32>(b_s_c_res, ref processId, ref iserr,need_rev);
			b_s_c_res = Proto.Deserialize_Normal<Int32>(b_s_c_res, ref index, ref iserr, need_rev);
			Proto.PutBackByteBufferData(b_s_c_res);
			if (iserr){
				return false;
			}
			return true;
		}
	}
}
