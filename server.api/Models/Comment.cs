using System;

namespace server.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string CreatedBy { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public bool Deleted { get;set; }
    }
}