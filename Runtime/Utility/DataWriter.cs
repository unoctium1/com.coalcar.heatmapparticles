using System;
using System.IO;
using UnityEngine;

namespace HeatmapParticles
{
    namespace Utility
    {
        public class DataWriter
        {
            BinaryWriter writer;

            public DataWriter(BinaryWriter writer)
            {
                this.writer = writer;
            }

            public void Write(float value)
            {
                writer.Write(value);
            }

            public void Write(int value)
            {
                writer.Write(value);
            }

            public void Write(short value)
            {
                writer.Write(value);
            }

            public void Write(Quaternion value)
            {
                writer.Write(value.x);
                writer.Write(value.y);
                writer.Write(value.z);
                writer.Write(value.w);
            }

            public void Write(Vector3 value)
            {
                writer.Write(value.x);
                writer.Write(value.y);
                writer.Write(value.z);
            }

            public void Write(DateTime time)
            {
                writer.Write(time.ToFileTime());
            }
        }
    }
}
