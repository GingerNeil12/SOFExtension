using System;
using System.Collections.Generic;
using SOFExtension.Models;

namespace SOFExtension
{
	public static class NaiveCache
	{
		private static IDictionary<string, CacheModel> _cache;

		public static void Create( CacheModel model )
		{
			if( _cache == null ) {
				_cache = new Dictionary<string, CacheModel>();
			}

			if( !_cache.ContainsKey( model.Query ) ) {
				_cache.Add( model.Query, model );
			}
		}

		public static CacheModel Get( string query )
		{
			if( _cache == null ) {
				return null;
			}

			var model = new CacheModel();
			if( _cache.TryGetValue( query, out model ) ) {
				var timeDifference = model.AddedOn.Hour - DateTime.Now.Hour;
				if( timeDifference >= 1 ) {
					Remove( query );
					return null;
				}
				return model;
			}
			return null;
		}

		private static void Remove( string query )
		{
			if( _cache != null && _cache.ContainsKey( query ) ) {
				_cache.Remove( query );
			}
		}
	}
}
