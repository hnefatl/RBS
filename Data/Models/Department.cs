﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;

using Shared;

namespace Data.Models
{
    [Table("Departments")]
    public class Department
        : DataModel
    {
        // Name of the department (eg Computing, Maths)
        public string Name { get; set; }

        // Teachers in the department
        public virtual List<Teacher> Teachers { get; set; }
        // Rooms owned by the department
        public virtual List<Room> Rooms { get; set; }

        public Department()
        {
            Name = string.Empty;

            Teachers = new List<Teacher>();
            Rooms = new List<Room>();
        }

        public override bool Conflicts(List<DataModel> Others)
        {
            // Conflicts occur on matching names and ID
            return Others.Cast<Department>().Any(d => d.Id != Id && d.Name == Name);
        }
        
        public override void Update(DataModel Other)
        {
            Department d = (Department)Other;

            Name = d.Name;
            Teachers.Clear();
            Teachers.AddRange(d.Teachers);
            Rooms.Clear();
            Rooms.AddRange(d.Rooms);
        }

        // Serialise properties and IDs
        public override void Serialise(Writer Out)
        {
            base.Serialise(Out);

            Out.Write(Name);
            Out.Write(Teachers.Count);
            Teachers.ForEach(t => Out.Write(t.Id));
            Out.Write(Rooms.Count);
            Rooms.ForEach(r => Out.Write(r.Id));
        }
        // Deserialise properties and IDs
        protected override void Deserialise(Reader In)
        {
            base.Deserialise(In);

            Name = In.ReadString();
            Teachers = Enumerable.Repeat(new Teacher(), In.ReadInt32()).ToList();
            Teachers.ForEach(t => t.Id = In.ReadInt32());
            Rooms = Enumerable.Repeat(new Room(), In.ReadInt32()).ToList();
            Rooms.ForEach(r => r.Id = In.ReadInt32());
        }
        // Obtain references to related items
        public override bool Expand(IDataRepository Repo)
        {
            try
            {
                for (int x = 0; x < Teachers.Count; x++)
                    Teachers[x] = Repo.Users.OfType<Teacher>().SingleOrDefault(t => t.Id == Teachers[x].Id);
                for (int x = 0; x < Rooms.Count; x++)
                    Rooms[x] = Repo.Rooms.SingleOrDefault(r => r.Id == Rooms[x].Id);
            }
            catch
            {
                return false;
            }
            return true;
        }
        // Set references to this item
        public override void Attach()
        {
            Teachers.ForEach(t => t.Department = this);
            Rooms.ForEach(r => r.Department = this);
        }
        // Remove references to this item
        public override void Detach()
        {
            Teachers.ForEach(t => { if (t != null) t.Department = null; });
            Rooms.ForEach(r => { if (r != null) r.Department = null; });
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
