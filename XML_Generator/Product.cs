using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XML_Generator
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public List<string> Pictures { get; set; }
        public List<Tuple<string, string>> Params { get; set; }

    }

}
