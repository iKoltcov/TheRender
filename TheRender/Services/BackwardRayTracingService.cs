using System;
using System.Collections.Generic;
using TheRender.Entities;
using System.Numerics;
namespace TheRender.Services
{
    public class ForwardRayTracingService
    {
        public Vector3 GenerateRayDirection(SceneEntity world, uint bunchOfRays)
        {
            for(int k=0;k<bunchOfRays;k++)
            {
                for (int j = 0; j < world.WinHeight; j++)
                {
                    for (int i = 0; i < world.WinWidth; i++)
                    {
                        dir_x = (i + 0.5) - world.WinWidth / 2f;
                        dir_y = -(j + 0.5) + world.WinHeight / 2f;
                        dir_z = -world.WinHeight / (2f * Math.Tan(fov / 2f));

                        direction.X = (float) dir_x;
                        direction.Y = (float) dir_y;
                        direction.Z = (float) dir_z;

                        var arr_idx = i + j * world.WinWidth;
                        norm_dir = Vector3.Normalize(direction);

                        res_color = TracerService.cast_ray(origin, ref norm_dir, world.Spheres, world.Lights); //COLOR
                        color_buf.Add(res_color);
                    }
                }
            }
        }
        
    }
}