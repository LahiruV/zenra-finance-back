﻿namespace zenra_finance_back.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Nic { get; set; }
        public string Password { get; set; }
        public string Profile { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
