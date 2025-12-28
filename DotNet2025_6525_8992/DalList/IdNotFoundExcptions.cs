using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    internal class IdNotFoundExcptions: Exception
    {


        // בנאי ריק
        public IdNotFoundExcptions() : base("מזהה לא קיים.")
        {
        }

        // בנאי המקבל הודעה מותאמת אישית
        public IdNotFoundExcptions(string message) : base(message)
        {
        }

        // בנאי המקבל הודעה ושגיאה פנימית (Inner Exception)
        public IdNotFoundExcptions(string message, Exception inner) : base(message, inner)
        {
        }
    }

}

