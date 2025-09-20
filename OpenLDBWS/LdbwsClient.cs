using System.Xml;
using System.Xml.Serialization;
using OpenLDBWS.Entities;
using OpenLDBWS.Options;
using System.Text;
using Microsoft.Extensions.Options;

namespace OpenLDBWS;
public class LdbwsClient : ILdbwsClient
{
    private readonly IOptionsMonitor<LdbwsOptions> _optionsMonitor;
    private readonly string _url;
    public LdbwsClient(IOptionsMonitor<LdbwsOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        _url = "https://lite.realtime.nationalrail.co.uk/OpenLDBWS/ldb12.asmx";
    }

    private static T? DeserialiseSoapResponse<T>(string xml) 
    {
        XmlSerializer serializer = new(typeof(SoapEnvelope));

        SoapEnvelope envelope;

        using (StringReader reader = new(xml))
        {
            envelope = (SoapEnvelope)(serializer.Deserialize(reader));
        }
    
        XmlElement bodyContent = envelope.Body.Content;

        serializer = new(typeof(T));

        T content;

        using (StringReader reader = new(bodyContent.OuterXml))
        {
            content = (T)(serializer.Deserialize(reader));
        }

        return content;
    }

    private async Task<string> SendSoapRequest(string requestBody)
    {

        string apiKey = _optionsMonitor.CurrentValue.ApiKey;

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("API key is not configured.");
        }

        string soapRequest = $@"
        <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:typ=""http://thalesgroup.com/RTTI/2013-11-28/Token/types"" xmlns:ldb=""http://thalesgroup.com/RTTI/2021-11-01/ldb/"">
        <soap:Header>
            <typ:AccessToken>
                <typ:TokenValue>{apiKey}</typ:TokenValue>
            </typ:AccessToken>
        </soap:Header>
        <soap:Body>
        {requestBody}
        </soap:Body>
        </soap:Envelope>";


        using (HttpClient client = new())
        {
            HttpRequestMessage request = new(HttpMethod.Post, _url)
            {
                Content = new StringContent(soapRequest, Encoding.UTF8, "text/xml")
            };

            HttpResponseMessage response = await client.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

    }

    public async Task<GetDepBoardWithDetailsResponse> GetDepBoardWithDetails(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120)
    {
        string soapRequest = $@"
            <ldb:GetDepBoardWithDetailsRequest>
                <ldb:numRows>{numRows}</ldb:numRows>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterCrs>{filterCrs}</ldb:filterCrs>
                <ldb:filterType>{filterType}</ldb:filterType>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetDepBoardWithDetailsRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetDepBoardWithDetailsResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetArrBoardWithDetailsResponse> GetArrBoardWithDetails(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120)
    {
        string soapRequest = $@"
            <ldb:GetArrBoardWithDetailsRequest>
                <ldb:numRows>{numRows}</ldb:numRows>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterCrs>{filterCrs}</ldb:filterCrs>
                <ldb:filterType>{filterType}</ldb:filterType>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetArrBoardWithDetailsRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetArrBoardWithDetailsResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetArrDepBoardWithDetailsResponse> GetArrDepBoardWithDetails(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120)
    {
        string soapRequest = $@"
            <ldb:GetArrDepBoardWithDetailsRequest>
                <ldb:numRows>{numRows}</ldb:numRows>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterCrs>{filterCrs}</ldb:filterCrs>
                <ldb:filterType>{filterType}</ldb:filterType>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetArrDepBoardWithDetailsRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetArrDepBoardWithDetailsResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetArrivalBoardResponse> GetArrivalBoard(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120)
    {
        string soapRequest = $@"
            <ldb:GetArrivalBoardRequest>
                <ldb:numRows>{numRows}</ldb:numRows>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterCrs>{filterCrs}</ldb:filterCrs>
                <ldb:filterType>{filterType}</ldb:filterType>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetArrivalBoardRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetArrivalBoardResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetArrivalDepartureBoardResponse> GetArrivalDepartureBoard(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120)
    {
        string soapRequest = $@"
            <ldb:GetArrivalDepartureBoardRequest>
                <ldb:numRows>{numRows}</ldb:numRows>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterCrs>{filterCrs}</ldb:filterCrs>
                <ldb:filterType>{filterType}</ldb:filterType>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetArrivalDepartureBoardRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetArrivalDepartureBoardResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetDepartureBoardResponse> GetDepartureBoard(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120)
    {
        string soapRequest = $@"
            <ldb:GetDepartureBoardRequest>
                <ldb:numRows>{numRows}</ldb:numRows>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterCrs>{filterCrs}</ldb:filterCrs>
                <ldb:filterType>{filterType}</ldb:filterType>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetDepartureBoardRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetDepartureBoardResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetFastestDeparturesResponse> GetFastestDepartures(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120)
    {
        StringBuilder crsList = new StringBuilder("");

        foreach(string filterCrs in filterCrsList)
        {
            crsList.AppendFormat($"<ldb:crs>{filterCrs}</ldb:crs>\n");
        }


        string soapRequest = $@"
            <ldb:GetFastestDeparturesRequest>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterList>
                    {crsList}
                </ldb:filterList>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetFastestDeparturesRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetFastestDeparturesResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetFastestDeparturesWithDetailsResponse> GetFastestDeparturesWithDetails(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120)
    {
        StringBuilder crsList = new StringBuilder("");

        foreach(string filterCrs in filterCrsList)
        {
            crsList.AppendFormat($"<ldb:crs>{filterCrs}</ldb:crs>\n");
        }


        string soapRequest = $@"
            <ldb:GetFastestDeparturesWithDetailsRequest>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterList>
                    {crsList}
                </ldb:filterList>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetFastestDeparturesWithDetailsRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetFastestDeparturesWithDetailsResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetNextDeparturesResponse> GetNextDepartures(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120)
    {
        StringBuilder crsList = new StringBuilder("");

        foreach(string filterCrs in filterCrsList)
        {
            crsList.AppendFormat($"<ldb:crs>{filterCrs}</ldb:crs>\n");
        }


        string soapRequest = $@"
            <ldb:GetNextDeparturesRequest>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterList>
                    {crsList}
                </ldb:filterList>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetNextDeparturesRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetNextDeparturesResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetNextDeparturesWithDetailsResponse> GetNextDeparturesWithDetails(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120)
    {
        StringBuilder crsList = new StringBuilder("");

        foreach(string filterCrs in filterCrsList)
        {
            crsList.AppendFormat($"<ldb:crs>{filterCrs}</ldb:crs>\n");
        }


        string soapRequest = $@"
            <ldb:GetNextDeparturesWithDetailsRequest>
                <ldb:crs>{crs}</ldb:crs>
                <ldb:filterList>
                    {crsList}
                </ldb:filterList>
                <ldb:timeOffset>{timeOffset}</ldb:timeOffset>
                <ldb:timeWindow>{timeWindow}</ldb:timeWindow>
            </ldb:GetNextDeparturesWithDetailsRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetNextDeparturesWithDetailsResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }

    public async Task<GetServiceDetailsResponse> GetServiceDetails(string serviceId)
    {
        string soapRequest = $@"
            <ldb:GetServiceDetailsRequest>
                <ldb:serviceID>{serviceId}</ldb:serviceID>
            </ldb:GetServiceDetailsRequest>";


        string response = await SendSoapRequest(soapRequest);

        if (response != null)
        {
            return DeserialiseSoapResponse<GetServiceDetailsResponse>(response);
        }

        throw new Exception("Unable to get response from api");
    }
}