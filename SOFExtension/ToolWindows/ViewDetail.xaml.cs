using SOFExtension.Models;
using SOFExtension.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SOFExtension.ToolWindows
{
	/// <summary>
	/// Interaction logic for ViewDetail.xaml
	/// </summary>
	public partial class ViewDetail : UserControl
	{
		private readonly long _questionId;
		private readonly string _query;

		private Client _client;

		public ViewDetail( long questionId, string query )
		{
			InitializeComponent();

			_questionId = questionId;
			_query = query;

			OnStart();
		}

		private void OnStart()
		{
			if( !LoadFromCache() ) {
				LoadQuestionFromStackoverflow();
			}
		}

		private bool LoadFromCache()
		{
			var model = NaiveCache.GetQuestionModel( _questionId );
			if( model != null ) {
				txtTitle.Text = HtmlParser( model.Question.Title );
				txtBody.Text = HtmlParser( model.Question.BodyMarkdown );
				btnGotToSite.Tag = model.Question.Link;
				icAnswers.ItemsSource = model.Answers;
				return true;
			}
			return false;
		}

		private async Task LoadQuestionFromStackoverflow()
		{
			_client = new Client();
			var result = await _client.GetQuestionResultAsync( _questionId );

			var model = result.Items.FirstOrDefault();
			txtTitle.Text = HtmlParser( model.Title );
			txtBody.Text = HtmlParser( model.BodyMarkdown );
			btnGotToSite.Tag = model.Link;

			var cacheModel = new QuestionCacheModel() {
				QuestionId = model.QuestionId,
				AddedOn = DateTime.Now,
				Question = model
			};

			LoadAnswersFromStackoverflow( cacheModel );
		}

		private async Task LoadAnswersFromStackoverflow( QuestionCacheModel cacheModel )
		{
			_client = new Client();
			var result = await _client.GetAnswersResultAsync( _questionId );
			foreach( var item in result.Items ) {
				item.BodyMarkdown = HtmlParser( item.BodyMarkdown );
			}
			result.Items = result.Items.OrderByDescending( x => x.IsAccepted ).ToList();
			icAnswers.ItemsSource = result.Items;
			cacheModel.Answers = result.Items;
			NaiveCache.AddQuestionModel( cacheModel );
		}

		private string HtmlParser( string data )
		{
			return WebUtility.HtmlDecode( data );
		}

		private void btnBack_Click( object sender, RoutedEventArgs e )
		{
			this.Content = new SOFMainWindowControl( _query );
		}

		private void btnGotToSite_Click( object sender, RoutedEventArgs e )
		{
			var link = ( (Button)sender ).Tag;
			var ps = new ProcessStartInfo( (string)link ) {
				UseShellExecute = true,
				Verb = "open"
			};
			Process.Start( ps );
		}
	}
}
