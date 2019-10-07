using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using TheRender.Entities;
using TheRender.Entities.Interfaces;
using TheRender.Extensions;
using TheRender.Results;

namespace TheRender.Services
{
    public class RayTracingService : IDisposable
    {
        private static readonly Random Random = new Random();

        private readonly int countTask;
        private readonly object lockObject = new object();
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly int width;
        private readonly int height;
        private readonly PixelEntity[,] pixels;

        private readonly float fieldOfView = 1.57f;
        private readonly int maxDepthReflect = 5;
        private readonly float epsilon = 1e-3f;
        
        private readonly ColorEntity backgroundColor = new ColorEntity(0.0f, 0.6f, 0.9f); 

        private readonly List<IEssence> essences;
        private readonly List<ILight> lights;

        public RayTracingService(int width, int height, int countTask)
        {
            this.width = width;
            this.height = height;
            this.countTask = countTask;

            pixels = new PixelEntity[width, height];
            cancellationTokenSource = new CancellationTokenSource();

            for (var widthIterator = 0; widthIterator < width; widthIterator++) {
                for (var heightIterator = 0; heightIterator < height; heightIterator++)
                {
                    pixels[widthIterator, heightIterator] = new PixelEntity()
                    {
                        Color = backgroundColor,
                        AccumulationColors = new Vector3(),
                        CountAccumulations = 0
                    };
                }
            }

            essences = new List<IEssence>();
            lights = new List<ILight>();
        }

        public void AddLight(ILight light) 
        {
            lights.Add(light);
        }

        public void AddEssence(IEssence essence)
        {
            essences.Add(essence);
        }

        public PixelEntity[,] GetPixels()
        {
            if (pixels == null)
            {
                throw new NullReferenceException("Pixel array not initialized");
            }
            
            return pixels;
        }

        public void Run()
        {            
            var stepSize = width / (float)countTask;
            for (var taskIterator = 0; taskIterator < countTask; taskIterator++)
            {
                var iterator = taskIterator;
                new Thread(() => RaysTrace(iterator, stepSize, cancellationTokenSource.Token)).Start();
            }
        }

        private void RaysTrace(int taskNumber, float stepSize, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var x = Random.Next((int) stepSize * taskNumber, (int) stepSize * (taskNumber + 1));
                    var y = Random.Next(0, height);
                    var offsetX = (float) Random.NextDouble();
                    var offsetY = (float) Random.NextDouble();
                    var direction = new Vector3(
                            x + offsetX - width * 0.5f,
                            -(y + offsetY) + height * 0.5f,
                            width / (float) Math.Tan(fieldOfView * 0.5f))
                        .Normalize();

                    var color = CastRay(new RayEntity()
                    {
                        Origin = new Vector3(0.0f, 0.0f, 0.0f),
                        Direction = direction
                    });

                    lock (lockObject)
                    {
                        var pixel = pixels[x, y];
                        pixel.AccumulationColors += color.ToVector3();
                        pixel.Color = new ColorEntity(pixel.AccumulationColors / ++pixel.CountAccumulations);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + exception.StackTrace);
            }
        }

        private ColorEntity CastRay(RayEntity rayEntity, int depth = 0)
        {
            if (depth > maxDepthReflect)
            {
                return backgroundColor;
            }
            
            var intersect = SceneIntersect(rayEntity);

            if(intersect == null)
            {
                return backgroundColor;
            }

            var diffuseLightIntensity = 0.0f;
            var specularLightIntensity = 0.0f;
            var normal = intersect.Essence.GetNormal(intersect.Point);
            
            foreach(var light in lights)
            {
                var vectorToLight = light.Position - intersect.Point;
                var directionToLight = vectorToLight.Normalize();
                var distanceToLight = vectorToLight.Length();

                var rayToLight = new RayEntity()
                {
                    Origin = Vector3.Dot(directionToLight, normal) < 0 
                        ? intersect.Point - directionToLight * epsilon
                        : intersect.Point + directionToLight * epsilon,
                    Direction = directionToLight
                };
                
                if(SceneIntersect(rayToLight, distanceToLight) == null)
                {
                    diffuseLightIntensity += (light.Intensity / distanceToLight * distanceToLight) * Math.Max(0.0f, Vector3.Dot(directionToLight, normal));
                    specularLightIntensity += (float)Math.Pow(Math.Max(0.0f, -Vector3.Dot((-directionToLight).Reflect(normal), rayEntity.Direction)), intersect.Essence.Material.Specular) * light.Intensity;
                }
            }

            var reflectColor = new ColorEntity(0.0f, 0.0f, 0.0f);
            if (intersect.Essence.Material.ReflectComponent > 0.0f)
            {
                var reflectDirection = rayEntity.Direction.Reflect(normal).Normalize();
                var reflectionRay = new RayEntity()
                {
                    Origin = Vector3.Dot(reflectDirection, normal) < 0 
                        ? intersect.Point - normal * epsilon
                        : intersect.Point + normal * epsilon,
                    Direction = reflectDirection
                };
                reflectColor = CastRay(reflectionRay, depth + 1);
            }

            return intersect.Essence.Material.Color * diffuseLightIntensity * intersect.Essence.Material.DiffuseComponent 
                + new ColorEntity(1.0f, 1.0f, 1.0f) * specularLightIntensity * intersect.Essence.Material.SpecularComponent
                + reflectColor * intersect.Essence.Material.ReflectComponent;
        }

        private SceneIntersectResult SceneIntersect(RayEntity rayEntity, float? distanceMax = null)
        {
            Vector3? point = null;
            float? distanceMin = null;
            IEssence essenceRef = null;

            foreach (var essence in essences)
            {
                var collisionPoint = essence.CheckCollision(rayEntity);

                if (!collisionPoint.HasValue)
                {
                    continue;
                }
                
                var distance = (rayEntity.Origin - collisionPoint.Value).Length();
                if (distanceMax != null && distanceMax < distance)
                {
                    continue;
                }

                if (distanceMin == null ^ distanceMin > distance)
                {
                    distanceMin = distance;
                    essenceRef = essence;
                    point = collisionPoint;
                }
            }

            if (essenceRef == null)
            {
                return null;
            }

            return new SceneIntersectResult()
            {
                Essence = essenceRef,
                Point = point.Value
            };
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
