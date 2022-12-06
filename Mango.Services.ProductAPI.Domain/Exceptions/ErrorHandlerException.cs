using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Services.ProductAPI.Domain.Exceptions
{
    public class ErrorHandlerException : Exception
    {
        public ErrorHandlerException(string product, string message) 
            : base($"Product {product} not found. see inner message : {message}")
        {
        }

    }
}
