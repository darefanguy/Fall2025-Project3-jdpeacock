using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2025_Project3_jdpeacock.Models
{
    [Index(nameof(ActorId), nameof(MovieId), IsUnique = true)]
    public class ActorMovie
    {

        [Key]
        public int id { get; set; }
        [ForeignKey("Actor")]
        public int ActorId { get; set; }
        public Actor? Actor { get; set; }
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
    }
}