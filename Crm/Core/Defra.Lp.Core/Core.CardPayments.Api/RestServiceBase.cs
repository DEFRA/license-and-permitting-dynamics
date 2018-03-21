
namespace CardPayments
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public abstract class RestServiceBase
    {

        private readonly RestServiceConfiguration _configuration;

        private HttpClient _httpclient;

        protected HttpClient Httpclient
        {
            get
            {
                return _httpclient;
            }
            set
            {
                this._httpclient = value;
            }
        }

        protected RestServiceBase(RestServiceConfiguration configuration)
        {
            this._configuration = configuration;
            this.Init();
        }

        protected void Init()
        {
            // TLS 1.2 required for Gov Pay
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this._httpclient = new HttpClient();
            this._httpclient.DefaultRequestHeaders.Host = _configuration.TargetHost;
            this._httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_configuration.SecurityHeader, _configuration.ApiKey);
        }


        /// <summary>
        /// Post a message to the target API
        /// </summary>
        /// <param name="urlPath"></param>
        /// <param name="byteContent"></param>
        /// <returns></returns>
        protected HttpResponseMessage Post(string urlPath, ByteArrayContent byteContent)
        {
            return this._httpclient.PostAsync($"{_configuration.TargetUrl}{urlPath}", byteContent).Result;
        }

        protected HttpResponseMessage Get(string urlPath)
        {
            return this._httpclient.GetAsync($"{_configuration.TargetUrl}{urlPath}").Result;
        }
    }
}