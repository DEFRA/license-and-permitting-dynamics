namespace Defra.Lp.Core.Workflows.Base
{
    using System;
    using System.Text;
    using System.Net.Http;

    public abstract class APIServiceBase
    {
        protected readonly string TargetURL;

        protected readonly string APIKey;

        private HttpClient httpclient;

        protected HttpClient _httpclient
        {
            get
            {
                this.Init();

                return httpclient;
            }
            set
            {
                this.httpclient = value;
            }
        }

        protected APIServiceBase(string TargetURL, string APIKey)
        {
            this.TargetURL = TargetURL;
            this.APIKey = APIKey;

            this.Init();
        }

        protected void Init()
        {
            this.httpclient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(this.APIKey);
            this.httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
    }
}