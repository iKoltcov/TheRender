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
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new Vector3(-20.0f, 0.0f, 120.0f),
                Radius = 20.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new Vector3(0.0f, -5.0f, 100.0f),
                Radius = 15.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new Vector3(10.0f, -10.0f, 80.0f),
                Radius = 10.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new Vector3(15.0f, -15.0f, 70.0f),
                Radius = 5.0f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new Vector3(17.0f, -17.5f, 65.0f),
                Radius = 2.5f
            });
            rayTracingService.AddEssence(new SphereEntity
            {
                Material = MaterialEntity.Default,
                Position = new Vector3(18.0f, -18.75f, 62.5f),
                Radius = 1.25f
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