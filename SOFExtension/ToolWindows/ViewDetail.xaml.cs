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

        public ViewDetail(long questionId, string query)
        {
            InitializeComponent();

            _questionId = questionId;
            _query = query;

            OnStart();
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
                txtTitle.Text = model.Question.Title;
                txtBody.Text = model.Question.BodyMarkdown;
                hlLink.NavigateUri = new Uri( model.Question.Link);
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
            model = HtmlParser( model );
            txtTitle.Text = model.Title;
            txtBody.Text = model.BodyMarkdown;
            hlLink.NavigateUri = new Uri( model.Link );

            QuestionCacheModel cacheModel = new QuestionCacheModel() {
                QuestionId = model.QuestionId,
                AddedOn = DateTime.Now,
                Question = model
            };

            LoadAnswersFromStackoverflow( cacheModel );
        }

        private async Task LoadAnswersFromStackoverflow(QuestionCacheModel cacheModel)
        {
            _client = new Client();
            var result = await _client.GetAnswersResultAsync( _questionId );
            icAnswers.ItemsSource = result.Items;
            cacheModel.Answers = result.Items;
            NaiveCache.AddQuestionModel( cacheModel );
        }

        private SOFQuestionModel.Item HtmlParser(SOFQuestionModel.Item model)
        {
            model.Title = WebUtility.HtmlDecode( model.Title );
            model.BodyMarkdown = WebUtility.HtmlDecode( model.BodyMarkdown );
            return model;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new SOFMainWindowControl(_query);
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
