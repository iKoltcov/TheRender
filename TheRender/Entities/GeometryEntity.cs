using  System.Numerics;
using System.Collections.Generic;

namespace TheRender.Entities
{
    public class GeometryEntity
    {
        public Vector3 Center { get; set; }
        public int DivisionHorizontal { get; set; } //horizontal surface division
        public int DivisionVertical { get; set; }    //vertical surface division
        public Material Material { get; set; }
        public List<Sector> Wireframe;

        public GeometryEntity(Vector3 c, Material m) : base()
        {
            Wireframe = new List<Sector>();
            DivisionHorizontal = 10;   //for example. Should be changed in constructors
            DivisionVertical = 10;
            Center = c;
            Material = m;
        }
    }
}