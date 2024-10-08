﻿namespace Helpplaner.Service.Core
{
    using System.Data.SqlClient;
    using System.Net;
    using System.Net.Sockets;
    using Helpplaner.Service.Shared;
    using Helpplaner.Service.SqlHandling;
    using Helpplaner.Service.Objects;
 
    public class Server
    {
        Socket _socket;  
        IPEndPoint _endPoint;    
        IPAddress _ipAddress;   
        int _port;  
        IServiceLogger _logger;
        ClientHandler clientHandler;


        Dictionary<Guid, SessionHandler> _sessions = new Dictionary<Guid, SessionHandler>();
        SqlConnection _connection;
        SelectSqlCommandHandler _selectSqlCommandHandler;


        public    bool isRunning = false;  
        public Server(IServiceLogger logger , string ipAddress = "127.0.0.1", int port = 50000 )
        {
            _ipAddress = IPAddress.Parse(ipAddress);
            _port = port;
           
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _logger = logger;
            Config config = new Config();

            _connection = new SqlConnection(config.ConnectionString);
           
          
            _selectSqlCommandHandler = new SelectSqlCommandHandler(_connection, _logger);
        }  
        
        public void Start()
        {
            try
            {
                _connection.Open();
                lock (_logger)
                {
                    _logger.Log("Connection to database opened", "green");
                }
                _endPoint = new IPEndPoint(_ipAddress, _port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(_endPoint);
                _socket.Listen(100);
                lock (_logger)
                {
                    _logger.Log("Server started", "green");
                    _logger.Log($"Listening on {_ipAddress}:{_port}", "green");
                }
                isRunning = true;
                clientHandler = new ClientHandler(_socket, _logger);
                Thread thread = new Thread(clientHandler.AcceptClients);
                thread.IsBackground = true;
                thread.Start();
                isRunning = true;
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, "red"); 
               
                _logger.Log("Server could not be started", "red");
                isRunning = false;


            }
           
         
            
            
         
          


        }
        public void Stop() {

            try
            {
               clientHandler.PostMessage("Shutdown");

                Thread.Sleep(5000);
                lock (_logger)
                {
                    _logger.Log("Server stopping", "red");
                }   
               
                _socket.Close();
                lock (_logger)
                {
                    _logger.Log("Socket closed", "red");
                }
                clientHandler.Close();
                lock (_logger)
                {
                    _logger.Log("All sessions closed", "red");
                }
           
               
              

            }
            catch (Exception e)
            {
                lock (_logger) { _logger.Log(e.Message, "red"); }
              
            }
                
                _logger.Log("Server stopped", "red");   
           
            _connection.Close();   
            _logger.Log("Connection to database closed", "red");    
           

          isRunning = false;


        }
        public void Restart()
        {
            Stop();
            Start();
        }

        public void GiveAllUsers()
        {
            try
            {
                User[] users = _selectSqlCommandHandler.GiveAllUsers();
                string[] usernames = new string[users.Length];
                for (int i = 0; i < users.Length; i++)
                {
                    _logger.Log(users[i].ToString(), "white");
                }
            }
            catch (NullReferenceException ex)
            {

                _logger.Log(ex.Message, "red"); 
            }
          
         
        }
        public void GiveUser(int id)
        {
            try
            {
              User user = _selectSqlCommandHandler.GiveUser(id);
                _logger.Log(user.ToString(), "white");
            }
            catch (NullReferenceException ex)
            {

                _logger.Log(ex.Message, "red");
            }


        }
        public void GiveProject(int id)
        {
            try
            {
                Project project = _selectSqlCommandHandler.GiveProjekt(id);
                _logger.Log(project.ToString(), "white");
            }
            catch (NullReferenceException ex)
            {

                _logger.Log(ex.Message, "red");
            }   
        }


    }
}