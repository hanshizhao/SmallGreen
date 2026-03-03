using Newtonsoft.Json;
using RestSharp;
using SmallGreen.Dto.Base;

namespace SmallGreen.Desktop.Settings.Services
{
    public class HttpRestClient
    {
        private readonly string apiUrl;
        protected readonly RestClient client;

        public HttpRestClient(string _apiUrl)
        {
            apiUrl = _apiUrl;
            var options = new RestClientOptions
            {
                ThrowOnAnyError = true,
            };

            client = new RestClient(options);
        }

        public async Task<ApiResponse> ExecuteAsync(BaseRequest baseRequest)
        {
            try
            {
                var request = new RestRequest(apiUrl + baseRequest.Route, baseRequest.Method);
                request.AddHeader("Content-Type", baseRequest.ContentType);

                if (baseRequest.Parameter != null)
                {
                    if (baseRequest.Method == Method.Get)
                    {
                        AddQueryParameters(request, baseRequest.Parameter);
                    }
                    else
                    {
                        request.AddJsonBody(baseRequest.Parameter);
                    }
                }

                var response = await client.ExecuteAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = response.Content ?? string.Empty;
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                    if (apiResponse == null)
                    {
                        return new ApiResponse("反序列化API响应时出错:" + baseRequest.Route);
                    }
                    return apiResponse;
                }
                else
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage))
                    {
                        return new ApiResponse("调用API出现未知错误:" + baseRequest.Route);
                    }
                    else
                    {
                        return new ApiResponse(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse(ex.Message);
            }
        }

        public async Task<ApiResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)
        {
            try
            {
                var request = new RestRequest(apiUrl + baseRequest.Route, baseRequest.Method);
                request.AddHeader("Content-Type", baseRequest.ContentType);

                if (baseRequest.Parameter != null)
                {
                    if (baseRequest.Method == Method.Get)
                    {
                        AddQueryParameters(request, baseRequest.Parameter);
                    }
                    else
                    {
                        request.AddJsonBody(baseRequest.Parameter);
                    }
                }

                var response = await client.ExecuteAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = response.Content ?? string.Empty;
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
                    if (apiResponse == null)
                    {
                        return new ApiResponse<T>("反序列化API响应时出错:" + baseRequest.Route);
                    }
                    return apiResponse;
                }
                else
                {
                    if (string.IsNullOrEmpty(response.ErrorMessage))
                    {
                        return new ApiResponse<T>("调用API出现未知错误:" + baseRequest.Route);
                    }
                    else
                    {
                        return new ApiResponse<T>(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>(ex.Message);
            }
        }

        private void AddQueryParameters(RestRequest request, object parameter)
        {
            var type = parameter.GetType();

            // Handle array parameters (e.g., string[])
            if (type.IsArray)
            {
                var array = (System.Collections.IEnumerable)parameter;
                foreach (var item in array)
                {
                    request.AddQueryParameter("subSystemNames", item?.ToString());
                }
            }
            // Handle object parameters with properties
            else
            {
                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(parameter);
                    if (value != null)
                    {
                        request.AddQueryParameter(prop.Name, value.ToString());
                    }
                }
            }
        }
    }
}
