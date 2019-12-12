using System;
using System.Collections.Generic;

namespace SOFExtension.Models
{
	public class SearchCacheModel
	{
		public string Query { get; set; }
		public List<SOFSearchModel.Item> Items { get; set; }
		public DateTime AddedOn { get; set; }
	}
}
