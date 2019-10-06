using System.Numerics;
using TheRender.Entities.Interfaces;

namespace TheRender.Entities
{
    public class PointLightEntity : ILight
    {
        public Vector3 Position { get; set; }

        public ColorEntity Color { get; set; }

        public float Intensity { get; set; }
    }
}
