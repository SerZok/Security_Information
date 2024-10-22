
class Program
{
    static void Main()
    {
        Security security = new Security();
        string text;

        text = Security.Input();
        int[] ints = new int[10]{ 1, 2, 3, 4, 5, 6, 7 , 8, 9, 0};

        //Console.WriteLine(text);
        security.Coding(text, ints);


    }
}
//{А,Б,..,Я,<пробел>} 17

class Security {
    public static string Input()
    {
        return Console.ReadLine();
    }

    public void Coding(string ioText, int[]Cn)
    {
        blocks = Blocking(ioText);
        foreach(string block in blocks)
        {
            Console.WriteLine($"До кодирования: {block}");

            char[] chars = block.ToCharArray();

            for(int i=0; i<block.Length; i++)
            {
                chars[i] = chars[Cn[i]];
            }

            Console.WriteLine($"После кодирования: {chars}");

        }
    }

    private List<string> blocks = new List<string>();
    public int[] Cn;

    private List<string> Blocking(string ioText)
    {

        for (int i = 0; i < ioText.Length; i += 10)
        {
            // Извлекаем подстроку длиной до 10 символов
            string block = ioText.Substring(i, Math.Min(10, ioText.Length - i));

            // Если блок меньше 10 символов, дополняем пробелами
            while (block.Length < 10)
            {
                block += '*';
            }

            blocks.Add(block);
        }

        return blocks;
    }


}

