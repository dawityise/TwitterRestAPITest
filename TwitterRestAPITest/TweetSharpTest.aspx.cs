using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using TweetSharp;

namespace TwitterRestAPITest
{
    public partial class TweetSharpTest : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            var oauth_consumer_key = "your consumer key";
            var oauth_consumer_secret = "your consumer secret";
       
            string dbToken = string.Empty;
            string dbSecret = string.Empty;

            //the assumption here is your application has members
            //check whether an authenticated member has twitter access token and secret within our local database; 
            //for now let's say there is no in db ==> hasTokensInDb = false
            bool hasTokensInDb = false; //GetMembersTwitterTokenFromDb(ref string dbToken, ref string dbSecret);
            var service = new TwitterService(oauth_consumer_key, oauth_consumer_secret);

            if (hasTokensInDb)
            {
                service.AuthenticateWith(dbToken, dbSecret);
                DoWhatThisAppNeedToDo(ref service);
            }
            else
            {
                //if oauth_token is null (i.e. if we haven't sent a token request to twitter yet, 
                //then send a get token request by addressing the this page as a callback (Request.Url.AbsoluteUri)

                if (Request["oauth_token"] == null)
                {
                    OAuthRequestToken requestToken = service.GetRequestToken(Request.Url.AbsoluteUri);
                    Response.Redirect(string.Format("http://twitter.com/oauth/authorize?oauth_token={0}", requestToken.Token));

                }
                else
                {
                    string requestToken = Request["oauth_token"].ToString();
                    string pin = Request["oauth_verifier"].ToString();
                    // Using the values Twitter sent back, get an access token from Twitter
                    var accessToken = service.GetAccessToken(new OAuthRequestToken { Token = requestToken }, pin);
                    // Use that access token and send a tweet on the user's behalf
                    if (accessToken != null && !string.IsNullOrEmpty(accessToken.Token) && !string.IsNullOrEmpty(accessToken.TokenSecret))
                    {
                        service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
                        UpdateDatabase(accessToken.Token, accessToken.TokenSecret);
                        DoWhatThisAppNeedToDo(ref service);
                    }
                }
            }
        }

        private void UpdateDatabase(string token, string secret)
        {
            //do database updates
        }
        private void DoWhatThisAppNeedToDo(ref TwitterService tService)
        {
            //if you want status update only uncomment the below line of code instead
            //var result = tService.SendTweet(new SendTweetOptions { Status = Guid.NewGuid().ToString() });
                
            Bitmap img = new Bitmap(Server.MapPath("~/test.jpg"));

            if (img != null)
            {
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                Dictionary<string, Stream> images = new Dictionary<string, Stream> { { "mypicture", ms } };
                //Twitter compares status contents and rejects dublicated status messages. 
                //Therefore in order to create a unique message dynamically, a generic guid has been used
                var result = tService.SendTweetWithMedia(new SendTweetWithMediaOptions { Status = Guid.NewGuid().ToString(), Images = images });
                if (result != null && result.Id > 0)
                {
                    Response.Redirect("https://twitter.com");
                }
                else
                {
                    Response.Write("fails to update status");
                }
            }
        }

    }
}