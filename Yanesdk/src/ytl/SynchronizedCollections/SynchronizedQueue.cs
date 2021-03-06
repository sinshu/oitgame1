using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Ytl
{
	/// <summary>
	///	Enqueue/Dequeueに対して排他処理を行なうQueue
	/// 
	/// PC-queue(producer-consumer queue)で用いるのに使うと良い。
	/// </summary>
	public class SynchronizedQueue<T> : Queue<T>
	{
		/// <summary>
		///	[async]	Lockを行なうEnqueue
		/// </summary>
		public new void Enqueue(T t)
		{
			lock ( SyncObject )
			{
				base.Enqueue(t);
			}
		}

		/// <summary>
		///	[async]	Lockを行なうDequeue
		/// </summary>
		public new T Dequeue()
		{
			lock ( SyncObject )
			{
				return base.Dequeue();
			}
		}

		/// <summary>
		/// [async] Lockを行なうClear
		/// </summary>
		public new void Clear()
		{
			lock (SyncObject)
			{
				base.Clear();
			}
		}

		/// <summary>
		/// [async] Lockを行なうDequeue
		/// </summary>
		/// <remarks>
		/// 例外は投げない。Dequeueできないときはdefault(T)が戻る
		/// </remarks>
		/// <returns></returns>
		public T DequeueForced()
		{
			lock ( SyncObject )
			{
				if (base.Count != 0)
					return base.Dequeue();
				else
					return default(T);
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
