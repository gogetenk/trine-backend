﻿using System;

namespace Dto
{
    public class UserActivityDto
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfilePicUrl { get; set; }
        public DateTime? SignatureDate { get; set; }
        public string SignatureUri { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string Email { get; set; }
        public bool CanSign { get; set; }

    }
}