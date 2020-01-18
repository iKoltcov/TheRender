using System;
using System.Numerics;
using TheRender.Entities.Interfaces;
using TheRender.Extensions;
using TheRender.Results;
using TheRender.Services;

namespace TheRender.Entities
{
    public class SphereLightEntity : ILight
    {
        private readonly int CountOfIteration = 10;

        private static readonly Random Random = new Random();
     
        private static readonly Object lockObject = new Object();
        
        public Vector3 Position { get; set; }
        
        public float Radius { get; set; }

        public ColorEntity Color { get; set; }

        public float Intensity { get; set; }

        public SphereLightEntity(Vector3 position, float radius, float intensity)
        {
            Position = position;
            Radius = radius;
            Intensity = intensity;
        }
        
        public LightIntensityResult? GetIlluminance(RayTracingService service, SceneIntersectResult intersect)
        {
            var result = new LightIntensityResult()
            {
                Specular = 0.0f,
                Diffuse = 0.0f,
            };
            
            for (int i = 0; i < CountOfIteration; i++)
            {
                var h = 2.0 * Random.NextDouble() - 1.0;
                var angleTh = Math.Asin(h);
                var angleFi = Random.NextDouble() * Math.PI * 2.0;
                var point = new Vector3(
                    (float)(Position.X + Radius * Math.Cos(angleTh) * Math.Cos(angleFi)),
                    (float)(Position.Y + Radius * Math.Cos(angleTh) * Math.Sin(angleFi)),
                    (float)(Position.Z + Radius * Math.Sin(angleTh)));
                
                var vectorToLight = point - intersect.Collision.Point;
                var directionToLight = vectorToLight.Normalize();
                var distanceToLight = vectorToLight.Length();

                var rayToLight = new RayEntity()
                {
                    Origin = Vector3.Dot(directionToLight, intersect.Collision.Normal) < 0
                        ? intersect.Collision.Point - directionToLight * MathHelper.Epsilon
                        : intersect.Collision.Point + directionToLight * MathHelper.Epsilon,
                    Direction = directionToLight
                };

                SceneIntersectResult castRayToLight;
                lock(lockObject)
                {
                    castRayToLight = service.SceneIntersect(rayToLight, distanceToLight);
                }
                
                if (castRayToLight?.Collision == null)
                {
                    result.Diffuse += (Intensity / distanceToLight * distanceToLight) * Math.Max(0.0f,
                        Vector3.Dot(directionToLight, intersect.Collision.Normal));
                    result.Specular += (float) Math.Pow(
                                           Math.Max(0.0f,
                                               -Vector3.Dot((-directionToLight).Reflect(intersect.Collision.Normal),
                                                   intersect.OriginRay.Direction)),
                                           intersect.Essence.Material.SpecularIntensity) * Intensity;
                }
            }

            if (Math.Abs(result.Diffuse) < MathHelper.Epsilon && Math.Abs(result.Diffuse) < MathHelper.Epsilon)
            {
                return null;
            }

            result.Diffuse /= CountOfIteration;
            result.Specular /= CountOfIteration;

            return result;
        }
    }
}
