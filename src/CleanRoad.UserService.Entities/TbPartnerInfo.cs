﻿using System;

namespace CleanRoad.UserService.Entities
{
    public partial class TbPartnerInfo
    {
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerPass { get; set; }
        public int? Balance { get; set; }
        public DateTime? Udate { get; set; }
    }
}
