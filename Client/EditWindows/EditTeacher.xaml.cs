﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;

using Data.Models;

namespace Client.EditWindows
{
    public partial class EditTeacher
        : Window, INotifyPropertyChanged
    {
        protected string _FirstName;
        public string FirstName { get { return _FirstName; } set { _FirstName = value; OnPropertyChanged("FirstName"); } }

        protected string _LastName;
        public string LastName { get { return _LastName; } set { _LastName = value; OnPropertyChanged("LastName"); } }

        protected string _TeacherTitle;
        public string TeacherTitle { get { return _TeacherTitle; } set { _TeacherTitle = value; OnPropertyChanged("TeacherTitle"); } }

        protected string _LogonName;
        public string LogonName { get { return _LogonName; } set { _LogonName = value; OnPropertyChanged("LogonName"); } }

        protected string _Access;
        public string Access { get { return _Access; } set { _Access = value; OnPropertyChanged("Access"); } }
        public string[] AccessModes { get { return Enum.GetNames(typeof(AccessMode)); } }

        protected string _Email;
        public string Email { get { return _Email; } set { _Email = value; OnPropertyChanged("Email"); } }

        protected string _Department;
        public string Department { get { return _Department; } set { _Department = value; OnPropertyChanged("Department"); } }
        public string[] Departments { get; set; }

        protected List<Class> Classes { get; set; }

        public int TeacherId { get; set; }

        public EditTeacher(Teacher Existing)
        {
            using (DataRepository Repo = new DataRepository())
                Departments = Repo.Departments.Select(d => d.Name).ToArray();

            InitializeComponent();

            if (Existing == null)
            {
                FirstName = string.Empty;
                LastName = string.Empty;
                TeacherTitle = string.Empty;
                LogonName = string.Empty;
                Access = string.Empty;
                Email = string.Empty;
                Department = string.Empty;
                Classes = new List<Class>();
                TeacherId = 0;
            }
            else
            {
                FirstName = Existing.FirstName;
                LastName = Existing.LastName;
                TeacherTitle = Existing.Title;
                LogonName = Existing.LogonName;
                Access = Enum.GetName(typeof(AccessMode), Existing.Access);
                Email = Existing.Email;
                Department = Existing.Department.Name;
                Classes = Existing.Classes;
                TeacherId = Existing.Id;
            }
        }

        public Teacher GetTeacher()
        {
            Teacher New = new Teacher();

            try
            {
                New.FirstName = FirstName;
                New.LastName = LastName;
                New.Title = TeacherTitle;
                New.LogonName = LogonName;
                New.Access = (AccessMode)Enum.Parse(typeof(AccessMode), Access);
                New.Email = Email;
                using (DataRepository Repo = new DataRepository())
                    New.Department = Repo.Departments.Single(d => d.Name == Department);
                New.Classes = Classes;
                New.Id = TeacherId;
            }
            catch
            {
                return null;
            }

            return New;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            string Error = null;

            AccessMode Out;
            if (string.IsNullOrWhiteSpace(FirstName))
                Error = "You must enter a first name.";
            else if (string.IsNullOrWhiteSpace(LastName))
                Error = "You must enter a last name.";
            else if (string.IsNullOrWhiteSpace(TeacherTitle))
                Error = "You must enter a title.";
            else if (string.IsNullOrWhiteSpace(LogonName))
                Error = "You must enter a logon name.";
            else if (!Enum.TryParse(Access, out Out)) // Should never happen, we're using a combobox
                Error = "Invalid access mode.";
            // The possible patterns for emails are crazy, like 200+ character long regexes that still don't match all possibilities. Down to the user to enter a "valid" email.

            if (!string.IsNullOrWhiteSpace(Error))
                MessageBox.Show(Error, "Error", MessageBoxButton.OK);
            else
            {
                DialogResult = true;
                Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}