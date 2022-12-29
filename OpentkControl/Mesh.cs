using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpentkControl
{
    class Mesh
    {
        public float[] vertices;
        public uint[] indices;
        public int triCount;

        public float minX;
        public float minY;
        public float minZ;
        public float maxX;
        public float maxY;
        public float maxZ;


        public Mesh(float[] vertices, uint[] indices, int triCount)
        {
            this.vertices = vertices;
            this.indices = indices;
            minX = Min(0);
            minY = Min(1);
            minZ = Min(2);
            maxX = Max(0);
            maxY = Max(1);
            maxZ = Max(2);
        }

        public Mesh()
        {
            vertices = null;
            indices = null;
        }

        float Min(uint start)
        {
            if (start >= vertices.Length)
            {
                return -1;
            }
            float min = vertices[start];
            for (uint i = start; i < vertices.Length; i += 3)
            {
                min = Math.Min(min, vertices[i]);
            }
            return min;
        }

        float Max(uint start)
        {
            if (start >= vertices.Length)
            {
                return -1;
            }
            float max = vertices[start];
            for (uint i = start; i < vertices.Length; i += 3)
            {
                max = Math.Max(max, vertices[i]);
            }
            return max;
        }
    }
}
