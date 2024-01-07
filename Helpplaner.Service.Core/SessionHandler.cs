﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;   
using Helpplaner.Service.Shared;
using System.Data.SqlClient;
using Helpplaner.Service.SqlHandling;
using Helpplaner.Service.Objects;   

namespace Helpplaner.Service.Core
{
    internal class SessionHandler
    {
        Socket _clientSocket;
        IServiceLogger _logger;
       public Guid _sessionId;
       SocketWriter writer;
        SocketReader reader;
        SqlConnection _connection;  
        InsertSqlCommandHandler _insertSqlCommandHandler;   
        SelectSqlCommandHandler _selectSqlCommandHandler;  
       User user;


      public  SessionHandler(Socket client, IServiceLogger logger,Guid id)
        {
            _logger = logger;
            _clientSocket = client; 
            _sessionId = id;
            writer = new SocketWriter(_clientSocket, _logger);  
            reader = new SocketReader(_clientSocket, _logger);
           
               
                
                _connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=HELPPLANER;Integrated Security=True");
            _insertSqlCommandHandler = new InsertSqlCommandHandler(_connection, _logger);
            _selectSqlCommandHandler = new SelectSqlCommandHandler(_connection, _logger);

        }   
        public void HandleClient()
        {
            
            string text= "";
            
            while (text != "exit" )
            {
                if (user != null)
                {
                    text = reader.Read();
                    try
                    {
                        Project project =  new Project();
                        Objects.Task[] tasks; 
                        User[] users;
                        Project[] projects;
                        int id;
                        string check; 
                        _logger.Log(text, "green");
                        switch (text.Split(';')[0])
                        {

                           
                          
                            //format is command;parameter1;parameter2;parameter3;...    
                            case "getallusers":
                                OpenConnection();
                                writer.SendObject(_selectSqlCommandHandler.GiveAllUsers());
                                CloseConnection();
                                break;
                            case "getallprojects":
                                OpenConnection();
                                projects = _selectSqlCommandHandler.GetAllProjekte(user); 
                                
                                CloseConnection();
                                foreach (Project proj in projects)
                                {
                                    writer.SendObject(proj);
                                   check = reader.Read();
                                }   
                                writer.Send("done");
                                _logger.Log("All projects sent", "green");
                                break;
                            case "getalltasks":
                                //parameter1 is project id  
                                 id = int.Parse(text.Split(';')[1].Trim());
                               
                                OpenConnection();
                                project = _selectSqlCommandHandler.GiveProjekt(id);
                                tasks = _selectSqlCommandHandler.GetAllTasks(project);
                                CloseConnection();
                                foreach (Objects.Task task in project.Tasks)
                                {
                                    writer.SendObject(task);
                                }
                                writer.Send("done");
                               
                                break;
                            case "getalluserprojects":

                                id = int.Parse(text.Split(';')[1].Trim());

                                //parameter1 is project id
                                OpenConnection();
                                users =  _selectSqlCommandHandler.GetAllUsers(project);
                                CloseConnection();
                                foreach (User user in users)
                                {
                                    writer.SendObject(user);
                                }   
                                writer.Send("done");
                                break;

                            case "logout":
                                user = null;
                                writer.Send("done");
                                break;  

                        }
                    }
                    catch (Exception ex)
                    {

                       _logger.Log(ex.Message, "red");   
                        writer.Send("0!;error");
                    }
                    
                }
                else
                {
                    
                    // login sent in the format username;password    
                    text = reader.Read();
                    OpenConnection();   
                    CheckPassword(text);
                    CloseConnection();  

                   
                   
                }



            }
            Close();    
         
        }
        public void CloseConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }   
        public void OpenConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Closed)
            {
                _connection.Open();
            }
        }   
        private void CheckPassword(string text)
        {


            try
            {
                User trueUser = _selectSqlCommandHandler.GiveUser(text.Split(';')[0].Trim());
                if (trueUser.Nutzer_Passwort == text.Split(';')[1].Trim())
                {
                    user = trueUser;
                    writer.Send("done");
                    writer.SendObject(user);
                }
                else
                {
                    user = null;
                    writer.Send("NDone");

                }
            }
            catch (Exception EX)
            {

                writer.Send("Login nicht erfolgreich");
            }
            

        }

        public event EventHandler SessionClosed;    
        public void Close()
        {
            if(_clientSocket.Connected)
            {
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
               
            }
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
           
            if (SessionClosed != null) //required in C# to ensure a handler is attached
                SessionClosed(this, EventArgs.Empty);

        }
    }
}
