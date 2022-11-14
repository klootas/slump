using Newtonsoft.Json;
using System.Windows;

namespace SlumpaGrupper
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Person : DependencyObject
    {
        [JsonProperty]
        public string Name
        {
            get => _name.Trim();
            set => _name = value;
        }

        string _name;
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
            Presented = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}