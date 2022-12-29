using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpentkControl
{
    struct Vertex : IComparable
    {
        public float x, y, z;
        public int index;

        public Vertex(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            index = 0;
        }

        public int CompareTo(object obj)
        {
            Vertex vertex = (Vertex)obj;
            if (this.x != vertex.x)
            {
                if (this.x < vertex.x)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (this.y != vertex.y)
            {
                if (this.y < vertex.y)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else if (this.z != vertex.z)
            {
                if (this.z < vertex.z)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
