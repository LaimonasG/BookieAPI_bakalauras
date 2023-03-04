﻿using Bakalauras.Auth.Model;

namespace Bakalauras.data.entities
{
    public class Profile
    {
        public int Id { get; set; }

        public double Points { get; set; }

        public ICollection<ProfileBook> ProfileBooks { get; set; }
        public ICollection<ProfileText> ProfileTexts { get; set; }

        public ICollection<DailyQuestionProfile> DailyQuestionProfiles { get; set; }

        public BookieUser User { get; set; }
    }
}
