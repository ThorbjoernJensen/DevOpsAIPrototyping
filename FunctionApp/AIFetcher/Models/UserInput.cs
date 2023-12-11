using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIFetcher.Models
{
    internal class UserInput
    {
        public UserInput(string input, string workItems)
        {
            this.input = input;
            this.workItems = workItems;
        }

        public string input { get; set; }
        public string workItems { get; set; }
    }
}




