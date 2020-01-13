using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TheRender.Entities.Interfaces;
using TheRender.Extensions;
using TheRender.Results;

namespace TheRender.Entities
{
    public class QuadEntity : IGeometry, IEssence
    {
        public TriangleEntity[] Polygons { get; set; }

        public Vector3 Position { get; set; }
        
        public MaterialEntity Material { get; set; }

        public QuadEntity(Vector3 position, Vector3[] vertexes, MaterialEntity material)
        {
            Position = position;
            Material = material;

            var polygons = new List<TriangleEntity>
            {
                new TriangleEntity(
                    Position + vertexes[0],
                    Position + vertexes[1],
                    Position + vertexes[2],
                    Vector3.Cross(vertexes[1] - vertexes[0], vertexes[2] - vertexes[0]).Normalize()),
                new TriangleEntity(
                    Position + vertexes[0],
                    Position + vertexes[2],
                    Position + vertexes[3],
                    Vector3.Cross(vertexes[2] - vertexes[0], vertexes[3] - vertexes[0]).Normalize())
            };
            Polygons = polygons.ToArray();
        }
        
        public CollisionResult CheckCollision(RayEntity ray)
        {
            if (Polygons.Count() != 2)
            {
                throw new Exception("Quad must contain only two polygons!");
            }

            foreach (var polygon in Polygons)
            {
                var collision = ray.IntersectTriangle(polygon.VertexA, polygon.VertexB, polygon.VertexC, polygon.Normal);
                
                if (collision.HasValue)
                {
                    return new CollisionResult()
                    {
                        Point = collision.Value.Point,
                        Normal = polygon.Normal,
                    };
                }
            }

            return null;
        }
    }
}