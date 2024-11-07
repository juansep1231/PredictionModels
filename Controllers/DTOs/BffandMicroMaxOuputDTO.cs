namespace DashboardModels.Controllers.DTOs
{
    public class BffandMicroMaxOuputDTO
    {
        public Dictionary<string, float> BFFProcessorScores { get; set; }
        public Dictionary<string, float> MicroProcessorScores { get; set; }
        public Dictionary<string, float> BFFMemoryScores { get; set; }
        public Dictionary<string, float> MicroMemoryScores { get; set; }
    }
}
