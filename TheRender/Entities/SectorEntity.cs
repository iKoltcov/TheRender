using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TheRender.Entities
{
    public class SectorEntity
    {
       public Vector3 TopLeft { get; set; }
        public Vector3 TopRight { get; set; }
        public Vector3 Bottom { get; set; }
        public Vector3 GeometryCenter { get; set; }
        public SectorEntity(float radius, Vector3 pos, float theta, float phi, float phi_stp, float theta_stp)    //create sector for sphercal surface
        {
            //float phi_stp, theta_stp;
            GeometryCenter = pos;
            Vector3 p1, p2, p3, p4; // 

            p1.X = GeometryCenter.X + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Cos(phi));
            p1.Y = GeometryCenter.Y + radius * (float)theta;
            p1.Z = GeometryCenter.Z + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Sin(phi));

            p2.X = GeometryCenter.X + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Cos(phi + phi_stp));
            p2.Y = GeometryCenter.Y + radius * (float)theta;
            p2.Z = GeometryCenter.Z + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Sin(phi + phi_stp));

            p3.X = GeometryCenter.X + radius * (float)(Math.Sin(Math.Acos(theta) + theta_stp) * Math.Cos(phi));
            p3.Y = GeometryCenter.Y + radius * (float)theta;
            p3.Z = GeometryCenter.Z + radius * (float)(Math.Sin(Math.Acos(theta) + theta_stp) * Math.Sin(phi));

            TopLeft = new Vector3(p1.X, p1.Y, p1.Z);
            TopRight = new Vector3(p2.X, p2.Y, p2.Z);
            Bottom = new Vector3(p3.X, p3.Y, p3.Z);
        }
        //
        // public bool intersectionTest_1(Vector3 p)
        // {
        //     var N_1 = Vector3.Normalize(Vector3.Cross(TopLeft - Bottom, TopLeft - TopRight));   //1st triangle normal
        //     var N_2 = Vector3.Normalize(Vector3.Cross(BottomRight - Bottom, BottomRight - TopRight));   //2nd triangle normal
        //
        //     var nom = Vector3.Dot(TopLeft - GeometryCenter, N_1);
        //     var denom = Vector3.Dot(p, N_1);
        //     var u = nom / denom;
        //
        //     if (u >= 0) //1st triangle check
        //         return true;
        //     else
        //     {
        //         nom = Vector3.Dot(BottomRight - GeometryCenter, N_2);    //2nd triangle check
        //         denom = Vector3.Dot(p, N_2);
        //         u = nom / denom;
        //         if (u >= 0)
        //             return true;
        //     }
        //
        //     return false;   //no intersection found
        // }
    }
}