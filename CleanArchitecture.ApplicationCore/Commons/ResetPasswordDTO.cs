﻿namespace CleanArchitecture.ApplicationCore.Commons
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
