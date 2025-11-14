namespace Fall2025_Project3_jdpeacock.Models.ViewModels
{
    public class ActorInfoViewModel
    {
        public Actor Actor { get; set; }

        public IEnumerable<Movie> Movies { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Tweets { get; set; }

        public double AvgSentiment { get; set; }
    }
}