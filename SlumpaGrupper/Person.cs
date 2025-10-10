using System.Windows;

namespace SlumpaGrupper
{

    public class Person : DependencyObject
    {

        string _name;
        public string Name
        {
            get => _name.Trim();
            set => _name = value;
        }


        public string Group { get; set; }

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