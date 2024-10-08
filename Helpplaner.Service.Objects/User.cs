﻿namespace Helpplaner.Service.Objects
{
    using System.Runtime.Serialization.Formatters.Binary;
    [Serializable]
    public class User
    {

        public User() { }   
        public string ID { get; set; }  
        public string Password { get; set; }
        public string Username { get; set; }    
        public string Email { get; set; }   
        public bool IsSysadmin { get; set; }  
        public bool IsAdmin { get; set; }   = false;
        

        public bool Selected { get; set; } = false; 

        /// <summary>
        /// asdada
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{ID}, {Username}" ;
        }
    
    }
}