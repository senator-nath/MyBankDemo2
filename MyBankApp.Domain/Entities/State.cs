using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Domain.Entities
{
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<LGA> LGAs { get; set; } = new List<LGA>();
    }
}
