namespace WindowsFormsApp
{
    public class ParseHTML
    {
        string PathToFile;

        // шлях за замовчуванням
        string PathToOutput = @"E:\";

        string FileName;

        public ParseHTML(string path)
        {
            PathToFile = path;
        }

        public ParseHTML(string path, string outPath, string Name)
        {
            PathToFile = path;
            PathToOutput = outPath;
            FileName = Name;
        }

        public void ProcessOutput(string FileString)
        {
            string outputf = PathToOutput + @"\" + @"LIST__" + FileName + @".txt";

            if (!File.Exists(outputf))
            {
                using (var myFile = File.Create(outputf)) { }
            }

            // склали список всіх файлів-посилань
            string ToProcess = "";
            for (int i = 3; i < FileString.Length - 1; ++i)
            {
                if (FileString[i] == '=' && ( (FileString[i - 1] == 'f' && FileString[i - 2] == 'e') || (FileString[i - 2] == 'f' && FileString[i - 3] == 'e') ))
                {
                    // якщо є пробіл після = та перед "
                    if (FileString[i + 1] == ' ')
                    {
                        i += 3;
                    }
                    else
                    {
                        i += 2;
                    } 

                    // додати символ початку нового рядка
                    ToProcess += "^";
                    
                    while (FileString[i] != '"')
                    {
                        ToProcess += FileString[i];
                        ++i;
                    }

                    // додати символ закінчення рядка
                    ToProcess += "@";

                    ToProcess += Environment.NewLine;
                }
            }

            // обрали з них html та оформили відповідним чином
            FileStream file1 = new FileStream(outputf, FileMode.Open, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(file1);

            string wr = ""; int cntr = 0;
            for (int i = 0; i < ToProcess.Length - 5; ++i)
            {
                if (ToProcess[i] == '.' && 
                    ToProcess[i + 1] == 'h' && ToProcess[i + 2] == 't' &&
                    ToProcess[i + 3] == 'm' && ToProcess[i + 4] == 'l')
                {
                    int j = i;
                    while (ToProcess[j - 1] != '^')
                    {
                        --j;
                    }

                    wr += ("[" + (++cntr).ToString() + "] ");

                    for (int k = j; ToProcess[k] != '@'; ++k)
                    {
                        wr += ToProcess[k];
                    }

                    wr += Environment.NewLine;
                }
            }
            writer.Write(wr);
            writer.Close();
        }

        public void Parse()
        {
            FileStream file = new FileStream(PathToFile, FileMode.Open, FileAccess.ReadWrite);
            StreamReader reader = new StreamReader(file);

            string FileString = reader.ReadToEnd();
            reader.Close();

            // парсить <a></a> теги
            string toProcess = "";
            for (int i = 0; i < FileString.Length - 1; ++i)
            {
                if (FileString[i] == '<' && (FileString[i + 1] == 'a' || FileString[i + 1] == 'A'))
                {
                    // додати символ початку нового рядка
                    toProcess += "^";
                    
                    i += 3;
                    while (FileString[i] != '>')
                    {
                        toProcess += FileString[i];
                        ++i;
                    }

                    // додати символ закінчення рядка
                    toProcess += "@";
                    
                    toProcess += Environment.NewLine;
                }
            }

            //парсить посилання на файли
            ProcessOutput(toProcess);

            MessageBox.Show("Please, refresh current window!");
        }
    }
}