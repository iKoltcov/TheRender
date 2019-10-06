using System.Numerics;

namespace TheRender.Entities.Interfaces
{
    public interface IEssence : ICoordinatable
    {        
        MaterialEntity Material { get; set; }
        
        Vector3? CheckCollision(RayEntity ray);
        
        Vector3 GetNormal();

        Vector3 GetNormal(Vector3 intersect);
    }
}
