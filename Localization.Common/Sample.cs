using System;
using System.Globalization;
using System.ComponentModel;
namespace CSLocalization.Test
{
    public class TestClass : INotifyPropertyChanging, INotifyPropertyChanged
    {
        static TestClass()
        {
            Current = new TestClass();
        }

        private CultureInfo culture = CultureInfo.CurrentCulture;

        public static TestClass Current { get; }

        public CultureInfo Culture
        {
            get { return culture; }
            set
            {
                if (culture == value)
                    return;
                this.OnPropertyChanging("Culture");
                culture = value;
                this.OnPropertyChanged("Culture");
                this.OnPropertyChanging("Sunday");
                this.OnPropertyChanged("Sunday");
                this.OnPropertyChanging("Monday");
                this.OnPropertyChanged("Monday");
                this.OnPropertyChanging("Tuesday");
                this.OnPropertyChanged("Tuesday");
                this.OnPropertyChanging("Wednesday");
                this.OnPropertyChanged("Wednesday");
                this.OnPropertyChanging("Thursday");
                this.OnPropertyChanged("Thursday");
                this.OnPropertyChanging("Friday");
                this.OnPropertyChanged("Friday");
                this.OnPropertyChanging("Saturday");
                this.OnPropertyChanged("Saturday");
            }
        }

        public string Sunday
        {
            get
            {
                string text;
                switch (Culture.LCID)
                {
                    case 127:
                        text = "Hello Sunday";
                        break;

                    default:
                        text = "Sunday";
                        break;
                }
                return text;
            }
        }

        public string Monday
        {
            get
            {
                string text;
                switch (Culture.LCID)
                {
                    case 127:
                        text = "Hello Monday";
                        break;

                    default:
                        text = "Monday";
                        break;
                }
                return text;
            }
        }

        public string Tuesday
        {
            get
            {
                string text;
                switch (Culture.LCID)
                {
                    case 127:
                        text = "Hello Tuesday";
                        break;

                    default:
                        text = "Tuesday";
                        break;
                }
                return text;
            }
        }

        public string Wednesday
        {
            get
            {
                string text;
                switch (Culture.LCID)
                {
                    case 127:
                        text = "Hello Wednesday";
                        break;

                    default:
                        text = "Wednesday";
                        break;
                }
                return text;
            }
        }

        public string Thursday
        {
            get
            {
                string text;
                switch (Culture.LCID)
                {
                    case 127:
                        text = "Hello Thursday";
                        break;

                    default:
                        text = "Thursday";
                        break;
                }
                return text;
            }
        }

        public string Friday
        {
            get
            {
                string text;
                switch (Culture.LCID)
                {
                    case 127:
                        text = "Hello Friday";
                        break;

                    default:
                        text = "Friday";
                        break;
                }
                return text;
            }
        }

        public string Saturday
        {
            get
            {
                string text;
                switch (Culture.LCID)
                {
                    case 127:
                        text = "Hello Saturday";
                        break;

                    default:
                        text = "Saturday";
                        break;
                }
                return text;
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        private void OnPropertyChanging(string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}