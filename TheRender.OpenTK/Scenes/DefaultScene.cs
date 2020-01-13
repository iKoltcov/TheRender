using System.Numerics;
using TheRender.Entities;
using TheRender.Entities.Interfaces;
using TheRender.Services;

namespace TheRender.OpenTK.Scenes
{
    public static class DefaultScene
    {
        public static RayTracingService AddDefaultScene(this RayTracingService rayTracingService)
        {
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new Vector3(0.0f, 50.0f, 0.0f),
                Intensity = 0.8f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new Vector3(-20.0f, 50.0f, 100.0f),
                Intensity = 0.8f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddEssence(new SphereEntity(new Vector3(0.0f, -5.0f, 100.0f), 15.0f));
            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, -20.0f, 0.0f),
                new[]
                {
                    new Vector3(-1000.0f, 0.0f, 1000.0f),
                    new Vector3(1000.0f, 0.0f, 1000.0f),
                    new Vector3(1000.0f, 0.0f, -1000.0f),
                    new Vector3(-1000.0f, 0.0f, -1000.0f),
                },
                new MaterialEntity()
                {
                    Color = ColorEntity.White,
                    DiffuseComponent = 0.7f,
                    ReflectComponent = 0.3f,
                }));

            return rayTracingService;
        }
    }
}