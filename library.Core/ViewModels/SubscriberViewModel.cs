﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.ViewModels
{
    public class SubscriberViewModel
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string? MobileNumber { get; set; }
        public bool? HasWhatsApp { get; set; }
        public string? Email { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
        public string? Area { get; set; }
        public string? Governorate { get; set; }
        public string? Address { get; set; }
        public bool IsBlackListed { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
