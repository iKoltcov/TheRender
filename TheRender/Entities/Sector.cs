using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace TheRender.Entities
{
    public class Sector
    {
       public Vector3 TopLeft { get; set; }
        public Vector3 TopRight { get; set; }
        public Vector3 BottomLeft { get; set; }
        public Vector3 BottomRight { get; set; }
        public int minHit { get; set; }
        public int maxHit { get; set; }
        public Vector3 Color { get; set; }

        public int hitCounter { get; set; }
        public Vector3 center { get; set; }
        public float sqr { get; set; } //square of the sector (via 2 triangles)
        public float getSqr { get { return sqr; } }
        public int HitCounter
        {
            get { return hitCounter; }
        }
        public void IncHitCounter()
        {
            hitCounter += 1;
        }
        public Sector()
        {
            TopLeft = new Vector3();
            TopRight = new Vector3();
            BottomLeft = new Vector3();
            BottomRight = new Vector3();

            Color = new Vector3();
            center = new Vector3(0, 0, -10);
            hitCounter = 0;
        }
        //public Sector(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) : base()
        //{
        //    TopLeft = p1;
        //    TopRight = p2;
        //    BottomLeft = p3;
        //    BottomRight = p4;
        //    calcSqr();
        //    //center = new Vector3((p1.X + p4.X) / 2, (p1.Y + p4.Y) / 2, (p1.Z + p4.Z) / 2);
        //}

        public Sector(float radius, Vector3 pos, float theta, float phi, float phi_stp, float theta_stp) : base()    //create sector for sphercal surface
        {
            //float phi_stp, theta_stp;
            center = pos;
            Vector3 p1, p2, p3, p4; // 
            //float cir_len = (float)Math.PI * 2.0f * radius;
            //phi_stp = cir_len / (float)div_vert;    //phi - vertical layout
            //theta_stp = 2 * radius / (float)div_hor;

            p1.X = center.X + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Cos(phi));
            p1.Y = center.Y + radius * (float)theta;
            p1.Z = center.Z + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Sin(phi));

            p2.X = center.X + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Cos(phi + phi_stp));
            p2.Y = center.Y + radius * (float)theta;
            p2.Z = center.Z + radius * (float)(Math.Sin(Math.Acos(theta)) * Math.Sin(phi + phi_stp));

            p3.X = center.X + radius * (float)(Math.Sin(Math.Acos(theta) + theta_stp) * Math.Cos(phi));
            p3.Y = center.Y + radius * (float)theta;
            p3.Z = center.Z + radius * (float)(Math.Sin(Math.Acos(theta) + theta_stp) * Math.Sin(phi));

            p4.X = center.X + radius * (float)(Math.Sin(Math.Acos(theta) + theta_stp) * Math.Cos(phi + phi_stp));
            p4.Y = center.Y + radius * (float)theta;
            p4.Z = center.Z + radius * (float)(Math.Sin(Math.Acos(theta) + theta_stp) * Math.Sin(phi + phi_stp));

            TopLeft = new Vector3(p1.X, p1.Y, p1.Z);
            TopRight = new Vector3(p2.X, p2.Y, p2.Z);
            BottomLeft = new Vector3(p3.X, p3.Y, p3.Z);
            BottomRight = new Vector3(p4.X, p4.Y, p4.Z);
            //Sector res = new Sector(p1, p2, p3, p4);
            //return res;
        }

        public bool intersectionTest_1(Vector3 p)
        {
            var N_1 = Vector3.Normalize(Vector3.Cross(TopLeft - BottomLeft, TopLeft - TopRight));   //1st triangle normal
            var N_2 = Vector3.Normalize(Vector3.Cross(BottomRight - BottomLeft, BottomRight - TopRight));   //2nd triangle normal

            var nom = Vector3.Dot(TopLeft - center, N_1);
            var denom = Vector3.Dot(p, N_1);
            var u = nom / denom;

            if (u >= 0) //1st triangle check
                return true;
            else
            {
                nom = Vector3.Dot(BottomRight - center, N_2);    //2nd triangle check
                denom = Vector3.Dot(p, N_2);
                u = nom / denom;
                if (u >= 0)
                    return true;
            }

            return false;   //no intersection found
        }

        
        
        public bool BaricTest(Vector3 p)
        {
            //sides of triangles
            float s1, s2, s3, s4, s5;
            s1 = (float)Math.Sqrt(Math.Pow(TopLeft.X - BottomLeft.X, 2) + Math.Pow(TopLeft.Y - BottomLeft.Y, 2) + Math.Pow(TopLeft.Z - BottomLeft.Z, 2));    //TL-BL
            s2 = (float)Math.Sqrt(Math.Pow(TopLeft.X - TopRight.X, 2) + Math.Pow(TopLeft.Y - TopRight.Y, 2) + Math.Pow(TopLeft.Z - TopRight.Z, 2));  //TL-TR
            s3 = (float)Math.Sqrt(Math.Pow(TopRight.X - BottomLeft.X, 2) + Math.Pow(TopRight.Y - BottomLeft.Y, 2) + Math.Pow(TopRight.Z - BottomLeft.Z, 2));  //TR-BL
            s4 = (float)Math.Sqrt(Math.Pow(BottomLeft.X - BottomRight.X, 2) + Math.Pow(BottomLeft.Y - BottomRight.Y, 2) + Math.Pow(BottomLeft.Z - BottomRight.Z, 2));  //BL-BR
            s5 = (float)Math.Sqrt(Math.Pow(TopRight.X - BottomRight.X, 2) + Math.Pow(TopRight.Y - BottomRight.Y, 2) + Math.Pow(TopRight.Z - BottomRight.Z, 2));  //TR-BR

            float sp1 = 0, sp2 = 0; //semi-perimeters
            sp1 = (s1 + s2 + s3) / 2;
            sp2 = (s3 + s4 + s5) / 2;

            float sq1 = 0, sq2 = 0; //Left and Right triangles squares via Heron
            sq1 = (float)Math.Sqrt(sp1 * (sp1 - s1) * (sp1 - s2) * (sp1 - s3));
            sq2 = (float)Math.Sqrt(sp2 * (sp2 - s3) * (sp2 - s4) * (sp2 - s5));

            //temporals
            float ts1, ts2, ts3;    //temp sides for inter triangles
            ts1 = (float)Math.Sqrt(Math.Pow(TopLeft.X - p.X, 2) + Math.Pow(TopLeft.Y - p.Y, 2) + Math.Pow(TopLeft.Z - p.Z, 2));
            ts2 = (float)Math.Sqrt(Math.Pow(BottomLeft.X - p.X, 2) + Math.Pow(BottomLeft.Y - p.Y, 2) + Math.Pow(BottomLeft.Z - p.Z, 2));
            ts3 = (float)Math.Sqrt(Math.Pow(TopRight.X - p.X, 2) + Math.Pow(TopRight.Y - p.Y, 2) + Math.Pow(TopRight.Z - p.Z, 2));

            float tsp = 0;  //temp semiperimeter
            float tsq1, tsq2, tsq3; //temp squares

            tsp = (ts1 + ts3 + s1) / 2;
            tsq1 = tsp * (tsp - s1) * (tsp - ts1) * (tsp - ts3);
            tsq1 = tsq1 > 0 ? (float)Math.Sqrt(tsq1) : 0;
            //
            tsp = (ts1 + ts2 + s2) / 2;
            tsq2 = tsp * (tsp - s2) * (tsp - ts1) * (tsp - ts2);
            tsq2 = tsq2 > 0 ? (float)Math.Sqrt(tsq2) : 0;
            //
            tsp = (ts2 + ts3 + s3) / 2;
            tsq3 = tsp * (tsp - s3) * (tsp - ts2) * (tsp - ts3);
            tsq3 = tsq3 > 0 ? (float)Math.Sqrt(tsq3) : 0;

            float tsq;
            tsq = tsq1 + tsq2 + tsq3;

            if (Math.Abs(tsq - sq1) <= 0.01) //check 1st triangle using barycentric rule
            {
                hitCounter += 1;
                return true;
            }
            else
            {
                ts1 = (float)Math.Sqrt(Math.Pow(TopRight.X - p.X, 2) + Math.Pow(TopRight.Y - p.Y, 2) + Math.Pow(TopRight.Z - p.Z, 2));
                ts2 = (float)Math.Sqrt(Math.Pow(BottomRight.X - p.X, 2) + Math.Pow(BottomRight.Y - p.Y, 2) + Math.Pow(BottomRight.Z - p.Z, 2));
                ts3 = (float)Math.Sqrt(Math.Pow(BottomLeft.X - p.X, 2) + Math.Pow(BottomLeft.Y - p.Y, 2) + Math.Pow(BottomLeft.Z - p.Z, 2));

                tsp = (ts1 + ts3 + s3) / 2;
                tsq1 = tsp * (tsp - s3) * (tsp - ts1) * (tsp - ts3);
                tsq1 = tsq1 > 0 ? (float)Math.Sqrt(tsq1) : 0;
                //
                tsp = (ts1 + ts2 + s5) / 2;
                tsq2 = tsp * (tsp - s5) * (tsp - ts1) * (tsp - ts2);
                tsq2 = tsq2 > 0 ? (float)Math.Sqrt(tsq2) : 0;
                //
                tsp = (ts2 + ts3 + s4) / 2;
                tsq3 = tsp * (tsp - s4) * (tsp - ts2) * (tsp - ts3);
                tsq3 = tsq3 > 0 ? (float)Math.Sqrt(tsq3) : 0;

                tsq = tsq1 + tsq2 + tsq3;

                if (Math.Abs(tsq - sq2) <= 0.01)
                {
                    hitCounter += 1;
                    return true;
                }
            }
            return false;
        }

        public void enlighten(float val)
        {
            float R, G, B;
            R = Color.X + val;
            G = Color.Y + val;
            B = Color.Z + val;

            Color = new Vector3(R, G, B);
        }
        public void calcSqr()
        {
            float s1, s2, s3, s4, s5;
            s1 = (TopLeft - BottomLeft).Length();    //TL-BL
            s2 = (TopLeft - TopRight).Length();  //TL-TR
            s3 = (TopRight - BottomLeft).Length();  //TR-BL
            s4 = (BottomLeft - BottomRight).Length();  //BL-BR
            s5 = (TopRight - BottomRight).Length();  //TR-BR

            float sp1 = 0, sp2 = 0; //semi-perimeters
            sp1 = (s1 + s2 + s3) / 2;
            sp2 = (s3 + s4 + s5) / 2;

            float sq1 = 0, sq2 = 0; //Left and Right triangles squares via Heron
            sq1 = (float)Math.Sqrt(sp1 * (sp1 - s1) * (sp1 - s2) * (sp1 - s3));
            sq2 = (float)Math.Sqrt(sp2 * (sp2 - s3) * (sp2 - s4) * (sp2 - s5));

            sqr = sq1 + sq2;
        } 
    }
}