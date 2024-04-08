﻿using Helpplaner.Service.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Helpplaner.Client.GUI.Pages
{
    /// <summary>
    /// Interaktionslogik für NewAP.xaml
    /// </summary>
    public partial class NewAP : Window
    {
        Project Project;
        Helpplaner.Service.Objects.WorkPackage[] APs;
        ServerCommunicator sr;

        ProjectViewModel pvm;



        public NewAP(Project proj, ServerCommunicator sr, ProjectViewModel pvm)
        {
            InitializeComponent();
            Project = proj;
            this.sr = sr;
            this.DataContext = pvm;
            this.pvm = pvm;

            Zuständiger.ItemsSource = pvm.users;    




        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Service.Objects.WorkPackage task = new Service.Objects.WorkPackage();
                task.ProjectID = Project.ID;
                task.Name = Name.Text;
                task.Description = Beschreibung.Text;
                task.RealTime = "1";
                task.ExpectedTime = "1"; 
               
                    User user = (User)Zuständiger.SelectedItem;
                
           

                task.Responsible = user.ID;

                sr.AddTaskToProject(task, Convert.ToInt32(Project.ID));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
            
            this.Close();

           


        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EnableButton()
        {
            if (Name.Text != "" && Beschreibung.Text != "" && Zuständiger.SelectedItem != null)
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
            EnableButton();
        }
    }
}
