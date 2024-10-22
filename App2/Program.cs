class Program
{
    static void Main()
    {
        Security security = new Security();
        string text = Security.Input();
        int[] ints = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

        string enText = security.Coding(text, ints);
        string origText = security.DeCoding(enText, ints);

        Console.WriteLine($"Шифрованный текст: {enText}");
        Console.WriteLine($"Расшифрованный текст: {origText}");

    }
}
//{А,Б,..,Я,<пробел>} 17

class Security
{
    public int[] Cn;
    public static string Input()
    {
        Console.Write("Введите исходный текст: ");
        return Console.ReadLine();
    }
    public string Coding(string ioText, int[] Cn)
    {
        List<string> blocks = new List<string>();
        blocks = Blocking(ioText, blocks);
        for (int i = 0; i < blocks.Count; i++)
        {
            char[] chars = blocks[i].ToCharArray();
            char[] encodedChars = new char[chars.Length];

            //Кодирование (перестановка)
            for (int j = 0; j < blocks[i].Length; j++)
            {
                encodedChars[j] = chars[Cn[j]];
            }
            blocks[i] = new string(encodedChars);
        }
        return string.Join("", blocks);
    }
    public string DeCoding(string ioText, int[] Cn)
    {
        List<string> blocks = new List<string>();
        blocks = Blocking(ioText, blocks);
        for (int i = 0; i < blocks.Count; i++)
        {
            char[] encodedChars = blocks[i].ToCharArray();
            char[] decodedChars = new char[encodedChars.Length];

            for (int j = 0; j < blocks[i].Length; j++)
            {
                decodedChars[Cn[j]] = encodedChars[j];
            }

            blocks[i] = new string(decodedChars);
        }
        return string.Join("", blocks);
    }


    //Функция для разбиения на блоки
    private List<string> Blocking(string ioText, List<string> blocks)
    {
        for (int i = 0; i < ioText.Length; i += 10)
        {
            // Извлекаем подстроку длиной до 10 символов
            string block = ioText.Substring(i, Math.Min(10, ioText.Length - i));

            // Если блок меньше 10 символов, дополняем пробелами
            while (block.Length < 10)
            {
                block += ' ';
            }
            blocks.Add(block);
        }
        return blocks;
    }
}

