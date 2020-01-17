using System;
using System.Collections.Generic;
using TheRender.Entities;
using System.Numerics;
using System.Threading;
using TheRender.Entities.Interfaces;
using TheRender.Services.Interfaces;

namespace TheRender.Services
{
    public class BackwardRayTracingService
    {
        private IntersectionService IntersectionService { get; }
        
        public BackwardRayTracingService()
        {
            IntersectionService = new IntersectionService();        
        }
        
        //private void IntersectionCheck(SceneEntity world, IntersectionEntity screenCell, Vector3 rayCasted, uint camRayIndex)
        public bool IntersectionCheck(SceneEntity world, IntersectionEntity screenCell, Vector3 rayCasted, Vector3 rayOrigin)
        {
            bool intersectionDetected = false;
            for (int i = 0; i < world.Geometries.Count; i++)
            {
                var geometry = world.Geometries[i];
                for (int j = 0; j < geometry.Wireframe.Count; j++)    //parsin' array of geometry's sectors
                {
                    var sector = geometry.Wireframe[i];
                    if (IntersectionService.PlaneIntersectionTest(sector.TopLeft, sector.TopRight, sector.Bottom, rayOrigin, rayCasted, geometry, screenCell))
                    {
                        intersectionDetected = true;
                        break;    //choose next geometry from the list (searchin' for the closest)
                    }
                }
            }

            if (intersectionDetected == false)    //backbround hitted
                screenCell.Color = world.Materials[0].DiffuseColor;
            return intersectionDetected;
        }

        public Vector3[] GenerateRaysBunch(int heightIndex, int widthIndex, SceneEntity world)
        {
            var arrayOfRays= new Vector3[world.NumOfRays];
            for (int i = 0; i < world.NumOfRays; i++)
            {
                var DirectionX = (float) ((widthIndex + 0.5) - world.WinWidth / 2f);
                var DirectionY = (float) (-(heightIndex + 0.5) + world.WinHeight / 2f);
                var DirectionZ = (float) (-world.WinHeight / (2f * Math.Tan(world.FOV / 2f)));
                var rayDirection = new Vector3(DirectionX,DirectionY,DirectionZ);
                Vector3.Normalize(rayDirection);

                arrayOfRays[i] = rayDirection;
            }

            return arrayOfRays;
        }

        public int SelectEvent(float diffuse, float reflect, float surfaceEvent)
        {
            if (diffuse > surfaceEvent && reflect > surfaceEvent)
                return 0;        //ray absorbed
            else
            {
                if (diffuse < reflect)
                    return 1;    //random reflection
                else
                {
                    return 2;    //mirror reflection
                }
            }
        }
        public void BasicLayerRaycasting(SceneEntity world)
        {
            var generatedRays= new Vector3[world.NumOfRays];
            float DirectionX, DirectionY, DirectionZ;    //view direction
            var rayProcessingService = new RayProcessingService();
            for (int heightIndex = 0; heightIndex < world.WinHeight; heightIndex++)
            {
                for (int widthIndex = 0; widthIndex < world.WinWidth; widthIndex++)
                {
                    generatedRays = GenerateRaysBunch(heightIndex, widthIndex, world);    //create bunch of rays for pixel
                    for (int i = 0; i < world.NumOfRays; i++)
                    {
                        var ray = generatedRays[i];
                        var rayOrigin = world.Cam;
                        var tempPixel = new IntersectionEntity(rayOrigin);
                        if(IntersectionCheck(world,tempPixel,ray,rayOrigin)==false)    //check if ray from bunch intersects any geometry
                            continue;
                        var rndSeed = new Random();
                        var randomEvent = (float)rndSeed.NextDouble();
                        var surfaceEvent = SelectEvent(tempPixel.Material.Albedo.X,tempPixel.Material.Albedo.Z,randomEvent);
                        if(surfaceEvent == 0)
                            world.PixelsHandler[heightIndex,widthIndex].Color = world.Materials[0].DiffuseColor;
                        else
                        {
                            world.PixelsHandler[heightIndex, widthIndex].Color += tempPixel.Color;
                            for (int ttl = 0; ttl < world.NumOfBounces; ttl++)
                            {
                                if (surfaceEvent == 1)
                                    ray = rayProcessingService.ReflectRayRandomly(ray, tempPixel.FaceNormal);
                                else
                                    ray = rayProcessingService.ReflectRay(ray, tempPixel.FaceNormal);
                                
                                if (IntersectionCheck(world, tempPixel, ray,rayOrigin) == false)
                                    break;
                                else
                                {
                                    randomEvent = (float)rndSeed.NextDouble();
                                    surfaceEvent = SelectEvent(tempPixel.Material.Albedo.X,tempPixel.Material.Albedo.Z, randomEvent);
                                    if (surfaceEvent == 0) 
                                        break;
                                    else
                                        world.PixelsHandler[heightIndex, widthIndex].Color += tempPixel.Color;
                                }
                            }
                        }
                    }
                    
                }
            }
            
        }

       
        // public void Run()
        // {
        //     var stepSize = width / (float)countTask;
        //     for (var taskIterator = 0; taskIterator < countTask; taskIterator++)
        //     {
        //         var iterator = taskIterator;
        //         Console.WriteLine($"{taskIterator} thread starts");
        //         new Thread(() => RaysTrace(iterator, stepSize, cancellationTokenSource.Token)).Start();
        //     }
        // }

        // private void RaysTrace(int taskNumber, float stepSize, CancellationToken cancellationToken)
        // {
        //     try
        //     {
        //         var minX = (int) stepSize * taskNumber;
        //         var maxX = (int) stepSize * (taskNumber + 1);
        //         var minY = 0;
        //         var maxY = height;
        //
        //         var cellIterator = 0;
        //         
        //         while (!cancellationToken.IsCancellationRequested)
        //         {
        //             var x = cellIterator % (maxX - minX) + minX;
        //             var y = cellIterator / (maxX - minX);
        //
        //             if (++cellIterator >= (maxX - minX) * (maxY - minY))
        //             {
        //                 cellIterator = 0;
        //             }
        //             
        //             var offsetX = (float) Random.NextDouble();
        //             var offsetY = (float) Random.NextDouble();
        //             var direction = new Vector3(
        //                     x + offsetX - width * 0.5f,
        //                     -(y + offsetY) + height * 0.5f,
        //                     width / (float) Math.Tan(fieldOfView * 0.5f))
        //                 .Normalize();
        //
        //             var color = CastRay(new RayEntity()
        //             {
        //                 Origin = new Vector3(0.0f, 0.0f, 0.0f),
        //                 Direction = direction
        //             });
        //
        //             lock (lockObject)
        //             {
        //                 var pixel = pixels[x, y];
        //                 pixel.AccumulationColors += color.ToVector3();
        //                 pixel.Color = new ColorEntity(pixel.AccumulationColors / ++pixel.CountAccumulations);
        //             }
        //         }
        //         Console.WriteLine($"{taskNumber} thread stops");
        //     }
        //     catch (Exception exception)
        //     {
        //         Console.WriteLine($"{exception.Message} {exception.StackTrace}");
        //     }
        // }
        //
        // public void AddLight(ILight light)
        // {
        //     lights.Add(light);
        // }
        //
        // public void AddEssence(IEssence essence)
        // {
        //     essences.Add(essence);
        // }
    }
}