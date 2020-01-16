using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TheRender.Entities.Interfaces;
using TheRender.Extensions;
using TheRender.Results;

namespace TheRender.Entities
{
    public class CubeEntity : IGeometry, IEssence
    {
        public TriangleEntity[] Polygons { get; set; }

        public Vector3 Position { get; set; }
        
        public MaterialEntity Material { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sizes">Weight, Height, Depth</param>
        /// <param name="material"></param>
        public CubeEntity(Vector3 position, Vector3 sizes, MaterialEntity material)
        {
            Position = position;
            Material = material;

            var polygons = new List<TriangleEntity>
            {
                new TriangleEntity(
                    Position + new Vector3(sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, -sizes.Z),
                    new Vector3(0.0f, 0.0f, -1.0f)),
                new TriangleEntity(
                    Position + new Vector3(sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, -sizes.Z),
                    Position + new Vector3(sizes.X, -sizes.Y, -sizes.Z),
                    new Vector3(0.0f, 0.0f, -1.0f)),
                new TriangleEntity(
                    Position + new Vector3(sizes.X, sizes.Y, sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, sizes.Z),
                    new Vector3(0.0f, 0.0f, 1.0f)),
                new TriangleEntity(
                    Position + new Vector3(sizes.X, sizes.Y, sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, sizes.Z),
                    Position + new Vector3(sizes.X, -sizes.Y, sizes.Z),
                    new Vector3(0.0f, 0.0f, 1.0f)),
                new TriangleEntity(
                    Position + new Vector3(sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, sizes.Z),
                    new Vector3(0.0f, 1.0f, 0.0f)),              
                new TriangleEntity(
                    Position + new Vector3(sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, sizes.Z),
                    Position + new Vector3(sizes.X, sizes.Y, sizes.Z),
                    new Vector3(0.0f, 1.0f, 0.0f)),               
                new TriangleEntity(
                    Position + new Vector3(sizes.X, -sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, sizes.Z),
                    new Vector3(0.0f, -1.0f, 0.0f)),                
                new TriangleEntity(
                    Position + new Vector3(sizes.X, -sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, sizes.Z),
                    Position + new Vector3(sizes.X, -sizes.Y, sizes.Z),
                    new Vector3(0.0f, -1.0f, 0.0f)),
                new TriangleEntity(
                    Position + new Vector3(sizes.X, -sizes.Y, sizes.Z),
                    Position + new Vector3(sizes.X, -sizes.Y, -sizes.Z),
                    Position + new Vector3(sizes.X, sizes.Y, -sizes.Z),
                    new Vector3(1.0f, 0.0f, 0.0f)),
                new TriangleEntity(
                    Position + new Vector3(sizes.X, -sizes.Y, sizes.Z),
                    Position + new Vector3(sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(sizes.X, sizes.Y, sizes.Z),
                    new Vector3(1.0f, 0.0f, 0.0f)),
                new TriangleEntity(
                    Position + new Vector3(-sizes.X, -sizes.Y, sizes.Z),
                    Position + new Vector3(-sizes.X, -sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, -sizes.Z),
                    new Vector3(-1.0f, 0.0f, 0.0f)),
                new TriangleEntity(
                    Position + new Vector3(-sizes.X, -sizes.Y, sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, -sizes.Z),
                    Position + new Vector3(-sizes.X, sizes.Y, sizes.Z),
                    new Vector3(-1.0f, 0.0f, 0.0f)),
            };
            Polygons = polygons.ToArray();
        }
        
        public CollisionResult CheckCollision(RayEntity ray)
        {
//            if (Polygons.Count() != 12)
//            {
//                throw new Exception("Cube must contain only two polygons!");
//            }

            TriangleEntity triangleRef = null;
            IntersectTriangleResult? collisionRef = null;
            var minDistance = double.MaxValue;
            
            foreach (var polygon in Polygons)
            {
                var collision = ray.IntersectTriangle(polygon);
                
                if (collision.HasValue && minDistance > collision.Value.Distance)
                {
                    minDistance = collision.Value.Distance;
                    triangleRef = polygon;
                    collisionRef = collision;
                }
            }

            if (triangleRef == null)
            {
                return null;
            }
            
            return new CollisionResult()
            {
                Normal = triangleRef.Normal,
                Point = collisionRef.Value.Point,
            };
        }
    }
}