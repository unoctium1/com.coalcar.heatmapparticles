using HeatmapParticles.Utility;
using System.IO;
using UnityEngine;

namespace HeatmapParticles
{
	[ExecuteInEditMode]
	public class Saver : MonoBehaviour
	{
		string savePath;


		void Awake()
		{
			savePath = Path.Combine(Application.persistentDataPath, "saveFile");
		}

		public void Save(PersistableObject o)
		{
			using (
				var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
			)
			{
				o.Save(new DataWriter(writer));
			}
		}

		public void Load(PersistableObject o)
		{
			using (
				var reader = new BinaryReader(File.Open(savePath, FileMode.Open))
			)
			{
				o.Load(new DataReader(reader));
			}
		}
	}
}
