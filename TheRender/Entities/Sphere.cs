using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TheRender.Entities
{
    public class Sphere:GeometryEntity
    {
        private float radius { get; set; }

        public Sphere(Vector3 c, float r, Material m) : base(c, m)
        {
            DivisionHorizontal = 160;
            DivisionVertical = 40;
            radius = r;

            createWireframe();
        }

        private void createWireframe()
        {
            float cir_len = (float)Math.PI * 2.0f * radius;
            float theta_stp = 2 * radius / (float)DivisionHorizontal;
            float phi_stp = cir_len / (float)DivisionVertical;    //phi - vertical layout
            Sector tmp = new Sector();
            for (float theta = -radius; theta < radius; theta += theta_stp)  //creating wireframe
            {
                for (float phi = 0; phi < 2.0 * Math.PI; phi += phi_stp)
                {
                    tmp = new Sector(radius, Center, theta, phi, theta_stp, phi_stp);// ЗАМЕНИТЬ КОНСТРУКТОР НА МЕТОД ИЛИ ВООБЩЕ ВЫНЕСТИ В СТАТИК!
                    Wireframe.Add(tmp);
                }
            }
        }


        public bool ray_intersect(Vector3 orig, Vector3 dir, ref double t0) //check if the ray intersects the sphere
        {
            Vector3 L = Center - orig;
            //float tca = L * dir;
            double tca = System.Numerics.Vector3.Dot(L, dir);
            //float d2 = L * L - tca * tca;
            double d2 = Vector3.Dot(L, L) - tca * tca;
            if (d2 > radius * radius) return false;
            double thc = Math.Sqrt(radius * radius - d2);
            t0 = tca - thc;
            double t1 = tca + thc;
            if (t0 < 0) t0 = t1;
            if (t0 < 0) return false;
            return true;
        }
    }
}