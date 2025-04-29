using System;

namespace VoteBirthy.DTOs
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
} 