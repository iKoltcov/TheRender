using System;
using System.Collections.Generic;
using System.Numerics;
using TheRender.Entities.Interfaces;
using TheRender.Extensions;
using TheRender.Results;

namespace TheRender.Entities
{
    public class SphereEntity : IEssence, IGeometry
    {
        public Vector3 Position { get; set; }

        public float Radius { get; set; }
        
        public MaterialEntity Material { get; set; }

        public TriangleEntity[] Polygons { get; set; }

        public SphereEntity(Vector3 position, float radius)
        {
            Material = MaterialEntity.Default;
            Position = position;
            Radius = radius;
            
            var triangles = new List<TriangleEntity>();
            var stackStep = Math.PI * 0.1;
            var sectorStep = Math.PI * 0.1;
            
            for(var i = 0; i < 2.0 / stackStep; i++)
            {
                var h = -1.0 + i * stackStep;

                var th = Math.Asin(h);
                if(Double.IsNaN(th)){
                    continue;
                }

                var nextTh = Math.Asin(h + stackStep);
                if(Double.IsNaN(nextTh)){
                    nextTh = Math.Asin(1.0);
                }

                for(var j = 0; j < (2.0 * Math.PI) / sectorStep; j++){
                    var fi = sectorStep * j;
                    var A = new Vector3(
                        (float)(Position.X + Radius * Math.Cos(th) * Math.Cos(fi)),
                        (float)(Position.Y + Radius * Math.Sin(th)),
                        (float)(Position.Z + Radius * Math.Cos(th) * Math.Sin(fi)));
                    var C = new Vector3(
                        (float)(Position.X + Radius * Math.Cos(th) * Math.Cos(fi + sectorStep)),
                        (float)(Position.Y + Radius * Math.Sin(th)),
                        (float)(Position.Z + Radius * Math.Cos(th) * Math.Sin(fi + sectorStep)));
                    var B = new Vector3(
                        (float)(Position.X + Radius * Math.Cos(nextTh) * Math.Cos(fi)),
                        (float)(Position.Y + Radius * Math.Sin(nextTh)),
                        (float)(Position.Z + Radius * Math.Cos(nextTh) * Math.Sin(fi)));
                    var D = new Vector3(
                        (float)(Position.X + Radius * Math.Cos(nextTh) * Math.Cos(fi + sectorStep)),
                        (float)(Position.Y + Radius * Math.Sin(nextTh)),
                        (float)(Position.Z + Radius * Math.Cos(nextTh) * Math.Sin(fi + sectorStep)));

                    triangles.Add(new TriangleEntity(A, B, D, Vector3.Cross(B - A, D - A).Normalize()));
                    triangles.Add(new TriangleEntity(A, C, D, Vector3.Cross(D - A, C - A).Normalize()));
                }
            }

            Polygons = triangles.ToArray();
        }
        
        public CollisionResult CheckCollision(RayEntity ray)
        {
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
                //Normal = (collisionRef.Value.Point - Position).Normalize(),
                Point = collisionRef.Value.Point,
            };
        }
    }
}