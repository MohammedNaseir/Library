using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Data.Models
{
	[Index(nameof(Name), IsUnique = true)]
	public class Governorate : BaseModel
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string Name { get; set; } = null!;

		public ICollection<Area> Areas { get; set; } = new List<Area>();
	}
}
