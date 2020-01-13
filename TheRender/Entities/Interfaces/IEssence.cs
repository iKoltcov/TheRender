using TheRender.Results;

namespace TheRender.Entities.Interfaces
{
    public interface IEssence : ICoordinatable
    {        
        MaterialEntity Material { get; set; }
        
        CollisionResult CheckCollision(RayEntity ray);
    }
}
