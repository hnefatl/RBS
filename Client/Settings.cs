﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Shared;

namespace Client
{
    //////////////////////////////////////////////////////////////////////////////////////////////
    // TODO: Ask client how to store client settings - file? Command-Line arguments on startup? //
    //////////////////////////////////////////////////////////////////////////////////////////////

    public sealed class Settings
    {
        private static string _Path = "Settings.txt";
        public static string Path { get { return _Path; } set { _Path = value; } }

        private static Dictionary<string, object> Inner { get; set; }

        // Static indexers don't work :(
        public static object Get(string Setting)
        {
            return Inner[Setting];
        }
        public static T Get<T>(string Setting)
        {
            return (T)Convert.ChangeType(Get(Setting), typeof(T));
        }

        public static void Set(string Setting, object Value)
        {
            Inner[Setting] = Value;
        }

        public static void Add(string Setting, object Value)
        {
            Inner.Add(Setting, Value);
        }
        public static void Remove(string Setting)
        {
            Inner.Remove(Setting);
        }
        public static void Clear()
        {
            Inner.Clear();
        }

        public static bool Contains(string Setting)
        {
            return Inner.ContainsKey(Setting);
        }

        public static bool Load()
        {
            try
            {
                Inner = new Dictionary<string, object>();

                using (FileStream File = new FileStream(Path, FileMode.Open))
                {
                    using (Reader Reader = GetReader(File))
                    {
                        while (true)
                        {
                            string Key = Reader.ReadString();
                            string Value = Reader.ReadString();
                            if (Key == null || Value == null)
                                break;

                            Inner.Add(Key, Value);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool Save()
        {
            try
            {
                using (FileStream File = new FileStream(Path, FileMode.Create))
                {
                    using (Writer Writer = GetWriter(File))
                    {
                        foreach (KeyValuePair<string, object> Setting in Inner)
                        {
                            Writer.Write(Setting.Key);
                            Writer.Write(Setting.Value);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static Reader GetReader(Stream Base)
        {
            return new Shared.TextReader(Base);
        }
        private static Writer GetWriter(Stream Base)
        {
            return new Shared.TextWriter(Base);
        }
    }
}