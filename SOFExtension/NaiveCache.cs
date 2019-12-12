using System;
using System.Collections.Generic;
using SOFExtension.Models;

namespace SOFExtension
{
	public static class NaiveCache
	{
		private static IDictionary<string, SearchCacheModel> _searchCache;
		private static IDictionary<long, QuestionCacheModel> _questionCache;

		public static void AddSearchModel( SearchCacheModel model )
		{
			if(_searchCache == null ) {
				_searchCache = new Dictionary<string, SearchCacheModel>();
			}

			model.Query.Trim();
			if( !_searchCache.ContainsKey( model.Query ) ) {
				_searchCache.Add( model.Query, model );
			}
		}

		private static void AddQuestionModel(QuestionCacheModel model)
		{
			if(_questionCache == null) {
				_questionCache = new Dictionary<long, QuestionCacheModel>();
			}

			if (!_questionCache.ContainsKey(model.QuestionId)) {
				_questionCache.Add(model.QuestionId, model);
			}
		}

		public static SearchCacheModel GetSearchModel( string query )
		{
			if(_searchCache == null ) {
				return null;
			}

			var model = new SearchCacheModel();
			if(_searchCache.TryGetValue( query.Trim(), out model ) ) {
				var timeDifference = (DateTime.Now - model.AddedOn).TotalMinutes;
				if( timeDifference >= 60 ) {
					RemoveSearchModel( query );
					return null;
				}
				return model;
			}
			return null;
		}

		public static QuestionCacheModel GetQuestionModel(long id)
		{
			if(_questionCache == null) {
				return null;
			}

			var model = new QuestionCacheModel();
			if(_questionCache.TryGetValue(id, out model)) {
				var timeDifference = (DateTime.Now - model.AddedOn).TotalMinutes;
				if(timeDifference >= 60) {
					RemoveQuestionModel(id);
					return null;
				}
				return model;
			}
			return null;
		}

		private static void RemoveSearchModel( string query )
		{
			if(_searchCache != null && _searchCache.ContainsKey( query ) ) {
				_searchCache.Remove( query );
			}
		}

		private static void RemoveQuestionModel(long id)
		{
			if(_questionCache != null && _questionCache.ContainsKey(id)) {
				_questionCache.Remove( id );
			}
		}
	}
}
