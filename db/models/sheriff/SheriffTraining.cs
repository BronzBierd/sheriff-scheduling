﻿using System;
using System.ComponentModel.DataAnnotations;
using Mapster;
using ss.db.models;

namespace SS.Db.models.sheriff
{
    [AdaptTo("[name]Dto")]
    public class SheriffTraining : SheriffEvent
    {
        public virtual LookupCode TrainingType { get; set; }
        public int? TrainingTypeId { get; set; }
        public DateTimeOffset? TrainingCertificationExpiry { get; set; }
        [MaxLength(200)]
        public string Note { get; set; }

        public Boolean FirstNotice { get; set; }
    }
}
