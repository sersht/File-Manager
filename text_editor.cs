namespace WindowsFormsApp
{
    public partial class TextEditor : Form
    {
        string path = "";

        public TextEditor()
        {
            InitializeComponent();
        }

        public TextEditor(string p)
        {
            path = p;
            InitializeComponent();
        }

        // Новий файл
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        // Закрити
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Відкрити файл з вікна програми
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (.txt)|*.txt";
            ofd.Title = "Open a file...";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(ofd.FileName);
                richTextBox1.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        // Зберегти, як...
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (.txt)|*.txt";
            sfd.Title = "Saving a file...";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfd.FileName);
                sw.Write(richTextBox1.Text);
                sw.Close();
            }
        }

        // Відмінити останню дію 
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        // "Вирізати"
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        // Копіювати
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        // Вставити
        private void insertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        // Якщо заданий шлях до файлу - одразу відкриває його, інакше - відкривається як "новий файл"
        private void TextEditor_Load(object sender, EventArgs e)
        {
            if (path != "")
            {
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                StreamReader reader = new StreamReader(file);
                richTextBox1.Text = reader.ReadToEnd();
                reader.Close();
            }
        }

        // Заміна шаблону
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string Old = textBox1.Text,
                   New = textBox2.Text,
                   text = richTextBox1.Text;
                if (Old == "")
                {
                    MessageBox.Show("Empty old-string");
                }
                else
                {
                    text = text.Replace(Old, New);
                }
                richTextBox1.Text = text;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Пошук по шаблону
        private void SearchForTemplate(TextBox tb)
        {
            try
            {
                if (tb.Text == "")
                {
                    MessageBox.Show("Пустo!" + Environment.NewLine + "Ввoдіть у верхнє текстове поле!");
                }
                else
                {
                    FileStream file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                    StreamReader reader = new StreamReader(file);

                    string ListText = reader.ReadToEnd(),
                        template = tb.Text;
                    string[] Words = ListText.Split(); //список слів із файлу

                    reader.Close();

                    ITrie trie = new Trie();
                    foreach (string s in Words)
                    {
                        trie.AddWord(s);
                    }

                    bool flag = true;
                    for (int i = 0; i < template.Length && flag; ++i)
                    {
                        int Len = template.Length - i;
                        string s = template.Substring(0, Len);

                        if (trie.HasPrefix(s))
                        {
                            var PrefixWords = trie.GetWords(s);
                            string toShow = "";

                            if (PrefixWords.Count > 1)
                            {
                                toShow += ("Неоднозначно: " + Environment.NewLine);
                            }

                            foreach (string ss in PrefixWords)
                            {
                                toShow += (ss + Environment.NewLine);
                            }
                            MessageBox.Show(toShow);

                            flag = false;
                        }

                    }

                    if (flag)
                    {
                        MessageBox.Show("I cant find such word in the list!");
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        // Дія відповідної кнопки
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SearchForTemplate(textBox1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
