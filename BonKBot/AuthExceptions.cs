using System;
using System.Collections.Generic;
using System.Text;

namespace BonKBot
{

    public class EqualUserAuthroityException : Exception
    {
        public EqualUserAuthroityException()
        {

        }

        public EqualUserAuthroityException(string message)
            : base(message)
        {

        }

        public EqualUserAuthroityException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

    public class UnauthorisedAuthroityException : Exception
    {
        public UnauthorisedAuthroityException()
        {

        }

        public UnauthorisedAuthroityException(string message)
            : base(message)
        {

        }

        public UnauthorisedAuthroityException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

    public class CalculationAuthorityException : Exception
    {
        public CalculationAuthorityException()
        {

        }

        public CalculationAuthorityException(string message)
            : base(message)
        {

        }

        public CalculationAuthorityException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }


}
