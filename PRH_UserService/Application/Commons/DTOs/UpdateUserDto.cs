using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Commons.DTOs;

public class UpdateUserDto
{
    public string FullName { get; set; } = null!;
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    public string PhoneNumber { get; set; } = null!;
    public string Descrtiption { get; set; } = null!;
}
