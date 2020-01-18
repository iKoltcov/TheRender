using System;
using System.Numerics;
using TheRender.Entities;
using TheRender.Results;

namespace TheRender.Extensions
{
    public static class MathHelper
    {
        private static readonly Random Random = new Random();

        public static readonly float Epsilon = 1e-3f;
        public static Vector3 Normalize(this Vector3 vector)
        {
            return Vector3.Normalize(vector);
        }
        
        public static Vector3 Reflect(this Vector3 vector, Vector3 normal) {
            return vector - normal * 2.0f * Vector3.Dot(vector, normal);
        }
        
        public static Vector3 DiffuseReflect(Vector3 normal) {
            var h = 2.0 * Random.NextDouble() - 1.0;
            var angleTh = Math.Asin(h);
            var angleFi = Random.NextDouble() * Math.PI * 2.0;

            var direction = new Vector3(
                (float)(Math.Cos(angleTh) * Math.Cos(angleFi)),
                (float)(Math.Cos(angleTh) * Math.Sin(angleFi)),
                (float)Math.Sin(angleTh));
            direction += normal;
            
            return direction.Normalize();
        }
        
        public static IntersectTriangleResult? IntersectTriangle(this RayEntity rayEntity, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 normal) 
        {
            var denom = Vector3.Dot(rayEntity.Direction, normal);
            if (Math.Abs(denom) < double.Epsilon)
            {
                return null;
            }
            var t = -(Vector3.Dot(vertexA, -normal) + Vector3.Dot(rayEntity.Origin, normal)) / denom;
            
            if (t < 0)
            {
                return null;
            }

            var point = rayEntity.Origin + rayEntity.Direction * t;
            if(point.IntersectTriangle(vertexA, vertexB, vertexC))
            {
                return new IntersectTriangleResult()
                {
                    Point = point,
                    Distance = t,
                };
            }

            return null;
        }

        public static IntersectTriangleResult? IntersectTriangle(this RayEntity rayEntity, TriangleEntity polygon)
        {
            return rayEntity.IntersectTriangle(polygon.VertexA, polygon.VertexB, polygon.VertexC, polygon.Normal);
        }
        
        private static bool IntersectTriangle(this Vector3 point, Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
        {
            var v0 = vertexC - vertexA;
            var v1 = vertexB - vertexA;
            var v2 = point - vertexA;

            var dot00 = Vector3.Dot(v0, v0);
            var dot01 = Vector3.Dot(v0, v1);
            var dot02 = Vector3.Dot(v0, v2);
            var dot11 = Vector3.Dot(v1, v1);
            var dot12 = Vector3.Dot(v1, v2);

            var invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
            var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            if (u <= 0) 
            {
                return false;
            }

            var v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            return (v > 0) && (u + v < 1.0f);
        }
    }
}
