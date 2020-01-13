using TheRender.Entities.Interfaces;

namespace TheRender.Results
{
    public class SceneIntersectResult
    {
        public IEssence Essence { get; set; }

        public CollisionResult Collision { get; set; } 
    }
}
