using System.Numerics;
using TheRender.Entities;
using System;
namespace TheRender.Services
{
    public class IntersectionService
    {
        public bool PlaneHit;
        protected void IntersectionDistanceTest(Vector3 Origin, Vector3 PlaneIntersectionPoint, GeometryEntity Geometry,
             IntersectionEntity IntersectionPoint)
        {
            var Dist = (Origin - PlaneIntersectionPoint).Length();
            if (Dist < IntersectionPoint.Distance)
            {
                IntersectionPoint.Distance = Dist;
                IntersectionPoint.Material = Geometry.Material;
                IntersectionPoint.IntersectionCoordinate = PlaneIntersectionPoint;
            }
        }
        
        protected bool BaricTest(Vector3 A, Vector3 B, Vector3 C, Vector3 IntersectionPoint)
        {
            float Side1, Side2, Side3;
            Side1 = (float)Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2) + Math.Pow(A.Z - B.Z, 2));
            Side2 = (float)Math.Sqrt(Math.Pow(C.X - B.X, 2) + Math.Pow(C.Y - B.Y, 2) + Math.Pow(C.Z - B.Z, 2));
            Side3 = (float)Math.Sqrt(Math.Pow(A.X - C.X, 2) + Math.Pow(A.Y - C.Y, 2) + Math.Pow(A.Z - C.Z, 2));

            float IntersectionSide1, IntersectionSide2, IntersectionSide3;
            IntersectionSide1 = (float)Math.Sqrt(Math.Pow(A.X - IntersectionPoint.X, 2) + Math.Pow(A.Y - IntersectionPoint.Y, 2) + Math.Pow(A.Z - IntersectionPoint.Z, 2));
            IntersectionSide2 = (float)Math.Sqrt(Math.Pow(C.X - IntersectionPoint.X, 2) + Math.Pow(C.Y - IntersectionPoint.Y, 2) + Math.Pow(C.Z - IntersectionPoint.Z, 2));
            IntersectionSide3 = (float)Math.Sqrt(Math.Pow(B.X - IntersectionPoint.X, 2) + Math.Pow(B.Y - IntersectionPoint.Y, 2) + Math.Pow(B.Z - IntersectionPoint.Z, 2));
            
            float SemiPerimeter; //triangle semi-perimeter
            SemiPerimeter = (Side1 + Side2 + Side3) / 2;

            float Square; //square via Heron
            Square = (float)Math.Sqrt(SemiPerimeter * (SemiPerimeter - Side1) * (SemiPerimeter - Side2) * (SemiPerimeter - Side3));
            
            float IntersectionSemiPerimeter;  //temp semiperimeter
            float IntersectionSquare1, IntersectionSquare2, IntersectionSquare3; //temp squares

            IntersectionSemiPerimeter = (IntersectionSide1 + IntersectionSide3 + Side1) / 2;
            IntersectionSquare1 = IntersectionSemiPerimeter * (IntersectionSemiPerimeter - Side1) * (IntersectionSemiPerimeter - IntersectionSide1) * (IntersectionSemiPerimeter - IntersectionSide3);
            IntersectionSquare1 = IntersectionSquare1 > 0 ? (float)Math.Sqrt(IntersectionSquare1) : 0;
            //
            IntersectionSemiPerimeter = (IntersectionSide1 + IntersectionSide2 + Side2) / 2;
            IntersectionSquare2 = IntersectionSemiPerimeter * (IntersectionSemiPerimeter - Side2) * (IntersectionSemiPerimeter - IntersectionSide1) * (IntersectionSemiPerimeter - IntersectionSide2);
            IntersectionSquare2 = IntersectionSquare2 > 0 ? (float)Math.Sqrt(IntersectionSquare2) : 0;
            //
            IntersectionSemiPerimeter = (IntersectionSide2 + IntersectionSide3 + Side3) / 2;
            IntersectionSquare3 = IntersectionSemiPerimeter * (IntersectionSemiPerimeter - Side3) * (IntersectionSemiPerimeter - IntersectionSide2) * (IntersectionSemiPerimeter - IntersectionSide3);
            IntersectionSquare3 = IntersectionSquare3 > 0 ? (float)Math.Sqrt(IntersectionSquare3) : 0;

            float TestSquare;
            TestSquare = IntersectionSquare1 + IntersectionSquare2 + IntersectionSquare3;
            
            if (Math.Abs(TestSquare - Square) <= 0.01) //check 1st triangle using barycentric rule
            {
                return true;
            }

            return false;
        }
        
        public bool PlaneIntersectionTest (Vector3 A, Vector3 B, Vector3 C, Vector3 Origin, Vector3 Direction, GeometryEntity geometryIntersected,  IntersectionEntity IntersectionPoint)
        {
            Vector3 res, normal, viewRay, ray;
            double e, d;
            
            normal= Vector3.Cross(A-B, A-C);
            Vector3.Normalize(normal);
            viewRay = Origin - A;
            d = Vector3.Dot(normal, viewRay);
            ray = Origin - Direction;
            e = Vector3.Dot(normal, ray);

            if (e != 0f)
            {
                res=new Vector3((float)(Origin.X + ray.X*d/e), (float)(Origin.Y + ray.Y * d / e),
                    (float)(Origin.Z + ray.Z * d / e));
                if (BaricTest(A, B, C, res))
                {
                    IntersectionDistanceTest(Origin, res, geometryIntersected,  IntersectionPoint);
                    return true;
                }
            }

            return false;
        }

        public IntersectionService()
        {
            PlaneHit = false;
        }
    }
}