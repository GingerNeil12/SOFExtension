using System;
using System.Collections.Generic;

namespace SOFExtension.Models
{
	public class CacheModel
	{
		public string Query { get; set; }
		public List<SOFSearchModel.Item> Items { get; set; }
		public DateTime AddedOn { get; set; }
	}
}
