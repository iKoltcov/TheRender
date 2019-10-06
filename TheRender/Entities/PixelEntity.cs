using System.Numerics;

namespace TheRender.Entities
{
    public class PixelEntity
    {
        public ColorEntity Color { get; set; }
        
        public Vector3 AccumulationColors { get; set; }
        
        public int CountAccumulations { get; set; }
    }
}