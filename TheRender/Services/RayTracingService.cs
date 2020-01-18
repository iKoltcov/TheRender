using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
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
        private volatile PixelEntity[,] pixels;

        private readonly float fieldOfView = 1.57f;
        private readonly int maxDepthReflect = 7;
        private readonly float epsilon = 1e-3f;

        private readonly ColorEntity backgroundColor = new ColorEntity(0.0f, 0.4f, 0.6f);
        private readonly ColorEntity defaultColor = new ColorEntity(0.3f, 0.3f, 0.3f);

        private readonly List<IEssence> essences;
        private readonly List<ILight> lights;

        public RayTracingService(int width, int height, int countTask)
        {
            this.width = width;
            this.height = height;
            this.countTask = countTask;

            pixels = new PixelEntity[width, height];
            cancellationTokenSource = new CancellationTokenSource();

            for (var widthIterator = 0; widthIterator < width; widthIterator++)
            {
                for (var heightIterator = 0; heightIterator < height; heightIterator++)
                {
                    pixels[widthIterator, heightIterator] = new PixelEntity()
                    {
                        Color = defaultColor,
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
            var stepSize = width / (float) countTask;
            for (var taskIterator = 0; taskIterator < countTask; taskIterator++)
            {
                var iterator = taskIterator;
                Console.WriteLine($"{taskIterator} thread starts");
                new Thread(() => RaysTrace(iterator, stepSize, cancellationTokenSource.Token)).Start();
            }
        }

        private void RaysTrace(int taskNumber, float stepSize, CancellationToken cancellationToken)
        {
            try
            {
                var minX = (int) stepSize * taskNumber;
                var maxX = (int) stepSize * (taskNumber + 1);
                var minY = 0;
                var maxY = height;

                var cellIterator = 0;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var x = cellIterator % (maxX - minX) + minX;
                    var y = cellIterator / (maxX - minX);

                    if (++cellIterator >= (maxX - minX) * (maxY - minY))
                    {
                        cellIterator = 0;
                    }

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
                        Direction = direction,
                    }) ?? backgroundColor;

                    lock (lockObject)
                    {
                        var pixel = pixels[x, y];
                        pixel.AccumulationColors += color.ToVector3();
                        pixel.Color = new ColorEntity(pixel.AccumulationColors / ++pixel.CountAccumulations);
                    }
                }

                Console.WriteLine($"{taskNumber} thread stops");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message} {exception.StackTrace}");
            }
        }

        private ColorEntity CastRay(RayEntity rayEntity, int depth = 0)
        {
            if (depth > maxDepthReflect)
            {
                return ColorEntity.Black;
            }

            var intersect = SceneIntersect(rayEntity);

            if (intersect?.Collision == null)
            {
                return backgroundColor;
            }

            var diffuseLightIntensity = 0.0f;
            var specularLightIntensity = 0.0f;

            foreach (var light in lights)
            {
                var vectorToLight = light.Position - intersect.Collision.Point;
                var directionToLight = vectorToLight.Normalize();
                var distanceToLight = vectorToLight.Length();

                var rayToLight = new RayEntity()
                {
                    Origin = Vector3.Dot(directionToLight, intersect.Collision.Normal) < 0
                        ? intersect.Collision.Point - directionToLight * epsilon
                        : intersect.Collision.Point + directionToLight * epsilon,
                    Direction = directionToLight
                };

                var castRayToLight = SceneIntersect(rayToLight, distanceToLight);

                if (castRayToLight?.Collision == null)
                {
                    diffuseLightIntensity += (light.Intensity / distanceToLight * distanceToLight) *
                                             Math.Max(0.0f, Vector3.Dot(directionToLight, intersect.Collision.Normal));
                    specularLightIntensity +=
                        (float) Math.Pow(
                            Math.Max(0.0f,
                                -Vector3.Dot((-directionToLight).Reflect(intersect.Collision.Normal),
                                    rayEntity.Direction)), intersect.Essence.Material.SpecularIntensity) *
                        light.Intensity;
                }
            }

            ColorEntity indirectIllumination;
            double eventRandom = Random.NextDouble();

            if (eventRandom <= intersect.Essence.Material.SpecularReflectComponent)
            {
                var reflectDirection = rayEntity.Direction.Reflect(intersect.Collision.Normal).Normalize();
                var reflectionRay = new RayEntity()
                {
                    Origin = Vector3.Dot(reflectDirection, intersect.Collision.Normal) < 0
                        ? intersect.Collision.Point - intersect.Collision.Normal * epsilon
                        : intersect.Collision.Point + intersect.Collision.Normal * epsilon,
                    Direction = reflectDirection,
                };
                indirectIllumination = CastRay(reflectionRay, depth + 1);
            }
            else
            if (eventRandom <= intersect.Essence.Material.SpecularReflectComponent + intersect.Essence.Material.DiffuseReflectComponent)
            {
                var diffuseReflectionDirection = MathHelper.DiffuseReflect(intersect.Collision.Normal);
                var reflectionRay = new RayEntity()
                {
                    Origin = Vector3.Dot(diffuseReflectionDirection, intersect.Collision.Normal) < 0 
                        ? intersect.Collision.Point - intersect.Collision.Normal * epsilon
                        : intersect.Collision.Point + intersect.Collision.Normal * epsilon,
                    Direction = diffuseReflectionDirection,
                };
                indirectIllumination = CastRay(reflectionRay, depth + 1) * 0.5f;
            }
            else
            {
                indirectIllumination = ColorEntity.Black;
            }

            
            var result = intersect.Essence.Material.Color * diffuseLightIntensity * intersect.Essence.Material.Diffuse
                         + ColorEntity.White * specularLightIntensity * intersect.Essence.Material.Specular;
            if (indirectIllumination != null)
            {
                result += indirectIllumination;
            }

            return result;
        }

        private SceneIntersectResult SceneIntersect(RayEntity rayEntity, float? distanceMax = null)
        {
            CollisionResult collisionRef = null;
            float? distanceMin = null;
            IEssence essenceRef = null;

            foreach (var essence in essences)
            {
                var collision = essence.CheckCollision(rayEntity);

                if (collision == null)
                {
                    continue;
                }

                var distance = (rayEntity.Origin - collision.Point).Length();
                if (distanceMax != null && distanceMax < distance)
                {
                    continue;
                }

                if (distanceMin == null ^ distanceMin > distance)
                {
                    distanceMin = distance;
                    essenceRef = essence;
                    collisionRef = collision;
                }
            }

            if (essenceRef == null)
            {
                return null;
            }

            return new SceneIntersectResult()
            {
                Essence = essenceRef,
                Collision = collisionRef
            };
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}