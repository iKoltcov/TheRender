using System.Numerics;
using TheRender.Entities.Interfaces;

namespace TheRender.Results
{
    public class SceneIntersectResult
    {
        public IEssence Essence { get; set; }

        public Vector3 Point { get; set; }
    }
}
