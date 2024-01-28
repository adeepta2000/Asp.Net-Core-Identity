using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCertificateCreationSystem.Common
{
    public class PayloadResponse<TEntity> where TEntity : class
    {
        public bool success { get; set; }
        public List<string> message { get; set; }
        public TEntity payload { get; set; }
        public string operation_type { get; set; }
    }
}
