using System.ComponentModel.DataAnnotations;

namespace FoodItem.Application.DTOs
{
    public record FoodItemDTO
        ( int Id,
        [Required] string Name,
        [Required, Range(1, int.MaxValue)] int Quantity,
        [Required,DataType(DataType.Currency)] decimal Price
        );
}
