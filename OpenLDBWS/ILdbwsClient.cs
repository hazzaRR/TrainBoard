using OpenLDBWS.Entities;

namespace OpenLDBWS;

public interface ILdbwsClient
{
    Task<GetArrBoardWithDetailsResponse> GetArrBoardWithDetails(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120);
    Task<GetArrDepBoardWithDetailsResponse> GetArrDepBoardWithDetails(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120);
    Task<GetArrivalBoardResponse> GetArrivalBoard(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120);
    Task<GetArrivalDepartureBoardResponse> GetArrivalDepartureBoard(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120);
    Task<GetDepartureBoardResponse> GetDepartureBoard(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120);
    Task<GetDepBoardWithDetailsResponse> GetDepBoardWithDetails(int numRows, string crs, string filterCrs = "", string filterType = "to", int timeOffset = 0, int timeWindow = 120);
    Task<GetFastestDeparturesResponse> GetFastestDepartures(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120);
    Task<GetFastestDeparturesWithDetailsResponse> GetFastestDeparturesWithDetails(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120);
    Task<GetNextDeparturesResponse> GetNextDepartures(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120);
    Task<GetNextDeparturesWithDetailsResponse> GetNextDeparturesWithDetails(string crs, List<string> filterCrsList, int timeOffset = 0, int timeWindow = 120);
    Task<GetServiceDetailsResponse> GetServiceDetails(string serviceId);
}