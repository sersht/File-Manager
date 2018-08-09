namespace WindowsFormsApp
{
    public partial class MainForm : Form
    {
        // Початкові значення для уникнення помилок при доступі до неініціалізованих полів
        private string GlobalNavigationStr = @"E:\";

        #region Observer
        Observer MainObs = new Observer();
        #endregion

        #region Функції оновлення вікон
        public void RefreshListView(string _funcPath, ListView LW)
        {
            LW.Items.Clear();

            //Заповнюємо рядки-папки
            string[] a = Directory.GetDirectories(_funcPath);
            foreach (string b in a)
            {
                DirectoryClass dir = new DirectoryClass(b);
                ListViewItem lvi = new ListViewItem(b);

                lvi.SubItems.Add(dir.GetName());
                lvi.SubItems.Add(@"Папка");

                LW.Items.Add(lvi);
            }

            //Заповнюємо рядки-файли
            a = Directory.GetFiles(_funcPath);

            int slashPos = -1, pointPos = -1;
            long length = -1;

            foreach (string b in a)
            {
                FileClass file = new FileClass(b);

                slashPos = b.LastIndexOf('\\'); // EXC: тут slashPos может быть равен -1
                pointPos = b.LastIndexOf('.');

                ListViewItem lvi = new ListViewItem(b);

                lvi.SubItems.Add(file.GetName());
                lvi.SubItems.Add(file.GetExtension());

                length = (new FileInfo(b).Length + 1000) / 1000;
                lvi.SubItems.Add(length.ToString() + @" Kb");

                LW.Items.Add(lvi);
            }
        }

        public void RefreshListViewLD(ListView LW, ToolStripTextBox TSTB, string GlobStr)
        {
            LW.Items.Clear();

            string[] drives = Environment.GetLogicalDrives();
            foreach (string st in drives)
            {
                ListViewItem lvi = new ListViewItem(st);

                lvi.SubItems.Add(st);
                lvi.SubItems.Add(@"Диск");

                LW.Items.Add(lvi);
            }

            GlobStr = "Мій Комп\'ютер";
            TSTB.Text = "Мій Комп\'ютер";
        }
        #endregion

        #region Ініціалізація/завантаження елементів головної форми
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshListViewLD(listView1, toolStripTextBox1, GlobalNavigationStr);
        }

        #endregion

        #region Навігація та взаємодія з вікном
        // Навігація по файловій системі - подвійний клік на елементі ListView
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Отримуємо інформацію за кліком на елемент ListView за індексом та SubItems[0] 
            try
            {
                int selectedIndex = listView1.SelectedIndices[0];
                string rootPath = listView1.Items[selectedIndex].SubItems[0].Text,
                       extens = listView1.Items[selectedIndex].SubItems[2].Text;

                if (extens == @"Папка" || extens == @"Диск")
                {
                    RefreshListView(rootPath, listView1);
                    GlobalNavigationStr = rootPath;
                    toolStripTextBox1.Text = rootPath;
                } //якщо обрана директорія
                else if (extens == @"text")
                {
                    Form3.FileToBeOpened = rootPath;
                    Form3 form3 = new Form3();
                    form3.Show();
                } //якщо обрана таблиця
                else if (extens == @"html")
                {
                    FileClass file = new FileClass(rootPath);
                    ParseHTML html = new ParseHTML(file.GetFullPath(), file.GetParentPath(), file.GetName());
                    html.Parse();
                } //якщо обраний html
                else if (extens == @"txt")
                {
                    TextEditor te = new TextEditor(rootPath);
                    te.Owner = this;
                    te.Show();
                } //якщо обраний текстовий файл
                else
                {
                    NewTestingClass n = new NewTestingClass();
                    FileClass file = new FileClass(rootPath);
                    if (n.TestGettingFileName(file))
                    {
                        MessageBox.Show("OK!!!");
                    }
                } //якщо обрано щось інше - не робити нічого
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Навігація по файловій системі - повернутися у попереднє вікно
        private void button3_Click(object sender, EventArgs e)
        {
            if (GlobalNavigationStr.Length < 4 || GlobalNavigationStr == "Мій Комп\'ютер")
            {
                RefreshListViewLD(listView1, toolStripTextBox1, GlobalNavigationStr);
            }
            else
            {
                int slashPos = GlobalNavigationStr.LastIndexOf('\\');
                string s = GlobalNavigationStr.Substring(0, slashPos);

                if (s.Length < 3)
                {
                    s += @"\";
                }

                GlobalNavigationStr = s;
                toolStripTextBox1.Text = s;

                RefreshListView(s, listView1);
            }
        }

        // Навігація по файловій системі - перехід до директорії за заданим шляхом
        private void GoTo_Click(object sender, EventArgs e)
        {
            string str = toolStripTextBox1.Text;
            if (Directory.Exists(str))
            {
                RefreshListView(str, listView1);
            } // Якщо директорія існує - переходить до неї
            else
            {
                MessageBox.Show(@"Invalid path");
            }
        }

        // Кнопка видалення файла або папки
        private void button5_Click(object sender, EventArgs e)
        {
            Subject test = new Subject("Delete");
            test.AddObserver(MainObs);

            if (listView1.SelectedIndices.Count != 0)
            {
                int selectedIndex = listView1.SelectedIndices[0];
                string objPath = listView1.Items[selectedIndex].SubItems[0].Text,
                       extens = listView1.Items[selectedIndex].SubItems[2].Text;

                // Неможливо видалити диск
                bool isDisk = false;
                string[] drivers = Environment.GetLogicalDrives();
                foreach (string s in drivers)
                {
                    if (s == objPath)
                    {
                        isDisk = true;
                    }
                }

                if (!isDisk)
                {
                    DeleteOrNot form = new DeleteOrNot(objPath, extens);
                    form.Owner = this;
                    form.Show();
                    test.NotifyObservers("return message : Request processing by another form");
                }
                else
                {
                    test.NotifyObservers("return message : You can't delete disk");
                    MessageBox.Show("You can't delete disk");
                }

                richTextBox1.Text = MainObs.GetLog();
            }
        }

        // Перемістити файл/папку
        private void button9_Click(object sender, EventArgs e)
        {
            Subject test = new Subject("Move");
            test.AddObserver(MainObs);

            if (listView1.SelectedIndices.Count != 0)
            {
                int selectedIndex = listView1.SelectedIndices[0];

                string pathToObj = listView1.Items[selectedIndex].SubItems[0].Text,
                       name = listView1.Items[selectedIndex].SubItems[1].Text,
                       extens = listView1.Items[selectedIndex].SubItems[2].Text;

                if (name[1] != ':' && name[2] != '\\') // Неможливо перемістити диск
                {
                    EnterDestinationOf form = new EnterDestinationOf(pathToObj, name, extens);
                    form.Owner = this;
                    form.Show();
                    test.NotifyObservers("return message : Request processing by another form");
                    richTextBox1.Text = MainObs.GetLog();
                }
                else
                {
                    test.NotifyObservers("return message : You can't move disk");
                    MessageBox.Show("You can't move disk");
                }

                richTextBox1.Text = MainObs.GetLog();
            }
        }

        // Створення файла або директорії за поточним шляхом
        private void button10_Click(object sender, EventArgs e)
        {
            Subject test = new Subject("Create");
            test.AddObserver(MainObs);

            test.NotifyObservers("return message : Request processing by another form");
            richTextBox1.Text = MainObs.GetLog();

            EnterNameOf form = new EnterNameOf(GlobalNavigationStr);
            form.Owner = this;
            form.Show();
        }

        //Змінити ім'я файлу або папки
        private void button7_Click(object sender, EventArgs e)
        {
            Subject test = new Subject("Change");
            test.AddObserver(MainObs);

            if (listView1.SelectedIndices.Count != 0)
            {
                int selectedIndex = listView1.SelectedIndices[0];

                string pathToObj = listView1.Items[selectedIndex].SubItems[0].Text,
                       extens = listView1.Items[selectedIndex].SubItems[2].Text;

                bool isDisk = false;

                if (extens == @"Диск")
                {
                    isDisk = true;
                }

                if (isDisk)
                {
                    test.NotifyObservers("return message: Can't rename root-directory");
                }
                else
                {
                    RenameObject form = new RenameObject(pathToObj, extens);
                    form.Owner = this;
                    form.Show();

                    test.NotifyObservers("return message : Request processing by another form");
                    richTextBox1.Text = MainObs.GetLog();
                }
            }
        }

        // Відкриття текстового редактора
        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                TextEditor te = new TextEditor();
                te.Owner = this;
                te.Show();
            }
        }
        
        // Кнопка "Про"
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("\t\tШемчука Сергія, К-26.\nФайловий менеджер з базовою функціональністю за наступним (7) варіантом.");
        }
        
        // Кнопка "Допомога" 
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("*Насолоджуйтесь інуїтивно зрозумілим інтерфейсом*");
        }
        #endregion
    }
}