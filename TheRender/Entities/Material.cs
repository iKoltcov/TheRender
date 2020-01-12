using System.Numerics;
namespace TheRender.Entities
{
    public class Material
    {
        //private Vector2 albedo; //diffuse and specular
        private Vector3 albedo;
        //X-diffuse component (kd)
        //Y-specular component (ks)
        //Z-reflective component
        private Vector3 diffuse_color;
        private float specular_exponent;    // value that controls the apparent smoothness of the surface

        public Material()
        {
            //albedo = new Vector2(1, 0);
            albedo = new Vector3(1, 0, 0);
            diffuse_color = new Vector3(1f, 1f, 1f);
            specular_exponent = 0;
        }

        //public Material(Vector2 a, Vector3 color, float spec):base()
        public Material(Vector3 a, Vector3 color, float spec) : base()
        {
            albedo = a;
            diffuse_color = color;
            specular_exponent = spec;
        }

        //public Vector2 Albedo
        public Vector3 Albedo
        {
            get { return albedo; }
        }
        public Vector3 Diffuse_color
        {
            get { return diffuse_color; }
        }
        public double Specular_exponent
        {
            get { return specular_exponent; }
        }
    }
}