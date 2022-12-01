using System.Collections.Generic;


namespace SM.Application.PositionDetails.Queries.Response
{
    public class DetailsResponse
    {
        public PositionHeader Header { get; set; }
        public IEnumerable<ParameterPosition> Parameters { get; set; }
    }

    public class PositionHeader
    {
        public long PositionId { get; set; }
        public long CMCode { get; set; }
        public string Position { get; set; }
        public string SMCode { get; set; }
        public int? LevelId { get; set; }
        public long GroupId { get; set; }
        public string PositionSalaryMarkLabel { get; set; }
    }

    public class ParameterPosition
    {
        public long ParameterId { get; set; }
        public string Title { get; set; }
        public List<ProjectParameterPosition> ProjetParameters { get; set; }
    }

    public class ProjectParameterPosition
    {
        public long Id { get; set; }
        public long? ParentParameterId { get; set; }
        public string Title { get; set; }
    }

    public class PositionSMDetails
    {
        public PositionHeader Header { get; set; }
        public IEnumerable<long> ProjectParameterIds { get; set; }
        public IEnumerable<long> CareerAxisIds { get; set; }
    }
    public class ParameterListDetails
    {
        public long ProjectParameterId { get; set; }
        public long ParameterId { get; set; }
        public string ProjectParameter { get; set; }
    }

    public class CareerAxisDetails
    {
        public long CareerAxisId { get; set; }
        public string CareerAxis { get; set; }
        public long ParentParameterId { get; set; }
    }
}
