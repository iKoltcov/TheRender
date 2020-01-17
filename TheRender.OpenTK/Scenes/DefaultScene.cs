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
                Position = new Vector3(0.0f, 25.0f, 0.0f),
                Intensity = 0.2f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            rayTracingService.AddLight(new PointLightEntity()
            {
                Position = new Vector3(0.0f, 25.0f, 90.0f),
                Intensity = 0.8f,
                Color = new ColorEntity(1.0f, 1.0f, 1.0f)
            });
            
            //rayTracingService.AddEssence(new SphereEntity(new Vector3(0.0f, 0.0f, 50.0f), 10.0f));
            rayTracingService.AddEssence(new CubeEntity(
                new Vector3(-15.0f, -5.0f, 80.0f), 
                new Vector3(5.0f, 5.0f, 5.0f), 
                MaterialEntity.Default(ColorEntity.Blue)));
            rayTracingService.AddEssence(new CubeEntity(
                new Vector3(0.0f, -5.0f, 90.0f), 
                new Vector3(5.0f, 5.0f, 5.0f), 
                MaterialEntity.Default(ColorEntity.Red)));
            rayTracingService.AddEssence(new CubeEntity(
                new Vector3(15.0f, -5.0f, 80.0f), 
                new Vector3(5.0f, 5.0f, 5.0f), 
                MaterialEntity.Default(ColorEntity.Yellow)));
            
            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, -10.0f, 0.0f),
                new[]
                {
                    new Vector3(-100.0f, -10.0f, 50.0f),
                    new Vector3(-100.0f, 50.0f, 50.0f),
                    new Vector3(0.0f, 50.0f, 100.0f),
                    new Vector3(0.0f, -10.0f, 100.0f),
                },
                MaterialEntity.Default(ColorEntity.White)));
            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, -10.0f, 0.0f),
                new[]
                {
                    new Vector3(100.0f, -10.0f, 50.0f),
                    new Vector3(0.0f, -10.0f, 100.0f),
                    new Vector3(0.0f, 50.0f, 100.0f),
                    new Vector3(100.0f, 50.0f, 50.0f),
                },
                MaterialEntity.Default(ColorEntity.Green)));
            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, -10.0f, 0.0f),
                new[]
                {
                    new Vector3(-100.0f, -10.0f, -50.0f),
                    new Vector3(0.0f, -10.0f, -100.0f),
                    new Vector3(0.0f, 50.0f, -100.0f),
                    new Vector3(-100.0f, 50.0f, -50.0f),
                },
                MaterialEntity.Default(ColorEntity.Green)));
            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, -10.0f, 0.0f),
                new[]
                {
                    new Vector3(0.0f, -10.0f, -100.0f),
                    new Vector3(100.0f, -10.0f, -50.0f),
                    new Vector3(100.0f, 50.0f, -50.0f),
                    new Vector3(0.0f, 50.0f, -100.0f),
                },
                MaterialEntity.Default(ColorEntity.Green)));
            
            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, -10.0f, 0.0f),
                new[]
                {
                    new Vector3(-100.0f, 0.0f, 100.0f),
                    new Vector3(100.0f, 0.0f, 100.0f),
                    new Vector3(100.0f, 0.0f, -100.0f),
                    new Vector3(-100.0f, 0.0f, -100.0f),
                },
                new MaterialEntity()
                {
                    Color = ColorEntity.White,
                    Diffuse = 0.9f,
                    Specular = 0.1f,
                    SpecularIntensity = 50.0f,
                    SpecularReflectComponent = 0.4f,
                    DiffuseReflectComponent = 0.1f,
                }));
            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, 30.0f, 0.0f),
                new[]
                {
                    new Vector3(100.0f, 0.0f, -100.0f),
                    new Vector3(100.0f, 0.0f, 100.0f),
                    new Vector3(-100.0f, 0.0f, 100.0f),
                    new Vector3(-100.0f, 0.0f, -100.0f),
                },
                MaterialEntity.Default(ColorEntity.White)));

            return rayTracingService;
        }
    }
}