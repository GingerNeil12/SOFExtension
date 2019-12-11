namespace SOFExtension.ToolWindows
{
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
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "SOFMainWindow");
        }

        private async Task ClientStuff()
        {
            var stringResult = string.Empty;
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            using(_client = new HttpClient(handler))
            {
                _client.BaseAddress = new Uri("https://api.stackexchange.com/2.2/");
                var responseMessage = await _client.GetAsync("search?order=desc&sort=activity&site=stackoverflow&intitle=wpf page loading");
                using(var stream = await responseMessage.Content.ReadAsStreamAsync())
                {
                    using(var streamReader = new StreamReader(stream))
                    {
                        stringResult = await streamReader.ReadToEndAsync();
                    }
                }
            }
            var result = JsonConvert.DeserializeObject<SOFSearchModel>(stringResult);
            icSearchItems.ItemsSource = result.Items;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ClientStuff();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var ps = new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}