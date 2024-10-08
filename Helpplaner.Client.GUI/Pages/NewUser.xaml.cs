﻿using Helpplaner.Service.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;


namespace Helpplaner.Client.GUI.Pages
{
    /// <summary>
    /// Interaktionslogik für NewAP.xaml
    /// </summary>
    public partial class NewUser : Window
    {
  
        Helpplaner.Service.Objects.WorkPackage[] APs;
        ServerCommunicator sr;

        ProjectViewModel pvm;

         User responsible;
        List<WorkPackage> dependencies;
        int time = 0;   


        public NewUser(ServerCommunicator sr, ProjectViewModel pvm)
        {
            InitializeComponent();
            dependencies = new List<WorkPackage>(); 

    
            this.sr = sr;
            this.DataContext = pvm;
            this.pvm = pvm;





        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              User newUSer = new User();    
                newUSer.Username = Name.Text;
                newUSer.Password = HashPassword( Password.Password);
                newUSer.Email = Email.Text;
                newUSer.IsSysadmin = Admin.IsChecked.Value;
                string Message = sr.CreateUser(newUSer);
                if (Message ==  "done")
                {
                    this.Close();

                }  
                else
                {
                    Waring.Content = Message;   
                }

              

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
            
           

           


        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EnableButton()
        {
            if (Name.Text != "" && Password.Password != "" &&   Regex.IsMatch(Email.Text, @"^[^@]+@[^@]+\.[^@]+$")  && Password.Password != "DummyPassowrd")
            {
                Save.IsEnabled = true;
            }
            else
            {
                Save.IsEnabled = false;
            }
        }

        private void Name_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = "";   
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableButton();
        }

        private void Beschreibung_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableButton();
        }

        private void Zuständiger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            responsible = (User)Zuständiger.SelectedItem; 
            EnableButton();
        }
        private void Dependants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dependencies.Clear();   
            foreach (var item in Dependants.SelectedItems)
            {
                dependencies.Add((WorkPackage)item);
            }   
            EnableButton();

        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            EnableButton();
        }

       

        public string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void Admin_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Admin_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
