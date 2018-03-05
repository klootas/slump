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

        public bool Leder
        {
            get { return (bool)GetValue(LederProperty); }
            set { SetValue(LederProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Deltar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LederProperty =
            DependencyProperty.Register(nameof(Leder), typeof(bool), typeof(Person));

        //public event PropertyChangedEventHandler PropertyChanged;

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            FirstNameUnique = false;
            Deltar = true;
            Leder = false;
        }

        public void MakeLeader()
        {
            if (FirstName.Contains(']'))
            {
                return;
            }
            FirstName = "[L]" + FirstName;
        }

        public void DeLedifiy()
        {
            if (FirstName.Contains(']'))
            {
                FirstName = FirstName.Split(']')[1];
            }
        }

        public override string ToString()
        {
            return FirstName +" "+ LastName;
        }
    }
}
