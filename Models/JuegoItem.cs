using System.ComponentModel.DataAnnotations;

namespace JuegoApi.Models;

public class JuegoItem
{
    [Key]
    public String? Name { get; set; }
    public float Time { get; set; }
}