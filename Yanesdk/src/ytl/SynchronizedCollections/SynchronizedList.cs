using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Ytl
{
	/// <summary>
	/// Listの非同期対応版
	/// </summary>
	/// <remarks>
	/// ただしすべてのメソッドを用意してあるわけではない。
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public class SynchronizedList<T> : List<T>
	{
		/// <summary>
		/// [async] 配列のような表記でアクセスする
		/// </summary>
		public new T this[int key]
		{
			get
			{
				lock ( SyncObject )
					return base[key];
			}
			set
			{
				lock ( SyncObject )
					base[key] = value;
			}
		}

		/// <summary>
		/// [async] 要素を追加する。
		/// </summary>
		/// <param name="t"></param>
		public new void Add(T t)
		{
			lock ( SyncObject )
				base.Add(t);
		}

		/// <summary>
		/// [async] 要素をクリアする。
		/// </summary>
		/// <param name="t"></param>
		public new void Clear()
		{
			lock ( SyncObject )
				base.Clear();
		}


		/// <summary>
		/// 同期用オブジェクト
		/// </summary>
		/// <remarks>
		/// 上記のメンバだけでは足りないときは、この同期用オブジェクトを
		/// lockして、baseメンバを呼び出してください。
		/// </remarks>
		public object SyncObject = new object();
	}
}
