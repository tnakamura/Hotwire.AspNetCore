using System.ComponentModel.DataAnnotations;

namespace WireDrive.Models;

public class OrderViewModel
{
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "Please enter your name")]
    [Display(Name = "Name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your address")]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Comments")]
    public string? Comments { get; set; }
}

public class OrderConfirmationViewModel
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
