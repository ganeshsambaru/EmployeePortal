﻿public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public string? FullName { get; set; }
    public string? ProfileImagePath { get; set; }
}
