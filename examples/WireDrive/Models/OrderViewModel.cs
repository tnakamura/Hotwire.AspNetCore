using System.ComponentModel.DataAnnotations;

namespace WireDrive.Models;

public class OrderViewModel
{
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "お名前を入力してください")]
    [Display(Name = "お名前")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "メールアドレスを入力してください")]
    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
    [Display(Name = "メールアドレス")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "住所を入力してください")]
    [Display(Name = "住所")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "コメント")]
    public string? Comments { get; set; }
}

public class OrderConfirmationViewModel
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
