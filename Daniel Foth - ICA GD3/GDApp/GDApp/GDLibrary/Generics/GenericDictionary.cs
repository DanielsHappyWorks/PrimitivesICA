﻿/*
Function: 		Provide generic map to associate key-value pairs AND allow dispose() to be called on all contents
Author: 		NMCG
Version:		1.0
Date Updated:	2/10/16
Bugs:			None
Fixes:			None
*/

using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class GenericDictionary<K, V> : IDisposable
    {
        #region Variables
        private string name;
        private Dictionary<K, V> dictionary;
        #endregion

        #region Properties
        protected Dictionary<K, V> Dictionary
        {
            get
            {
                return dictionary;
            }
        }

        public V this[K key]
        {
            get
            {
                if (!this.Dictionary.ContainsKey(key))
                    throw new Exception(key + " was not found in dictionary. Have you loaded it?");

                return this.dictionary[key];
            }
        }
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        #endregion

        public GenericDictionary(string name)
        {
            this.name = name;
            this.dictionary = new Dictionary<K, V>();
        }

        public virtual void Add(K key, V value)
        {
            if (!this.dictionary.ContainsKey(key))
            {
                this.dictionary.Add(key, value);
            }
        }

        public virtual void Remove(K key)
        {
            if (this.dictionary.ContainsKey(key))
            {
                V value = this.dictionary[key];
                this.dictionary.Remove(key);
            }
        }

        public virtual void Clear()
        {
            foreach (K key in this.dictionary.Keys)
            {
                Remove(key);
            }
        }

        public virtual int Count()
        {
            return this.dictionary.Count;
        }

        public virtual void Dispose()
        {
            //copy values from dictionary to list
            List<V> list = new List<V>(dictionary.Values);

            for (int i = 0; i < list.Count; i++)
            {
                V value = list[i];

                //if this is a disposable object (e.g. model, sound, font, texture) then call its dispose
                if (value is IDisposable)
                    ((IDisposable)value).Dispose();
                //if it's just a user-defined or C# object, then set to null for garbage collection
                else
                    value = default(V);
            }

            //empty the list
            list.Clear();

            //clear the dictionary
            this.dictionary.Clear();
        }
    }
}
