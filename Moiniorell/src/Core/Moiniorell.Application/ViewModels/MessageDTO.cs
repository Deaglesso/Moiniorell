using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.ViewModels
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Class { get; set; }
        public DateTime Date { get; set; }
    }
}
