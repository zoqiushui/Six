using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class Data : MonoBehaviour {

	public byte[] dataForSend;

		private byte[] a;

		private int b;

		private int c;

		private byte[] d;

		private static byte[] e;

		/// <summary>
		/// 只读消息体
		/// </summary>
		/// <param name="data"></param>
		public Data(byte[] data)
		{
			this.c = 16;
			this.a = data;
		}

		/// <summary>
		/// 传入0,从消息第一位读取
		/// </summary>
		/// <param name="data"></param>
		/// <param name="length"></param>
		public Data(byte[] data, int length)
		{
			this.c = length;
			this.a = data;
		}

		/// <summary>
		/// 发送消息体的长度
		/// </summary>
		/// <param name="length"></param>
        public Data(int length)
		{
			this.b = 0;
			this.dataForSend = new byte[length + 16];
		}

		public void Write_Byte(byte data)
		{
			this.d = new byte[1];
			this.d[0] = data;
			this.d.CopyTo(this.dataForSend, this.b);
			this.b += this.d.Length;
		}

		public void Write_Short(short data)
		{
			this.d = new byte[2];
			this.d = BitConverter.GetBytes(data);
			Array.Reverse(this.d);
			this.d.CopyTo(this.dataForSend, this.b);
			this.b += this.d.Length;
		}

		public void Write_Int(int data)
		{
			this.d = new byte[4];
			this.d = BitConverter.GetBytes(data);
			Array.Reverse(this.d);
			this.d.CopyTo(this.dataForSend, this.b);
			this.b += this.d.Length;
		}

		public void Write_Float(float data)
		{
			this.d = new byte[4];
			this.d = BitConverter.GetBytes(data);
			Array.Reverse(this.d);
			this.d.CopyTo(this.dataForSend, this.b);
			this.b += this.d.Length;
		}

		public void Write_String(string data)
		{
			this.d = Encoding.UTF8.GetBytes(data);
			this.d.CopyTo(this.dataForSend, this.b);
			this.b += this.d.Length;
		}

		public byte Read_Byte()
		{
            Data.e = new byte[1];
            Array.Copy(this.a, this.c, Data.e, 0, 1);
			this.c++;
            return Data.e[0];
		}

		public short Read_Short()
		{
            Data.e = new byte[2];
            Array.Copy(this.a, this.c, Data.e, 0, 2);
			this.c += 2;
            Array.Reverse(Data.e);
            return BitConverter.ToInt16(Data.e, 0);
		}

		public int Read_Int()
		{
            Data.e = new byte[4];
            Array.Copy(this.a, this.c, Data.e, 0, 4);
			this.c += 4;
            Array.Reverse(Data.e);
            return BitConverter.ToInt32(Data.e, 0);
		}

		public float Read_Float()
		{
            Data.e = new byte[4];
            Array.Copy(this.a, this.c, Data.e, 0, 4);
			this.c += 4;
            Array.Reverse(Data.e);
            return BitConverter.ToSingle(Data.e, 0);
		}

		public string Read_String(int length)
		{
            Data.e = new byte[length];
            Array.Copy(this.a, this.c, Data.e, 0, length);
			this.c += length;
            return Encoding.UTF8.GetString(Data.e);
		}
}
