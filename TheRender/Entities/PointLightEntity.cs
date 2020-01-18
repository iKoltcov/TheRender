using System;
using System.Numerics;
using TheRender.Entities.Interfaces;
using TheRender.Extensions;
using TheRender.Results;
using TheRender.Services;

namespace TheRender.Entities
{
    public class PointLightEntity : ILight
    {
        public Vector3 Position { get; set; }

        public ColorEntity Color { get; set; }

        public float Intensity { get; set; }

        public LightIntensityResult? GetIlluminance(RayTracingService service, SceneIntersectResult intersect)
        {
            var vectorToLight = Position - intersect.Collision.Point;
            var directionToLight = vectorToLight.Normalize();
            var distanceToLight = vectorToLight.Length();

            var rayToLight = new RayEntity()
            {
                Origin = Vector3.Dot(directionToLight, intersect.Collision.Normal) < 0
                    ? intersect.Collision.Point - directionToLight * MathHelper.Epsilon
                    : intersect.Collision.Point + directionToLight * MathHelper.Epsilon,
                Direction = directionToLight
            };

            var castRayToLight = service.SceneIntersect(rayToLight, distanceToLight);

            if (castRayToLight?.Collision == null)
            {
                return new LightIntensityResult()
                   {
                       Diffuse = (Intensity / distanceToLight * distanceToLight) * Math.Max(0.0f, Vector3.Dot(directionToLight, intersect.Collision.Normal)),
                       Specular = (float) Math.Pow(Math.Max(0.0f, -Vector3.Dot((-directionToLight).Reflect(intersect.Collision.Normal), intersect.OriginRay.Direction)), 
                                      intersect.Essence.Material.SpecularIntensity) * Intensity,
                   };
            }

            return null;
        }
    }
}
