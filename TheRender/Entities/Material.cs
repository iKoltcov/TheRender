using System.Numerics;
namespace TheRender.Entities
{
    public class Material
    {
        //public Vector2 Albedo; //diffuse and specular
        public Vector3 Albedo { get; set; }
        //X-diffuse component (kd)
        //Y-specular component (ks)
        //Z-reflective component
        public Vector3 DiffuseColor { get; set; }
        public float SpecularExponent { get; set; }    // value that controls the apparent smoothness of the surface

        public Material()
        {
            Albedo = new Vector3(1, 0, 0);
            DiffuseColor = new Vector3(0.4f, 0.4f, 0.7f);
            SpecularExponent = 0;
        }

        //public Material(Vector2 a, Vector3 color, float spec):base()
        public Material(Vector3 a, Vector3 color, float spec) : base()
        {
            Albedo = a;
            DiffuseColor = color;
            SpecularExponent = spec;
        }
    }
}