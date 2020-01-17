using System.Numerics;
using  System.Collections.Generic;
using System;
using TheRender.Services;

namespace TheRender.Entities
{
    public class SceneEntity
    {
        public List<Sphere> SpheresCollection { get; set; }
        public List<LightEntity> Lights { get; set; }
        public List<Material> Materials { get; set; }
        public List<Sphere> Spheres { get; set; }
        public List<GeometryEntity> Geometries { get; set; }
        public  IntersectionEntity[,] PixelsHandler { get; set; }

        public double FOV;
        public int WinWidth, WinHeight;
        public Vector3 Direction { get; set; }
        public  Vector3 Cam { get; set; }
        public int NumOfRays { get; set; }
        public int NumOfBounces { get; set; }
        public void rayCasting(int WinWidth, int WinHeight)
        {
            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 direction = new Vector3(0, 0, 0);
            Vector3 res_color = new Vector3();
            Vector3 norm_dir = new Vector3();

            for (int j = 0; j < WinHeight; j++)
            {
                for (int i = 0; i < WinWidth; i++)
                {
                    direction.X = (float)((i + 0.5) - WinWidth / 2f);
                    direction.Y = (float)(-(j + 0.5) + WinHeight / 2f);
                    direction.Z = (float) (-WinHeight / (2f * Math.Tan(FOV / 2f)));

                    norm_dir = Vector3.Normalize(direction);
                    //res_color = TracerService.cast_ray(origin, ref norm_dir, SpheresCollection, Lights);

                }
            }
        }

        public List<Material> MaterialsCollectionGenerator()
        {
            List<Material> materialsLst = new List<Material>();
            Material background = new Material();
            materialsLst.Add(background);
            Material ivory = new Material(new Vector3(0.6f, 0.3f, 0.0f), new Vector3(0.4f, 0.4f, 0.3f), 50f);
            materialsLst.Add(ivory);
            Material rubber_green = new Material(new Vector3(0.9f, 0.1f, 0f), new Vector3(0.1f, 0.9f, 0.1f), 10f);
            materialsLst.Add(rubber_green);
            Material red_1 = new Material(new Vector3(0.45f, 0.5f, 0f), new Vector3(1f, 0f, 0f), 50);
            materialsLst.Add(red_1);
            Material red_2 = new Material(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 0f, 0f), 500);
            materialsLst.Add(red_2);

            return materialsLst;
        }

        //public List<Sphere> sphereCollGen()
        public List<GeometryEntity> GeometriesCollectionGenerator()
        {
            List<GeometryEntity> geometriesColl = new List<GeometryEntity>();
            Sphere temp = new Sphere(new Vector3(0, 0, -5), 1f, Materials[0]);
            geometriesColl.Add(temp);

            ////add more spheres here by modifying and copy-pasting temp
            temp = new Sphere(new Vector3(2, -2, -40), 20f, Materials[1]);
            geometriesColl.Add(temp);
            temp = new Sphere(new Vector3(-2, 2, -10), 1f, Materials[3]);
            geometriesColl.Add(temp);
            temp = new Sphere(new Vector3(-4, 4, -12), 1f, Materials[2]);
            geometriesColl.Add(temp);


            //return spheres;
            return geometriesColl;
        }

        public List<LightEntity> lightCollGen()
        {
            List<LightEntity> lights = new List<LightEntity>();
            //Light temp = new Light();

            //var temp = new LightEntity(new Vector3(5, 10, 20), 10f);
            var temp = new LightEntity(new Vector3(0, 3, 20), 100000f);
            lights.Add(temp);

            return lights;
        }

        public void GenerateIntersectionMatrix()
        {
            for (int i = 0; i < WinHeight; i++)
            {
                for (int j = 0; j < WinWidth; j++)
                {
                    PixelsHandler[i,j] = new IntersectionEntity(Cam);
                }
            }
        }
        
        public SceneEntity()
        {
            Cam = new Vector3();
            FOV = Math.PI / 2f;
            WinWidth = 512;
            WinHeight = 512;
            NumOfRays = 10;
            NumOfBounces = 10;
            this.Direction = new Vector3();

            PixelsHandler = new IntersectionEntity [WinHeight,WinWidth];
            GenerateIntersectionMatrix();
            Materials = MaterialsCollectionGenerator();
            Geometries = GeometriesCollectionGenerator();
            Lights = lightCollGen();
        }
    }
}