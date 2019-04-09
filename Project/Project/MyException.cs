using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class MyException : Exception
    {
        public MyException(String msg) : base(msg)
        {
        }
    }
}
