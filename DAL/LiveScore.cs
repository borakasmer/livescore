namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LiveScore")]
    public partial class LiveScore
    {
        public int ID { get; set; }

        public int? Score { get; set; }

        [StringLength(20)]
        public string Name { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [StringLength(20)]
        public string Css { get; set; }
    }
}
