﻿namespace Bakalauras.data.entities
{
    public class ProfileBook
    {
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
