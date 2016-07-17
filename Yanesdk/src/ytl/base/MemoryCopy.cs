﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Ytl
{
	public class MemoryCopy
	{
		#region テスト用のコード
		/*
			byte[] array = new byte[8];

			int n = 12345;
			short u;
			Yanesdk.Ytl.MemoryCopy.SetShort(array , 0 , n);
			u = Yanesdk.Ytl.MemoryCopy.GetShort(array,0);
			if ( u != n )
				throw null;

			n = -12345;
			Yanesdk.Ytl.MemoryCopy.SetShort(array , 0 , n);
			u = Yanesdk.Ytl.MemoryCopy.GetShort(array , 0);
			if ( u != n )
				throw null;

			n = 12345;
			Yanesdk.Ytl.MemoryCopy.SetUShort(array , 0 , n);
			ushort us;
			us = Yanesdk.Ytl.MemoryCopy.GetUShort(array , 0);
			if ( us != n )
				throw null;

			n = 65000;
			Yanesdk.Ytl.MemoryCopy.SetUShort(array , 0 , n);
			us = Yanesdk.Ytl.MemoryCopy.GetUShort(array , 0);
			if ( us != n )
				throw null;

			n = 12345678;
			int nn;
			Yanesdk.Ytl.MemoryCopy.SetInt(array , 0 , n);
			nn = Yanesdk.Ytl.MemoryCopy.GetInt(array , 0);
			if ( nn != n )
				throw null;

			n = -n;
			Yanesdk.Ytl.MemoryCopy.SetInt(array , 0 , n);
			nn = Yanesdk.Ytl.MemoryCopy.GetInt(array , 0);
			if ( nn != n )
				throw null;

			ulong l;
			l = 123456781234567;
			ulong ll;
			Yanesdk.Ytl.MemoryCopy.SetShortLong(array , 0 , l);
			ll = Yanesdk.Ytl.MemoryCopy.GetShortLong(array , 0);
			if ( ll != l )
				throw null;

			l = 0 -l;
			Yanesdk.Ytl.MemoryCopy.SetShortLong(array , 0 , l);
			ll = Yanesdk.Ytl.MemoryCopy.GetShortLong(array , 0);
			if ( (l & 0xffffffffffff) != ll )
				throw null;
		*/		
		#endregion

		/// <summary>
		/// data[index]～data[index+length]までを0で埋める。
		/// data配列のサイズチェック等はしていない。
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="length"></param>
		public static void FillZero(byte[] data , int index , int length)
		{
			for ( int i = 0 ; i < length ; ++i )
				data[i + index] = 0;
		}

		/// <summary>
		/// メモリコピーを行なう
		/// 
		/// dst : 転送先
		/// src : 転送元
		/// サイズチェックは行なっていない
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="src"></param>
		/// <param name="start"></param>
		/// <param name="size"></param>
		public static void MemCopy(byte[] dst , int dst_start , byte[] src , int src_start , int size)
		{
			for ( int i = 0 ; i < size ; ++i )
			{
				dst[dst_start + i] = src[src_start + i];
			}
		}

		/// <summary>
		/// dstとsrcの比較。
		/// 
		/// サイズチェックは行なっていない
		/// </summary>
		/// <param name="dst"></param>
		/// <param name="dst_start"></param>
		/// <param name="src"></param>
		/// <param name="src_start"></param>
		/// <param name="size"></param>
		/// <returns>一致すればtrue。</returns>
		public static bool Compare(byte[] dst , int dst_start , byte[] src , int src_start , int size)
		{
			for ( int i = 0 ; i < size ; ++i )
			{
				if ( dst[dst_start + i] != src[src_start + i] )
					return false;
			}
			return true;
		}

		#region byte[]の一部をbyte,short,int,shortlong(6バイト),long,stringに分解,結合

		/// <summary>
		/// </summary>
		/// <remarks>
		/// こんなメソッドいらないけど、対称性を保つために用意する。
		///	</remarks>
		/// <param name="a"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static byte GetByte(byte[] a , int index)
		{
			return a[index];
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでShort化
		/// </summary>
		/// <param name="a"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static short GetShort(byte[] a , int index)
		{
			return ( short ) ( a[index] + ( a[index + 1] << 8 ) );
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでUShort化
		/// </summary>
		/// <param name="a"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		private static ushort GetUShort(byte[] a , int index)
		{
			return ( ushort ) ( a[index] + ( a[index + 1] << 8 ) );
		}
		// ushortがCLSCompliantにならないのでprivateにしておく。

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでint化
		/// </summary>
		/// <param name="a"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static int GetInt(byte[] a , int index)
		{
			return a[index] + ( a[index + 1] << 8 ) + ( a[index + 2] << 16 ) + ( a[index + 3] << 24 );
			// singed型の符号拡張には十分気をつける必要がある。
			// byteは無符号なので気にしなくてok
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでshortlong(6バイト)化
		/// 
		/// ShortLong型は架空の無符号6バイト整数型。
		/// </summary>
		/// <param name="a"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static long GetShortLong(byte[] a , int index)
		{
			ulong u = (uint)GetInt(a , index);
			u += ( ulong ) GetUShort(a , index + 4) << 32;
			// singed型の符号拡張には十分気をつける必要がある。
			// intはuintにcastしてから代入。UShortのほうは無符号なので気にしなくて良い。

			return (long)u;
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでlong化
		/// </summary>
		/// <param name="a"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public static long GetLong(byte[] a , int index)
		{
			ulong u = (uint)GetInt(a , index);
			u += (ulong)GetInt(a , index + 4) << 32;
			// singed型の符号拡張には十分気をつける必要がある。
			// intはuintにcastしてから代入。intのほうはulongにcastいきなりcast。
			// これは本来してはいけないが(符号拡張のとき上位ビットがまきぞえを食う)
			// その直後に32回シフトするので、無視される。

			return (long)u;
		}

		/// <summary>
		/// SetStringの逆の動作。
		/// data配列のindexの範囲チェックは行なっていない。
		/// 
		/// data[index]～data[index+length*2]の
		/// 範囲内に (char)0が出てきたら、文字列はそこでおしまい。
		/// (出てこなくても良い)
		/// </summary>
		/// <param name="s"></param>
		/// <param name="data"></param>
		/// <param name="start_index"></param>
		/// <param name="length"></param>
		public static string GetString(byte[] data , int index , int length)
		{
			StringBuilder b = new StringBuilder();

			for ( int i = 0 ; i < length ; ++i )
			{
				char c;
				c = (char)
					(data[index + i * 2 + 0] + 
					(data[index + i * 2 + 1] << 8));
				if ( c == 0 )
					break; // ここまででok
				b.Append(c);
			}
			return b.ToString();
		}

		/// <summary>
		/// SetStringAndLengthの逆の動作。
		/// data配列のindexの範囲チェックは行なっていない。
		/// 
		/// data[index]～data[4 + index+length*2]の
		/// 範囲内に (char)0が出てきたら、文字列はそこでおしまい。
		/// (出てこなくても良い)
		/// </summary>
		public static string GetStringAndLength(byte[] data, int index)
		{
			int length = GetInt(data, index);

			return GetString(data, index + 4, length);
		}


		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでbyteデータnの書き込み
		/// </summary>
		/// <param name="a"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static byte SetByte(byte[] a , int index , int n)
		{
			return a[index] = ( byte ) n;
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでShortデータnの書き込み
		/// </summary>
		public static void SetShort(byte[] a , int index , int n)
		{
			a[index + 0] = ( byte ) ( n & 0xff );
			a[index + 1] = ( byte ) ( ( n >> 8 ) & 0xff );
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでUShortデータnの書き込み
		/// </summary>
		public static void SetUShort(byte[] a , int index , int n)
		{
			a[index + 0] = ( byte ) ( n & 0xff );
			a[index + 1] = ( byte ) ( ( n >> 8 ) & 0xff );
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでIntデータnの書き込み
		/// </summary>
		public static void SetInt(byte[] a , int index , int n)
		{
			a[index + 0] = ( byte ) ( n & 0xff );
			a[index + 1] = ( byte ) ( ( n >> 8 ) & 0xff );
			a[index + 2] = ( byte ) ( ( n >> 16 ) & 0xff );
			a[index + 3] = ( byte ) ( ( n >> 24 ) & 0xff );

			// 符号型の右シフトでは最上位bitがcopyされてくることに
			// 留意しなくてはならない。

			// この場合、直後にmaskを取っているので、その部分は無視できる。
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでShortLong(6バイト)データnの書き込み
		/// 
		/// ShortLong型は架空の無符号6バイト整数型。
		/// </summary>
		public static void SetShortLong(byte[] a , int index , long n)
		{
			SetInt(a , index , ( int ) n);
			SetShort(a , index + 4 , ( ushort ) ( n >> 32 ));

			// 符号型の右シフトでは最上位bitがcopyされてくることに
			// 留意しなくてはならない。

			// この場合、直後にushortに切り落としているので、その部分は無視できる。
		}

		/// <summary>
		/// 配列の指定の位置からリトルエンディアンでlongデータnの書き込み
		/// </summary>
		public static void SetLong(byte[] a , int index , long n)
		{
			SetInt(a , index , ( int ) n);
			SetInt(a , index+4 , (int)(n >> 32));

			// 符号型の右シフトでは最上位bitがcopyされてくることに
			// 留意しなくてはならない。

			// この場合、直後にintに切り落としているので、その部分は無視できる。
		}

		/// <summary>
		/// stringからbyte[]へのコピー。
		/// little endianで保存。
		/// data配列は十分に確保してあると仮定。
		/// data[index]～data[index + length*2 ]までに格納される。
		/// 
		/// stringがnullおよび、文字列長さが足りないときは、
		/// data配列の該当部分は0で埋めることが保証される。
		/// </summary>
		/// <param name="s"></param>
		/// <param name="data"></param>
		/// <param name="start_index"></param>
		/// <param name="length"></param>
		public static void SetString(byte[] data , int index , string s , int length)
		{
			if ( s == null )
			{
				FillZero(data , index , length * 2);
				return;
			}
			for ( int i = 0 ; i < length ; ++i )
			{
				if ( i >= s.Length )
				{
					// 文字が足りなかったので残りをZeroFillして終了
					FillZero(data , index + i * 2 , ( length - i ) * 2);
					return;
				}
				char c = s[i];
				data[index + i * 2 + 0] = ( byte ) ( c & 0xff );
				data[index + i * 2 + 1] = ( byte ) ( c >> 8 );
			}
		//	// 終端の (char)0
		//	data[index + length * 2 + 0] = 0;
		//	data[index + length * 2 + 1] = 0;
		}

		/// <summary>
		/// stringからbyte[]へのコピー。
		/// little endianで保存。
		/// data配列は十分に確保してあると仮定。
		/// data[index]～data[4 + index + length*2 ]までに格納される。
		/// 
		/// このときの4は、stringのサイズを表すsizeof(int) == 4。
		/// このとき埋め込まれる値は、文字列の長さ(== 文字列自体のbyte数の半分)
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="s"></param>
		public static void SetStringAndLength(byte[] data, int index, string s)
		{
			// 文字数を埋める
			SetInt(data, index, s.Length);

			// 次に文字列本体を埋める。これで復元できる
			SetString(data, index + 4, s , s.Length);
		}

		#endregion

		#region byte,short,int,longをbyte[]と相互変換
		/// <summary>
		/// data配列のLengthが1ならbyte,2ならshort,4ならint,8ならlongとして
		/// 数値化。数値化するときはリトルエンディアンと仮定。
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static long ToNum(byte[] data)
		{
			// ulongで計算しないとMSBの符号が再現されない
			ulong n = 0;
			int shifter = 0;
			foreach (byte b in data)
			{
				n += ((ulong)b << shifter);
				shifter += 8; // こんなことせんといかんのか(´ω`)
			}

			switch (data.Length)
			{
				case 1:
					return (byte)n;
				case 2:
					return (short)n;
				case 4:
					return (int)n;
				case 8:
					return (long)n;
				default: // これエラーでもいいのだが(´ω`)
					return (long)n; 
			}
		}

		/// <summary>
		/// 配列の特定の場所(index)から、sizeバイトのデータをToNum化する
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="size"></param>
		public static long ToNum(byte[] data,int index,int size)
		{
			byte[] d = new byte[size];
			MemCopy(d, 0, data, index, size);
			return ToNum(d);
		}

		/// <summary>
		/// ToNumの逆変換
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ToByteArrayFromByte(short b)
		{
			return ToByteHelper((ulong)b, 2);
		}

		/// <summary>
		/// ToNumの逆変換
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ToByteArrayFromShort(short s)
		{
			return ToByteHelper((ulong)s, 2);
		}

		/// <summary>
		/// ToNumの逆変換
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ToByteArrayFromInt(int i)
		{
			return ToByteHelper((ulong)i, 4);
		}

		/// <summary>
		/// ToNumの逆変換
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static byte[] ToByteArrayFromLong(long l)
		{
			return ToByteHelper((ulong)l, 8);
		}

		/// <summary>
		/// ToByteArrayXXXXのヘルパ
		/// </summary>
		/// <param name="l"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		private static byte[] ToByteHelper(ulong l, int size)
		{
			byte[] by = new byte[size];
			for (int i = 0; i < size; ++i)
			{
				by[i] = (byte)(l & 0xff);
				l >>= 8;
			}
			return by;
		}

		/// <summary>
		/// ToByteArrayの配列の一部に値が書き込まれる版
		/// </summary>
		/// <remarks>
		/// 範囲チェックは行なっていない。
		/// </remarks>
		/// <param name="l"></param>
		/// <param name="a"></param>
		/// <param name="index"></param>
		/// <param name="size"></param>
		public static void WriteToByteArray(long l, byte[] a, int index, int size)
		{
			ulong l2 = (ulong)l;
			for (int i = 0; i < size; ++i)
			{
				a[index + i] = (byte)(l2 & 0xff);
				l2 >>= 8;
			}
		}

		#endregion

	}
}
