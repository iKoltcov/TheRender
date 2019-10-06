using System.Numerics;
using TheRender.Entities.Interfaces;

namespace TheRender.Entities
{
    public class CameraEntity : ICoordinatable, ITargetable
    {
        public Vector3 Position { get; set; }

        public Vector3 Target { get; set; }

        public Vector3 UpVector { get; set; }

        public float FieldOfViewRadian { get; set; }
    }
}
