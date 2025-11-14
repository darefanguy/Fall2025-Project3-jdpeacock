namespace Fall2025_Project3_jdpeacock.Models
{
    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public string imdbLink { get; set; }
        public string genre { get; set; }
        public int releaseYear { get; set; }
        public byte[]? poster { get; set; }

    }
}