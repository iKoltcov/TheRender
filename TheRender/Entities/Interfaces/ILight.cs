using TheRender.Results;
using TheRender.Services;

namespace TheRender.Entities.Interfaces
{
    public interface ILight : ICoordinatable
    {
        float Intensity { get; set; }

        LightIntensityResult? GetIlluminance(RayTracingService service, SceneIntersectResult intersect);
    }
}
