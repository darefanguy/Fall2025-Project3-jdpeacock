using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Fall2025_Project3_jdpeacock.Models
{
    public class Actor
    {
        public int id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public string imdbLink { get; set; }
        public byte[]? photo { get; set; }

    }
}