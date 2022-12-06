namespace Mango.Services.ProductAPI.Domain.Models.DTO
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public object Result { get; set; }
        public string DisplayMessage { get; set; } = "Success";
        public List<string> ErrorMessage { get; set; }
    }
}
