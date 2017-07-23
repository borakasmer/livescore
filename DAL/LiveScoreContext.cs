namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LiveScoreContext : DbContext
    {
        public LiveScoreContext()
            : base("name=LiveScoreContext")
        {
        }

        public virtual DbSet<LiveScore> LiveScore { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LiveScore>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<LiveScore>()
                .Property(e => e.Css)
                .IsUnicode(false);
        }
    }
}
