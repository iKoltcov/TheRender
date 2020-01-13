using System.Numerics;
using  System;

namespace TheRender.Entities
{
    public class IntersectionEntity
    {
        public uint Hits { get; set; }
        public double Distance { get; set; }
        public Vector3 IntersectionCoordinate { get; set; }
        public Vector3 FaceNormal { get; set; }
        public Material Material { get; set; }
        public Vector3 Color { get; set; }

        public IntersectionEntity(Vector3 cam)
        {
            Hits = 0;
            Distance = Double.MaxValue;
            IntersectionCoordinate = cam;
            FaceNormal = new Vector3();
            Material = new Material();
            Color = new Vector3(0.4f,0.4f,0.4f);
        }
    }
}