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

namespace SOFExtension.ToolWindows
{
	/// <summary>
	/// Interaction logic for SOFMainWindowControl.
	/// </summary>
	public partial class SOFMainWindowControl : UserControl
	{
		public List<SOFSearchModel.Item> SearchItems { get; set; }

		private HttpClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SOFMainWindowControl"/> class.
		/// </summary>
		public SOFMainWindowControl()
		{
			this.InitializeComponent();
			InitializeItemSource();
		}

		private void InitializeItemSource()
		{
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

		private async Task LoadFromStackoverflow()
		{
			var stringResult = string.Empty;
			var handler = new HttpClientHandler() {
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};
			using( _client = new HttpClient( handler ) ) {
				_client.BaseAddress = new Uri( "https://api.stackexchange.com/2.2/" );
				var uri = $"search?order=desc&sort=activity&site=stackoverflow&intitle={txtSearch.Text}";
				var responseMessage = await _client.GetAsync( uri );
				using( var stream = await responseMessage.Content.ReadAsStreamAsync() ) {
					using( var streamReader = new StreamReader( stream ) ) {
						stringResult = await streamReader.ReadToEndAsync();
					}
				}
			}
			var result = JsonConvert.DeserializeObject<SOFSearchModel>( stringResult );
			DecodeHtmlEntities( result.Items );
		}

		private void DecodeHtmlEntities( List<SOFSearchModel.Item> items )
		{
			using( var writer = new StringWriter() ) {
				foreach( var item in items ) {
					item.Title = WebUtility.HtmlDecode( item.Title );
				}
			}
			icSearchItems.ItemsSource = items;
			var cache = new CacheModel() {
				Query = txtSearch.Text,
				Items = items,
				AddedOn = DateTime.Now
			};
			NaiveCache.Create( cache );
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
			var result = NaiveCache.Get( txtSearch.Text );
			if( result == null ) {
				return false;
			}
			icSearchItems.ItemsSource = result.Items;
			return true;
		}

		private void Hyperlink_RequestNavigate( object sender, System.Windows.Navigation.RequestNavigateEventArgs e )
		{
			var ps = new ProcessStartInfo( e.Uri.AbsoluteUri ) {
				UseShellExecute = true,
				Verb = "open"
			};
			Process.Start( ps );
		}
	}
}