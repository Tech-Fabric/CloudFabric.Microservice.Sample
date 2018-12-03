using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudFabric.SampleService.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
    }
}
