using diabidabackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class CommentDTO
    {
        public int id { get; set; }
        public string commentText { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public UserDTO specialist { get; set; }
        public UserDTO patient { get; set; }
        public DateTime recordDate { get; set; }
        public List<CommentResponseDTO> responses { get; set; }
    }
}