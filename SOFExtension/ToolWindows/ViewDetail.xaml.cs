using Newtonsoft.Json;
using SOFExtension.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SOFExtension.ToolWindows
{
    /// <summary>
    /// Interaction logic for ViewDetail.xaml
    /// </summary>
    public partial class ViewDetail : UserControl
    {
        private readonly long _questionId;
        private readonly string _query;

        public string Title { get; set; }
        public string Link { get; set; }
        public string Body { get; set; }
        public string BodyMarkdown { get; set; }
        public int AnswerCount { get; set; }
        public int ViewCount { get; set; }

        private SOFQuestionModel.Item Question { get; set; }
        public List<SOFAnswerModel> Answers { get; set; }

        public ViewDetail(long questionId, string query)
        {
            InitializeComponent();
            InitializeBindings();

            _questionId = questionId;
            _query = query;

            OnStart();
        }

        private void InitializeBindings()
        {
            Answers = new List<SOFAnswerModel>();
        }

        private void OnStart()
        {
            if (!LoadFromCache()) {
                LoadQuestionFromStackoverflow();
            }
        }

        private bool LoadFromCache()
        {
            var model = NaiveCache.GetQuestionModel(_questionId);
            if(model != null) {
                Question = model.Question;
                Answers = model.Answers;
                return true;
            }
            return false;
        }

        private async Task LoadQuestionFromStackoverflow()
        {
            var stringResponse = string.Empty;
            var handler = new HttpClientHandler() {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            using(var client = new HttpClient(handler)) {
                client.BaseAddress = new Uri("https://api.stackexchange.com/2.2/");
                var uri = $"questions/{_questionId}?order=desc&sort=activity&site=stackoverflow&filter=!9Z(-wwK0y";
                var responseMessage = await client.GetAsync(uri);
                using(var stream = await responseMessage.Content.ReadAsStreamAsync()) {
                    using(var reader = new StreamReader(stream)) {
                        stringResponse = await reader.ReadToEndAsync();
                    }
                }
            }
            var list = JsonConvert.DeserializeObject<SOFQuestionModel>(stringResponse);
            var model = list.Items.FirstOrDefault();
            model = HtmlParser(model);
            Question = model;
            Title = model.Title;
            Body = model.Body;
            BodyMarkdown = model.BodyMarkdown;
            AnswerCount = model.AnswerCount;
            ViewCount = model.ViewCount;
            Link = model.Link;
        }

        private SOFQuestionModel.Item HtmlParser(SOFQuestionModel.Item model)
        {
            using(var writer = new StringWriter())
            {
                model.Title = WebUtility.HtmlDecode(model.Title);
            }
            return model;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new SOFMainWindowControl(_query);
        }
    }
}
