namespace TheRender.Entities
{
    public class MaterialEntity
    {
        public ColorEntity Color { get; set; }

        public float SpecularIntensity { get; set; }
        
        public float Diffuse { get; set; }
        
        public float Specular { get; set; }

        public float SpecularReflectComponent { get; set; }
        
        public float DiffuseReflectComponent { get; set; }

        public static MaterialEntity Default()
        {
            return new MaterialEntity()
            {
                Color = ColorEntity.Random(),
                Diffuse = 0.9f,
                Specular = 0.1f,
                SpecularIntensity = 50.0f,
                SpecularReflectComponent = 0.0f,
                DiffuseReflectComponent = 0.0f,
            };
        }

        public static MaterialEntity Default(ColorEntity colorEntity)
        {
            var defaultMaterial = Default();
            defaultMaterial.Color = colorEntity;

            return defaultMaterial;
        }

        public static MaterialEntity Mirror => new MaterialEntity()
        {
            Color = ColorEntity.White,
            Diffuse = 0.1f,
            Specular = 0.1f,
            SpecularIntensity = 50.0f,
            SpecularReflectComponent = 1.0f,
            DiffuseReflectComponent = 0.0f,
        };
    }
}
