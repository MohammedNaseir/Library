using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Data.Models
{
	[Index(nameof(Name), nameof(GovernorateId), IsUnique = true)]
	public class Area : BaseModel
	{
		public int Id { get; set; }

		[MaxLength(100)]
		public string Name { get; set; } = null!;

		public int GovernorateId { get; set; }

		public Governorate? Governorate { get; set; }
	}
}
