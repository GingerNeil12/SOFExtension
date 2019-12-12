using SOFExtension.Models;
using SOFExtension.Services;
using System;
using System.Collections.Generic;
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
        private SOFQuestionModel.Item Question;
        public List<SOFAnswerModel.Answer> Answers;

        public string Title { get; set; }
        public string Link { get; set; }
        public string Body { get; set; }
        public string BodyMarkdown { get; set; }
        public int AnswerCount { get; set; }
        public int ViewCount { get; set; }

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
            _client = new Client();
            Answers = new List<SOFAnswerModel.Answer>();
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
            var result = await _client.GetQuestionResultAsync( _questionId );
           
            var model = result.Items.FirstOrDefault();
            model = HtmlParser( model );
            Title = model.Title;
            Link = model.Link;
            Body = model.Body;
            BodyMarkdown = model.BodyMarkdown;
            AnswerCount = model.AnswerCount;
            ViewCount = model.ViewCount;

            QuestionCacheModel cacheModel = new QuestionCacheModel() {
                QuestionId = model.QuestionId,
                AddedOn = DateTime.Now,
                Question = model
            };

            LoadAnswersFromStackoverflow( cacheModel );
        }

        private async Task LoadAnswersFromStackoverflow(QuestionCacheModel cacheModel)
        {
            var result = await _client.GetAnswersResultAsync( _questionId );
            // Append to the item control
            cacheModel.Answers = result.Items;
            NaiveCache.AddQuestionModel( cacheModel );
        }

        private SOFQuestionModel.Item HtmlParser(SOFQuestionModel.Item model)
        {
            model.Title = WebUtility.HtmlDecode( model.Title );
            return model;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new SOFMainWindowControl(_query);
        }
    }
}
