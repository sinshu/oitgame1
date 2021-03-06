using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Ytl
{
	/// <summary>
	/// DictionaryのSynchronized版
	/// </summary>
	/// <typeparam name="Key"></typeparam>
	/// <typeparam name="Value"></typeparam>
	public class SynchronizedDictionary<Key,Value> : Dictionary<Key , Value>
	{

		/// <summary>
		/// [async] keyを保持しているかどうか
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public new bool ContainsKey(Key key)
		{
			lock ( SyncObject )
			{
				return base.ContainsKey(key);
			}
		}

		/// <summary>
		/// [async] valueを保持しているかどうか
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public new bool ContainsValue(Value value)
		{
			lock ( SyncObject )
			{
				return base.ContainsValue(value);
			}
		}

		/// <summary>
		/// [async] 配列のような表記でアクセスする
		/// </summary>
		public new Value this[Key key]
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
		/// 同期用オブジェクト
		/// </summary>
		/// <remarks>
		/// 上記のメンバだけでは足りないときは、この同期用オブジェクトを
		/// lockして、baseメンバを呼び出してください。
		/// </remarks>
		public object SyncObject = new object();
	}
}
