using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCertificateCreationSystem.Common
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public dynamic Result { get; set; }
        public string Message { get; set; }

        public OperationResult(bool success, dynamic result, string message)
        {
            Success = success;
            Result = result;
            Message = message;
        }
    }
}
