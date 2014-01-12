using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using LinqToTwitter;
namespace TwitterRestAPITest
{
    public partial class LinqToTwitterTest : System.Web.UI.Page
    {
        private WebAuthorizer auth;
        private TwitterContext twitterCtx;
        protected void Page_Load(object sender, EventArgs e)
        {           
            IOAuthCredentials credentials = new SessionStateCredentials();

            if (credentials.ConsumerKey == null || credentials.ConsumerSecret == null)
            {
                credentials.ConsumerKey = "your consumer key";
                credentials.ConsumerSecret = "your consumer secret";
            }
            if (Request.QueryString["oauth_token"] != null)
            {
                    string requestToken = Request["oauth_token"].ToString();
                    string pin = Request["oauth_verifier"].ToString();
                
            }
            
            auth = new WebAuthorizer
            {                 
                Credentials = credentials,
                PerformRedirect = authUrl => Response.Redirect(authUrl),
                

            };
            if (!Page.IsPostBack)
            {
                auth.CompleteAuthorization(Request.Url);
            }

            if (Request.QueryString["oauth_token"] == null)
            {
                auth.BeginAuthorization(Request.Url);
            }
            var twitterCtx = new TwitterContext(auth);
            string status = "Testing TweetWithMedia #Linq2Twitter " +
            DateTime.Now.ToString(CultureInfo.InvariantCulture);
            const bool PossiblySensitive = false;
            const decimal Latitude = StatusExtensions.NoCoordinate; 
            const decimal Longitude = StatusExtensions.NoCoordinate; 
            const bool DisplayCoordinates = false;

            string ReplaceThisWithYourImageLocation = Server.MapPath("~/test.jpg");

            var mediaItems =
                new List<Media>
                {
                    new Media
                    {
                        Data = Utilities.GetFileBytes(ReplaceThisWithYourImageLocation),
                        FileName = "test.jpg",
                        ContentType = MediaContentType.Jpeg
                    }
                };

            Status tweet = twitterCtx.TweetWithMedia(
                status, PossiblySensitive, Latitude, Longitude,
                null, DisplayCoordinates, mediaItems, null);

            Response.Write("Your tweet has been published successfully: " + tweet.Text);
        }
       
    }
}