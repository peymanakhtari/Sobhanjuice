using RestSharp;

namespace SobhanJuice.Utilities
{
    public  class SendSMS
    {
        public static void ConfirmationCode(string number,string code)
        {
            var client = new RestClient("http://188.0.240.110/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"op\" : \"patternV2\"" +
                ",\"user\" : \"09171898681\"" +
                ",\"pass\":  \"@13931393Pa\"" +
                ",\"fromNum\" : \"+983000505\"" +
                $",\"toNum\": \"{number}\"" +
                ",\"patternCode\": \"ajs9svkjjnz8vxt\"" +
                ",\"inputData\" : {\"code\": \"" + code + "\"}}"
                , ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
    
    }
}
