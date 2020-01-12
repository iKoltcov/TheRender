using System.Numerics;
using System.Collections.Generic;

namespace TheRender.Entities
{
    public class LightEntity
    {
        public double Intensity { get; set; }
        public Vector3 Center { get; set; }
        public LightEntity(Vector3 p, float i)
        {
            Center = p;
            Intensity = i;
        }
    }
}