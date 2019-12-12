using Newtonsoft.Json;
using SOFExtension.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using SOFExtension.Services;

namespace SOFExtension.ToolWindows
{
	/// <summary>
	/// Interaction logic for SOFMainWindowControl.
	/// </summary>
	public partial class SOFMainWindowControl : UserControl
	{
		public List<SOFSearchModel.Item> SearchItems { get; set; }

		private Client _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SOFMainWindowControl"/> class.
		/// </summary>
		public SOFMainWindowControl()
		{
			this.InitializeComponent();
			InitializeItemSource();
		}

		public SOFMainWindowControl(string query)
		{
			InitializeComponent();
			var model = NaiveCache.GetSearchModel(query);
			txtSearch.Text = query;
			icSearchItems.ItemsSource = model.Items;
		}

		private void InitializeItemSource()
		{
			_client = new Client();
			SearchItems = new List<SOFSearchModel.Item>();
			icSearchItems.ItemsSource = SearchItems;
		}

		/// <summary>
		/// Handles click on the button by displaying a message box.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event args.</param>
		[SuppressMessage( "Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code" )]
		[SuppressMessage( "StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern" )]
		private void button1_Click( object sender, RoutedEventArgs e )
		{
			MessageBox.Show(
				string.Format( System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString() ),
				"SOFMainWindow" );
		}

		private void btnSearch_Click( object sender, RoutedEventArgs e )
		{
			if( !string.IsNullOrWhiteSpace( txtSearch.Text ) ) {
				if( !LoadFromCache() ) {
					LoadFromStackoverflow();
				}
			}
		}

		private bool LoadFromCache()
		{
			var result = NaiveCache.GetSearchModel( txtSearch.Text );
			if( result == null ) {
				return false;
			}
			icSearchItems.ItemsSource = result.Items;
			return true;
		}

		private async Task LoadFromStackoverflow()
		{
			var result = await _client.GetSearchResultsAsync( txtSearch.Text );
			DecodeHtmlEntities(result.Items);
		}

		private void DecodeHtmlEntities(List<SOFSearchModel.Item> items)
		{
			foreach ( var item in items ) {
				item.Title = WebUtility.HtmlDecode( item.Title );
			}
			icSearchItems.ItemsSource = items;
			AddToCache(items);
		}

		private void AddToCache(List<SOFSearchModel.Item> items)
		{
			var cache = new SearchCacheModel()
			{
				Query = txtSearch.Text,
				Items = items,
				AddedOn = DateTime.Now
			};
			NaiveCache.AddSearchModel(cache);
		}

		private void Hyperlink_RequestNavigate( object sender, System.Windows.Navigation.RequestNavigateEventArgs e )
		{
			var ps = new ProcessStartInfo( e.Uri.AbsoluteUri ) {
				UseShellExecute = true,
				Verb = "open"
			};
			Process.Start( ps );
		}

		private void btnView_Click(object sender, RoutedEventArgs e)
		{
			var value = ((Button)sender).Tag;
			var viewDetail = new ViewDetail((long)value, txtSearch.Text);
			this.Content = viewDetail;
		}
	}
}