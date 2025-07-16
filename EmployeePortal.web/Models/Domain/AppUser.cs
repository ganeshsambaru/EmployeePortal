using System.ComponentModel.DataAnnotations;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }  // store hashed password

    [Required]
    public string Role { get; set; }
}
