/*
 * 06-Jun-2012 03:17AM
 * Jone Polvora
*/

//todo: Added AspectException class 

using System;
using System.Collections.Generic;

namespace DynamicObjectProxy
{
    public class AspectException : Exception
    {
        public List<Exception> Exceptions { get; private set; }

        public AspectException(List<Exception> exceptions)
        {
            Exceptions = exceptions;
        }
    }
}
