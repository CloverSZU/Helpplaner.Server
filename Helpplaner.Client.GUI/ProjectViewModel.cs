﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Helpplaner.Service.Objects;



namespace Helpplaner.Client.GUI
{
   public class ProjectViewModel 

    {

        public ProjectViewModel()
        { }


        public IEnumerable<Service.Objects.WorkPackage> Tasks { get; set; }

        public IEnumerable<User> users { get; set; }

        
     
    }
}
