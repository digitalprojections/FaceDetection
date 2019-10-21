using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetection
{
    public class TaskManager
    {
        private static List<string> TAKSLIST = null;
        public TaskManager()
        {
            TAKSLIST1 = new List<string>();
        }

        public static List<string> TAKSLIST1 { get => TAKSLIST; set => TAKSLIST = value; }
    }
}
