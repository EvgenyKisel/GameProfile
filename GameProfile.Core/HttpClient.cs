using GameProfile.Utils.Extensions;
using GameProfile.Utils.Logging;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace GameProfile.Core;

public class HttpClient
{
    private const int DefaultTimeoutMs = 60_000;
    private readonly RestClient _client;
    private readonly ILogger _logger;

    public HttpClient(ILogger logger)
    {
        var options = new RestClientOptions { Timeout = TimeSpan.FromMilliseconds(DefaultTimeoutMs) };
        _client = new RestClient(options, configureSerialization: cfg => cfg.UseNewtonsoftJson());
        _logger = logger;
    }

    public virtual RestResponse<T> SendGetRequest<T>(string url, HttpRequest request) =>
        SendRequest<T>(url, Method.Get, request);

    public virtual RestResponse<T> SendPostRequest<T>(string url, HttpRequest request) =>
        SendRequest<T>(url, Method.Post, request);

    public virtual RestResponse<T> SendPutRequest<T>(string url, HttpRequest request) =>
        SendRequest<T>(url, Method.Put, request);

    public virtual RestResponse<T> SendDeleteRequest<T>(string url, HttpRequest request) =>
        SendRequest<T>(url, Method.Delete, request);

    public virtual RestResponse<T> SendPatchRequest<T>(string url, HttpRequest request) =>
        SendRequest<T>(url, Method.Patch, request);

    public virtual RestResponse<T> SendRequest<T>(string url, Method method, HttpRequest requestModel)
    {
        var restRequest = new RestRequest(url, method);

        foreach (var param in requestModel.GetUrlParameters())
            restRequest.AddQueryParameter(param.Key, param.Value);

        foreach (var header in requestModel.GetHeaders())
            restRequest.AddHeader(header.Key, header.Value);

        var body = requestModel.GetBody();
        if (body != null)
            restRequest.AddStringBody(body, "application/json");

        _logger.Log(DateTime.UtcNow.ToUtcString());
        _logger.Log($"Sending: {method} {_client.BuildUri(restRequest)}");
        _logger.Log(requestModel.ToString());

        var response = _client.Execute<T>(restRequest);

        _logger.Log(DateTime.UtcNow.ToUtcString());
        _logger.Log($"Response: {(int)response.StatusCode} {response.StatusCode} ({response.ResponseStatus})");
        if (!string.IsNullOrEmpty(response.Content))
            _logger.Log($"Body:\n{response.Content}\n");
        if (response.ErrorException != null)
            _logger.Log($"Error: {response.ErrorException}");
        else if (response.ErrorMessage != null)
            _logger.Log($"Error: {response.ErrorMessage}");

        return response;
    }
}
