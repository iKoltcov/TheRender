using System;
using System.Collections.Generic;
using System.Numerics;
using TheRender.Entities.Interfaces;
using TheRender.Extensions;

namespace TheRender.Entities
{
    public class SphereEntity : IEssence
    {
        public Vector3 Position { get; set; }

        public float Radius { get; set; }

        public MaterialEntity Material { get; set; }

        public List<Sector> Wireframe { get; set; }
        
        public Vector3? CheckCollision(RayEntity ray)
        {
            var oc = ray.Origin - Position;
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = Vector3.Dot(oc, ray.Direction) * 2.0f;
            var c = Vector3.Dot(oc, oc) - Radius * Radius;

            var discriminant = b * b - 4.0f * a * c;
            if(discriminant < 0)
            {
                return null;
            }

            var sqrtDiscriminant = (float)Math.Sqrt(discriminant);
            var numerator = -b - sqrtDiscriminant;
            if(numerator > 0.0f)
            {
                return ray.Origin + ray.Direction * (numerator / (2.0f * a));
            }

            numerator = -b + sqrtDiscriminant;
            if (numerator > 0.0f)
            {
                return ray.Origin + ray.Direction * (numerator / (2.0f * a));
            }

            return null;
        }

        public Vector3 GetNormal()
        {
            throw new NotImplementedException();
        }
        
        public Vector3 GetNormal(Vector3 intersect)
        {
            return (intersect - Position).Normalize();
        }
        
        private void createWireframe()
        {
            float cir_len = (float)Math.PI * 2.0f * Radius;
            float theta_stp = 2 * Radius / (float)160;
            float phi_stp = cir_len / (float)40;    //phi - vertical layout
            Sector tmp = new Sector();
            for (float theta = -Radius; theta < Radius; theta += theta_stp)  //creating wireframe
            {
                for (float phi = 0; phi < 2.0 * Math.PI; phi += phi_stp)
                {
                    tmp = new Sector(Radius, Position, theta, phi, theta_stp, phi_stp);// ЗАМЕНИТЬ КОНСТРУКТОР НА МЕТОД ИЛИ ВООБЩЕ ВЫНЕСТИ В СТАТИК!
                    Wireframe.Add(tmp);
                }
            }
        }
    }
}
