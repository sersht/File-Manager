namespace WindowsFormsApp1
{
    interface ISubject
    {
        void AddObserver(Observer obs);
        void NotifyObservers(string state);
    }

    interface IObserver
    {
        void ObserveEvent(string state);
    }

    class Subject : ISubject
    {
        List<Observer> Observers = new List<Observer>();

        string name;

        public Subject(string input)
        {
            name = input;
        }

        public void AddObserver(Observer obs)
        {
            try
            {
                Observers.Add(obs);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void NotifyObservers(string state)
        {
            try
            {
                string s = (@"Notified by '" + name + @"'" + @" :: " + state);
                foreach (var observer in Observers)
                {
                    observer.ObserveEvent(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
    class Observer
    {
        string TextForLog = "";

        public void ObserveEvent(string Event)
        {
            try
            {
                DateTime localDateTime = DateTime.Now;
                TextForLog += (localDateTime.ToString() + " " + Event + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public string GetLog()
        {
            return TextForLog;
        }
    }
}
