using System.Numerics;
using TheRender.Entities;
using TheRender.Services;

namespace TheRender.OpenTK.Scenes
{
    public static class DefaultScene
    {
        public static RayTracingService AddDefaultScene(this RayTracingService rayTracingService)
        {
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new Vector3(0.0f, 50.0f, 50.0f),
                Intensity = 0.8f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new Vector3(-20.0f, 50.0f, 100.0f),
                Intensity = 0.8f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddEssence(new QuadEntity()
            {
                Material = new MaterialEntity()
                {
                    Color = ColorEntity.White,
                    DiffuseComponent = 0.7f,
                    ReflectComponent = 0.3f,
                },
                Position = new Vector3(0.0f, -20.0f, 100.0f),
                Normal = new Vector3(0.0f, 1.0f, 0.0f),
                VertexA = new Vector3(-1000.0f, 0.0f,  1000.0f),
                VertexB = new Vector3(1000.0f, 0.0f,  1000.0f),
                VertexC = new Vector3(1000.0f, 0.0f,  -1000.0f),
                VertexD = new Vector3(-1000.0f, 0.0f,  -1000.0f)
            });

            return rayTracingService;
        }
    }
}