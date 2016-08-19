using Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class SaveNpdDTO
    {
        public string ParentId { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string SkuId { get; set; }

        public IList<NpdProductAttribute> Attributes { get; set; }
    }
}
