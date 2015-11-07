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
using System.ComponentModel;

using Data.Models;

namespace Client.EditWindows
{
    public partial class EditRoom
        : EditWindow<Room>
    {
        protected string _RoomName;
        public string RoomName
        {
            get
            {
                return _RoomName;
            }
            set
            {
                _RoomName = value;
                OnPropertyChanged("RoomName");
            }
        }

        protected string _StandardSeats;
        public string StandardSeats
        {
            get
            {
                return _StandardSeats;
            }
            set
            {
                _StandardSeats = value;
                OnPropertyChanged("StandardSeats");
            }
        }

        protected string _SpecialSeatType;
        public string SpecialSeatType
        {
            get
            {
                return _SpecialSeatType;
            }
            set
            {
                _SpecialSeatType = value;
                OnPropertyChanged("SpecialSeatType");
            }
        }

        protected string _SpecialSeats;
        public string SpecialSeats
        {
            get
            {
                return _SpecialSeats;
            }
            set
            {
                _SpecialSeats = value;
                OnPropertyChanged("SpecialSeats");
            }
        }

        protected string _Department;
        public string Department
        {
            get { return _Department; }
            set
            {
                _Department = value;
                OnPropertyChanged("Department");
            }
        }
        public string[] Departments { get; set; }

        protected int RoomId { get; set; }

        protected List<Booking> Bookings { get; set; }

        public EditRoom(Room Current)
        {
            using (DataRepository Repo = new DataRepository())
                Departments = Repo.Departments.Select(d => d.Name).ToArray();

            InitializeComponent();

            if (Current == null)
            {
                RoomName = string.Empty;
                StandardSeats = string.Empty;
                SpecialSeatType = string.Empty;
                SpecialSeats = string.Empty;
                Bookings = new List<Booking>();
                RoomId = 0;
            }
            else
            {
                RoomName = Current.RoomName;
                StandardSeats = Convert.ToString(Current.StandardSeats);
                SpecialSeatType = Current.SpecialSeatType;
                SpecialSeats = Convert.ToString(Current.SpecialSeats);
                Bookings = Current.Bookings;
                RoomId = Current.Id;
            }
        }

        public override Room GetItem()
        {
            Room New = new Room();

            try
            {
                New.RoomName = RoomName;
                New.StandardSeats = Convert.ToInt32(StandardSeats);
                New.SpecialSeatType = SpecialSeatType;
                New.SpecialSeats = Convert.ToInt32(SpecialSeats);
                New.Id = RoomId;
                New.Bookings = Bookings;

                using (DataRepository Repo = new DataRepository())
                    New.Department = Repo.Departments.Single(d => d.Name == Department);
            }
            catch
            {
                return null;
            }

            return New;
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
            base.Close();
        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            string Error = null;

            int Temp;
            if (string.IsNullOrWhiteSpace(RoomName))
                Error = "You must enter a room name.";
            else if (!int.TryParse(StandardSeats, out Temp) || Temp < 0)
                Error = "Standard seats must be a non-negative integer";
            else if (!int.TryParse(SpecialSeats, out Temp) || Temp < 0)
                Error = "Special seats must be a non-negative integer";
            else if (Temp != 0 && string.IsNullOrWhiteSpace(SpecialSeatType))
                Error = "You must enter a special seat type (eg Workbench, Computer)";
            else
            {
                using (DataRepository Repo = new DataRepository())
                    if (Repo.Rooms.Any(r => r.Id != RoomId && r.RoomName == RoomName))
                        Error = "Another room with that name already exists.";
            }

            if (Error != null)
                MessageBox.Show(Error, "Error", MessageBoxButton.OK);
            else
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
