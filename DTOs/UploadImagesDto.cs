using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;


namespace Shapper.DTOs
{
    public class UploadImagesDto
    {
        [SwaggerSchema("Selecciona la imagen a subir", Format = "binary")]
        public IFormFile File { get; set; }
    }
}
