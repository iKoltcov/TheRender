using System;
using System.Collections.Generic;
using TheRender.Entities;
using System.Numerics;
namespace TheRender.Services
{
    public class BackwardRayTracingService
    {
        public BackwardRayTracingService()
        {
            
        }
        private void IntersectionCheck(SceneEntity world, IntersectionEntity screenCell, Vector3 rayCasted)
        {
            for (int i = 0; i < world.Geometries.Count; i++)
            {
                var geometry = world.Geometries[i];
                for (int j = 0; j < geometry.Wireframe.Count; j++)    //parsin' array of geometry's sectors
                {
                    var sector = geometry.Wireframe[i];
                    var planeHit = new IntersectionService();
                    if (planeHit.PlaneIntersectionTest(sector.TopLeft, sector.TopRight, sector.Bottom, world.Cam, rayCasted, geometry, screenCell))
                    {
                        screenCell.Hits++;
                        break;    //choose next geometry from the list (searchin' for the closest)
                    }
                }
            }
        }
        public void BasicLayerRaycasting(SceneEntity world, uint bunchOfRays)
        {
            Vector3 rayDirection;
            float DirectionX, DirectionY, DirectionZ;    //view direction
            for(int k=0;k<bunchOfRays;k++)
            {
                for (int j = 0; j < world.WinHeight; j++)
                {
                    for (int i = 0; i < world.WinWidth; i++)
                    {
                        DirectionX = (float) ((i + 0.5) - world.WinWidth / 2f);
                        DirectionY = (float) (-(j + 0.5) + world.WinHeight / 2f);
                        DirectionZ = (float) (-world.WinHeight / (2f * Math.Tan(world.FOV / 2f)));
                        rayDirection = new Vector3(DirectionX,DirectionY,DirectionZ);
                        Vector3.Normalize(rayDirection);
                        IntersectionCheck(world,world.PixelsHandler[j,i],rayDirection);
                    }
                }
            }
        }

        
    }
}