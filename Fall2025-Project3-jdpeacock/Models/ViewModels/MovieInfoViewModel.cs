namespace Fall2025_Project3_jdpeacock.Models.ViewModels
{
    public class MovieInfoViewModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<Actor> Actors { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Reviews { get; set; }
        public double AvgSentiment { get; set; }
    }
}