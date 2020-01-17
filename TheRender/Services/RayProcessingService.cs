using System.Numerics;
using System;
using TheRender.Entities;
using System.Collections.Generic;

namespace TheRender.Services
{
    public class RayProcessingService
    {
        public RayProcessingService()
        { }
        public Vector3 ReflectRay(Vector3 L, Vector3 N)
        {
            var res = Vector3.Multiply(N, (float)Vector3.Dot(N, L) * 2f) - L;
            return Vector3.Normalize(res);
        }

        public Vector3 ReflectRayRandomly(Vector3 vector, Vector3 surfaceNormal)
        {
            var rndSeed = new Random();
            var res = vector;
            do
            {
                res.X *= (float) rndSeed.NextDouble() * 2 - 1;
                res.Y *= (float) rndSeed.NextDouble() * 2 - 1;
                res.Z *= (float) rndSeed.NextDouble() * 2 - 1;
            } while (Vector3.Dot(res,surfaceNormal)<0);

            return res;
        }
        public Vector3 ShadowOriginDeltaCalc(Vector3 lightDir, Vector3 sphereDist) //shift of shadow origin
        {
            return Vector3.Dot(lightDir, sphereDist) > 0 ? Vector3.Multiply(sphereDist, (float)1e-3) : Vector3.Multiply(sphereDist, (float)-1e-3);
        }
        public Vector3 ReflectionOriginCalc(Vector3 refDir, Vector3 sphereDist)
        {
            return Vector3.Dot(refDir, sphereDist) > 0 ? Vector3.Multiply(sphereDist, (float)1e-3) : Vector3.Multiply(sphereDist, (float)-1e-3);
        }

        public void NormalizeColorsByMax(IntersectionEntity pixelsHandler)
        {
            
        }
        public void GenerateColors(List<IntersectionEntity> pixelsHandler, SceneEntity world)
        {
            Vector3 point = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 N = new Vector3(0f, 0f, 0f);

            Vector3 result;

            //diff and spec
            double diffuse_light_intensity = 0, specular_light_intensity = 0;
            Vector3 lightDirection = new Vector3();
            double lightDistance = 0;
            //shadows
            Vector3 shadowOrigin = new Vector3();
            Vector3 shadowPoint = new Vector3();
            

            for (int i = 0; i < world.Lights.Count; i++)
            {

                lightDirection = world.Lights[i].Center - point;
                lightDistance = lightDirection.Length();
                lightDirection = Vector3.Normalize(lightDirection);

                for (int j = 0; j < world.WinHeight; j++)
                {
                    for (int k = 0; j < world.WinWidth; k++)
                    {
                        var pixelIntersected = world.PixelsHandler[j, k];
                        if(pixelIntersected.Hits==0)
                            continue;
                        //shadows
                        shadowOrigin = point + ShadowOriginDeltaCalc(lightDirection, pixelIntersected.FaceNormal);
                        if (world.PixelsHandler[j,k].Hits>0 && (shadowPoint - shadowOrigin).Length() < lightDistance)    //light behind geometry
                            continue;

                        //check if light's behind the object
                        if ((float) (Vector3.Dot(pixelIntersected.NormalizedViewDirection, pixelIntersected.FaceNormal) * Vector3.Dot(lightDirection, pixelIntersected.FaceNormal)) > 0f)
                            continue;

                        diffuse_light_intensity += world.Lights[i].Intensity * Math.Abs(Vector3.Dot(lightDirection, pixelIntersected.FaceNormal)) /
                                                   (float) Math.Pow(lightDistance, 2);
                        var refl = ReflectRay(lightDirection, pixelIntersected.FaceNormal);
                        var spec = -1f * Vector3.Dot(refl, pixelIntersected.NormalizedViewDirection);
                        var specComplete = Math.Pow(Math.Max(0f, spec), pixelIntersected.Material.SpecularExponent) *
                                           Math.Abs(Vector3.Dot(lightDirection, N)) /
                                           (float) Math.Pow(lightDistance, 2);
                        specular_light_intensity += specComplete * world.Lights[i].Intensity;

                        var diffuseComponent =
                            Vector3.Multiply(Vector3.Multiply(pixelIntersected.Material.DiffuseColor, (float) diffuse_light_intensity),
                                pixelIntersected.Material.Albedo.X);
                        var specularComponent = Vector3.Multiply(new Vector3(1f, 1f, 1f),
                            (float) specular_light_intensity * pixelIntersected.Material.Albedo.Y);
                        //return diffuseComponent + specularComponent + reflectiveComponent;
                        pixelIntersected.Color = diffuseComponent + specularComponent;
                    }
                }
            }
        }
    }
}