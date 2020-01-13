using System.Numerics;

namespace TheRender.Entities
{
    public class TriangleEntity
    {
        public Vector3 VertexA { get; }

        public Vector3 VertexB { get; }

        public Vector3 VertexC { get; }

        public Vector3 Normal { get; }

        public TriangleEntity(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 normal)
        {
            VertexA = vertexA;
            VertexB = vertexB;
            VertexC = vertexC;
            Normal = normal;
        }

        public TriangleEntity(Vector3[] vertexes, Vector3 normal)
        {
            VertexA = vertexes[0];
            VertexB = vertexes[1];
            VertexC = vertexes[2];
            Normal = normal;
        }
        
        public TriangleEntity(Vector3[] vertexesAndNormal)
        {
            VertexA = vertexesAndNormal[0];
            VertexB = vertexesAndNormal[1];
            VertexC = vertexesAndNormal[2];
            Normal = vertexesAndNormal[3];
        }
    }
}