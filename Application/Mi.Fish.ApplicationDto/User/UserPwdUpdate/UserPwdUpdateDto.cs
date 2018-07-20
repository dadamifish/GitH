using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class UserPwdUpdateDto
    {
        public string oldpwd { get; set; }
        public string newpwd { get; set; }
        public string userid { get; set; }
     }
}
