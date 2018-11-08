using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace SlumpaGrupper
{
    class Person: DependencyObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool FirstNameUnique { private get; set; }
        public string Group { get; set; }

        public bool Deltar
        {
            get { return (bool)GetValue(DeltarProperty); }
            set { SetValue(DeltarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Deltar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeltarProperty =
            DependencyProperty.Register(nameof(Deltar), typeof(bool), typeof(Person));

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            FirstNameUnique = false;
            Deltar = true;
        }

        public override string ToString()
        {
            return FirstName +" "+ LastName;
        }
    }
}
