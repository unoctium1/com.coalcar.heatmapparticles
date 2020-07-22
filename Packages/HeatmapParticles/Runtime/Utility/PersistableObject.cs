using UnityEngine;

namespace HeatmapParticles
{
	namespace Utility
	{
		[DisallowMultipleComponent]
		public class PersistableObject : MonoBehaviour
		{
			public virtual void Save(DataWriter writer)
			{
				writer.Write(transform.localPosition);
				writer.Write(transform.localRotation);
				writer.Write(transform.localScale);
			}

			public virtual void Load(DataReader reader)
			{
				transform.localPosition = reader.ReadVector3();
				transform.localRotation = reader.ReadQuaternion();
				transform.localScale = reader.ReadVector3();
			}
		}
	}
}
