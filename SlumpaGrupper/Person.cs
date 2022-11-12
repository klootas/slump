using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace SlumpaGrupper
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Person: DependencyObject
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Group { get; set; }

        [JsonProperty]
        public bool Presented { get; set; }

        public bool IsParticipating
        {
            get { return (bool)GetValue(IsParticipatingProperty); }
            set { SetValue(IsParticipatingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Deltar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsParticipatingProperty =
            DependencyProperty.Register(nameof(IsParticipating), typeof(bool), typeof(Person));

        public Person(string name)
        {
            Name = name;
            IsParticipating = true;
            Presented= false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}