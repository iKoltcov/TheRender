using System;
using TheRender.Entities;
using TheRender.Entities.Interfaces;

namespace TheRender.Services.Interfaces
{
    public interface ITraceService : IDisposable
    {
        PixelEntity[,] GetPixels();
        
        void Run();
        
        void AddLight(ILight light);
        
        void AddEssence(IEssence essence);
    }
}