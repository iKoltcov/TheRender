using System.Numerics;
using  System;

namespace TheRender.Entities
{
    public class Intersection
    {
        public uint Hits { get; set; }
        public double Distance { get; set; }
        public Vector3 IntersectionCoordinate { get; set; }
        public Vector3 FaceNormal { get; set; }
        public Material Material { get; set; }

        public Intersection(Vector3 cam)
        {
            Hits = 0;
            Distance = Double.MaxValue;
            IntersectionCoordinate = cam;
            FaceNormal = new Vector3();
            Material = new Material();
        }
        /*
         //not sure if needed at all
        public Intersection (bool hitFlag, double dst, Vector3 pointOfIntersection, Vector3 normalToFace, Material mat)
        {
            hit = hitFlag;
            distance = dst;
            IntersectionCoordinate = pointOfIntersection;
            FaceNormal = normalToFace;
            material = mat;
        }
        */
    }
}