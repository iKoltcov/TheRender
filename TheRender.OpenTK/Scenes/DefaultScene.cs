using System.Numerics;
using TheRender.Entities;
using TheRender.Services;

namespace TheRender.OpenTK.Scenes
{
    public static class DefaultScene
    {
        public static RayTracingService AddDefaultScene(this RayTracingService rayTracingService)
        {
            rayTracingService.AddLight(new SphereLightEntity(new Vector3(40.0f, 25.0f, 0.0f),5.0f,0.9f));
            rayTracingService.AddLight(new SphereLightEntity(new Vector3(0.0f, 25.0f, 30.0f),5.0f,0.5f));

            rayTracingService.AddEssence(new CubeEntity(
                new Vector3(-17.5f, -5.0f, 40.0f), 
                new Vector3(5.0f, 5.0f, 10.0f), 
                MaterialEntity.Default(ColorEntity.Green)));
            rayTracingService.AddEssence(new SphereEntity(
                new Vector3(0.0f, 0.0f, 40.0f), 
                10.0f,
                new MaterialEntity()
                {
                    Color = ColorEntity.White,
                    Diffuse = 0.9f,
                    Specular = 0.1f,
                    SpecularIntensity = 50.0f,
                    SpecularReflectComponent = 0.0f,
                    DiffuseReflectComponent = 0.8f,
                }));
            rayTracingService.AddEssence(new CubeEntity(
                new Vector3(17.5f, -5.0f, 40.0f), 
                new Vector3(5.0f, 5.0f, 10.0f), 
                MaterialEntity.Default(ColorEntity.Red)));

            rayTracingService.AddEssence(new QuadEntity(
                new Vector3(0.0f, -10.0f, 0.0f),
                new[]
                {
                    new Vector3(-60.0f, 0.0f, 60.0f),
                    new Vector3(60.0f, 0.0f, 60.0f),
                    new Vector3(60.0f, 0.0f, -60.0f),
                    new Vector3(-60.0f, 0.0f, -60.0f),
                },
                MaterialEntity.Default(ColorEntity.White)));
            
            return rayTracingService;
        }
    }
}