using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtakDomain.Core.Entity
{
    public class Hotel
    {
        [Index(0)]
        public string Name { get; set; }

        [Index(1)]
        public string Address { get; set; }

        [Index(2)]
        public int Stars { get; set; }

        [Index(3)]
        public string Contact { get; set; }

        [Index(4)]
        public string Phone { get; set; }

        [Index(5)]
        public string Url { get; set; }
    }
}