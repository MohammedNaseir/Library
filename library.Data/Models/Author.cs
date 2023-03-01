﻿namespace library.Data.Models
{
    public class Author : BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
