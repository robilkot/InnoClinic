﻿namespace CommonData.Messages
{
    public class SpecializationUpdate
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
