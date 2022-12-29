using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpentkControl
{
    class Loader
    {
        public Mesh LoadSTL(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                {
                    var header = reader.ReadLine();
                    if (header.StartsWith("solid"))
                    {
                        var line = reader.ReadLine().Trim();
                        if (line.StartsWith("facet") || line.StartsWith("endsolid"))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            return ReaderSTLAscii(stream);
                        }
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    return ReadSTLBinary(stream);
                }
            }
        }

        Mesh ReadSTLBinary(FileStream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                reader.ReadBytes(80);
                UInt32 triCount = reader.ReadUInt32();

                if (stream.Length != 84 + triCount * 50)
                {
                    Console.WriteLine("error: bad stl file.");
                    return null;
                }

                List<Vertex> vertices = new List<Vertex>(int.Parse((triCount * 3).ToString()));
                byte[] buffer = reader.ReadBytes(int.Parse((triCount * 50).ToString()));
                int pos = 3 * sizeof(float);
                int i = 0;
                while (i < triCount * 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var vertex = new Vertex();
                        vertex.x = BitConverter.ToSingle(buffer, pos);
                        pos += sizeof(float);
                        vertex.y = BitConverter.ToSingle(buffer, pos);
                        pos += sizeof(float);
                        vertex.z = BitConverter.ToSingle(buffer, pos);
                        pos += sizeof(float);
                        vertices.Add(vertex);
                        i++;
                    }
                    pos += (sizeof(UInt16) + 3 * sizeof(float));
                }
                return MeshFromVerts(vertices, int.Parse((triCount).ToString()));
            }
        }

        Mesh ReaderSTLAscii(FileStream stream)
        {
            List<Vertex> vertices = new List<Vertex>();
            int triangleCount = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
            {
                var length = reader.BaseStream.Length;
                reader.ReadLine();
                bool okay = true;
                while (!reader.EndOfStream && okay == true)
                {
                    var line = reader.ReadLine().Trim();
                    if (line.StartsWith("endsolid"))
                    {
                        break;
                    }
                    else if (!line.StartsWith("facet normal") || !reader.ReadLine().Trim().StartsWith("outer loop"))
                    {
                        okay = false;
                        break;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        var vertexLine = reader.ReadLine().Trim().Split(' ');
                        if (vertexLine[0] != "vertex")
                        {
                            okay = false;
                            break;
                        }
                        try
                        {
                            vertices.Add(new Vertex(float.Parse(vertexLine[1]), float.Parse(vertexLine[2]), float.Parse(vertexLine[3])));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            okay = false;
                            break;
                        }
                    }
                    if (!reader.ReadLine().Trim().StartsWith("endloop") || !reader.ReadLine().Trim().StartsWith("endfacet"))
                    {
                        okay = false;
                        break;
                    }
                    triangleCount++;
                }

                if (okay)
                {
                    return MeshFromVerts(vertices, triangleCount);
                }
                else
                {
                    Console.WriteLine("error: bad stl file.");
                    return null;
                }
            }
        }

        Mesh MeshFromVerts(List<Vertex> vertices, int triCount)
        {
            for (int i = 0; i < triCount * 3; i++)
            {
                var temp = vertices[i];
                temp.index = i;
                vertices[i] = temp;
            }
            vertices.Sort();
            uint[] indices = new uint[triCount * 3];
            int vertexCount = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertexCount == 0 || !vertices[i].Equals(vertices[vertexCount - 1]))
                {
                    vertices[vertexCount++] = vertices[i];
                }
                indices[vertices[i].index] = (uint)vertexCount - 1;
            }
            //foreach (var vertex in vertices)
            //{
            //    if (vertexCount == 0 || !vertex.Equals(vertices[vertexCount - 1]))
            //    {
            //        vertices[vertexCount++] = vertex;
            //    }
            //    indices[vertex.index] = (uint)vertexCount - 1;
            //}
            var count = vertices.Count;
            vertices.RemoveRange(vertexCount, count - vertexCount);

            float[] floatVerts = new float[vertexCount * 3];
            //foreach (var vertex in vertices)
            //{
            //    floatVerts.Add(vertex.x);
            //    floatVerts.Add(vertex.y);
            //    floatVerts.Add(vertex.z);
            //}
            int pos = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                floatVerts[pos++] = vertices[i].x;
                floatVerts[pos++] = vertices[i].y;
                floatVerts[pos++] = vertices[i].z;
            }
            return new Mesh(floatVerts, indices, triCount);
        }
    }
}
