using System.Collections.Generic;

namespace Kvin.Shorter
{
    public class Response {
        public bool Success{ get; set; }
        public string Payload{ get; set; }
        public List<string> ErrorMessages{ get; set; }

        public Response ()
        {
            ErrorMessages = new List<string>();
        }
    }
}