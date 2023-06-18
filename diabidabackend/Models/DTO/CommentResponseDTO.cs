using diabidabackend.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace diabidabackend.Models.DTO
{
    public class CommentResponseDTO
    {
        public int id { get; set; }
        public string responseText { get; set; }
        public int commentId { get; set; } 
        public UserDTO user { get; set; }
        public DateTime recordDate { get; set; }
    }
}