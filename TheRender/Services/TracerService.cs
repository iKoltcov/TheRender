using System.Numerics;
using System.Collections.Generic;
using TheRender.Entities;
using System;
using System.Linq;
using TheRender.Extensions;

namespace TheRender.Services
{
    public class TracerService
    {
        public Vector3 ReflectRayByPhong(Vector3 L, Vector3 N)
        {
            var res = Vector3.Multiply(N, (float)Vector3.Dot(N, L) * 2f) - L;
            return Vector3.Normalize(res);
        }
        public  bool scene_intersect(Vector3 orig, Vector3 dir, List<Sphere> spheres, ref Vector3 hit, ref Vector3 N, ref Material Material)
        {
            double spheres_dist = double.MaxValue;
            for (int i = 0; i < spheres.Count; i++)
            {
                double dist_i = 0;
                if (spheres[i].ray_intersect(orig, dir, ref dist_i) && dist_i < spheres_dist)
                {
                    spheres_dist = dist_i;
                    hit = orig + Vector3.Multiply(dir, (float)dist_i);  //hit = orig + dir* dist_i;
                    N = Vector3.Normalize(hit - spheres[i].Center);
                    Material = spheres[i].Material;
                }
            }
            return spheres_dist < Double.MaxValue;
        }

        public  Vector3 ShadowOriginDeltaCalc(Vector3 lightDir, Vector3 sphereDist) //shift of shadow origin
        {
            return Vector3.Dot(lightDir, sphereDist) > 0 ? Vector3.Multiply(sphereDist, (float)1e-3) : Vector3.Multiply(sphereDist, (float)-1e-3);
        }
        public  Vector3 ReflectionOriginCalc(Vector3 refDir, Vector3 sphereDist)
        {
            return Vector3.Dot(refDir, sphereDist) > 0 ? Vector3.Multiply(sphereDist, (float)1e-3) : Vector3.Multiply(sphereDist, (float)-1e-3);
        }
        //added reflection recursive depth limition
        public  Vector3 cast_ray(Vector3 orig, ref Vector3 dir, List<Sphere> spheres, List<LightEntity> lights, uint depth = 0)
        {
            Vector3 point = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 N = new Vector3(0f, 0f, 0f);
            Material Material = new Material();

            Vector3 result;

            //if (depth>2 || !scene_intersect(orig, dir, spheres, ref point, ref N, ref Material))
            if (!scene_intersect(orig, dir, spheres, ref point, ref N, ref Material))
            {
                return result = new Vector3(0.4f, 0.4f, 0.4f);
            }

            //diff and spec
            double diffuse_light_intensity = 0, specular_light_intensity = 0;
            Vector3 light_dir = new Vector3();
            double light_dist = 0;
            //shadows
            Vector3 shadowOrigin = new Vector3();
            Vector3 shadowPoint = new Vector3();
            Vector3 shadowNorm = new Vector3();
            Material shadowMat = new Material();
            //reflection
            Vector3 reflectionDir = ReflectRayByPhong(dir, N);
            Vector3 reflectionOrigin = point + ReflectionOriginCalc(reflectionDir, N);
            //Vector3 reflectionColor = cast_ray(reflectionOrigin, ref reflectionDir, spheres, lights, depth + 1);

            for (int i = 0; i < lights.Count; i++)
            {

                light_dir = lights[i].Center - point;
                light_dist = light_dir.Length();
                light_dir = Vector3.Normalize(light_dir);

                //shadows
                shadowOrigin = point + ShadowOriginDeltaCalc(light_dir, N);
                if (scene_intersect(shadowOrigin, light_dir, spheres, ref shadowPoint, ref shadowNorm, ref shadowMat) && (shadowPoint - shadowOrigin).Length() < light_dist)
                {
                    continue;
                }
                //check if light's behind the object
                if ((float)(Vector3.Dot(dir, N) * Vector3.Dot(light_dir, N)) > 0f)
                {
                    continue;
                }

                diffuse_light_intensity += lights[i].Intensity * Math.Abs(Vector3.Dot(light_dir, N)) / (float)Math.Pow(light_dist, 2);
                var refl = ReflectRayByPhong(light_dir, N);
                var spec = -1f * Vector3.Dot(refl, dir);
                var specComplete = Math.Pow(Math.Max(0f, spec), Material.SpecularExponent) * Math.Abs(Vector3.Dot(light_dir, N)) / (float)Math.Pow(light_dist, 2);
                specular_light_intensity += specComplete * lights[i].Intensity;
            }
            var diffuseComponent = Vector3.Multiply(Vector3.Multiply(Material.DiffuseColor, (float)diffuse_light_intensity), Material.Albedo.X);
            var specularComponent = Vector3.Multiply(new Vector3(1f, 1f, 1f), (float)specular_light_intensity * Material.Albedo.Y);
            //return diffuseComponent + specularComponent + reflectiveComponent;
            return diffuseComponent + specularComponent;
        }

        public  void generateColors()    //Create colors array
        {
            //create the scene
            SceneEntity world = new SceneEntity();
            //List<Illuminance> illuminances = new List<Illuminance>();
            double fov = world.FOV;
            int win_w = world.WinWidth, win_h = world.WinHeight;
            //pseudo-coords
            double dir_x = world.X, dir_y = world.Y, dir_z = world.Z;
            //rendering vars
            List<Vector3> color_buf = new List<Vector3>();
            List<Vector3> monochrome_buf = new List<Vector3>();

            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 direction = new Vector3(0, 0, 0);
            Vector3 res_color = new Vector3();
            Vector3 norm_dir = new Vector3();

            for (int j = 0; j < win_h; j++)
            {
                for (int i = 0; i < win_w; i++)
                {
                    dir_x = (i + 0.5) - win_w / 2f;
                    dir_y = -(j + 0.5) + win_h / 2f;
                    dir_z = -win_h / (2f * Math.Tan(fov / 2f));

                    direction.X = (float)dir_x;
                    direction.Y = (float)dir_y;
                    direction.Z = (float)dir_z;

                    var arr_idx = i + j * win_w;
                    norm_dir = Vector3.Normalize(direction);

                    res_color = TracerService.cast_ray(origin, ref norm_dir, world.Spheres, world.Lights);  //COLOR
                    color_buf.Add(res_color);
                }
            }
            float max;
            max = color_buf.Max(pixel => new[] { pixel.X, pixel.Y, pixel.Z }.Max());
            Vector3 tmp;
            Vector2 pos = new Vector2();
            for (int j = 0; j < win_h; j++)
            {
                for (int i = 0; i < win_w; i++)
                {
                    var arr_idx = i + j * win_w;    //list index [0; width*height]
                    pos.X = i;
                    pos.Y = j;
                    //with normalization on max
                    var red = (float)((color_buf[arr_idx].X / (max * 2f) * 255f));
                    var green = (float)((color_buf[arr_idx].Y / (max * 2f) * 255f));
                    var blue = (float)((color_buf[arr_idx].Z / (max * 2f) * 255f));
                    //w/t norm.
                    //var red = (float)((color_buf[arr_idx].X * 255f));
                    //var green = (float)((color_buf[arr_idx].Y * 255f));
                    //var blue = (float)((color_buf[arr_idx].Z * 255f));

                    //illuminances.Add(new Illuminance(new Vector2(i, j), new Vector3(red, green, blue)));
                }

            }

            //return illuminances;
        }
    }
}