using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Security security = new Security();

        string text = Console.ReadLine();
        int A = 25, C = 37, seed = 7;

        string encryptedText = security.Encrypt(text, A, C, seed);
        Console.WriteLine("Зашифрованный текст: " + encryptedText);

        string decryptedText = security.Decrypt(encryptedText, A, C, seed);
        Console.WriteLine("Расшифрованный текст: " + decryptedText);
    }
}

// Класс для гаммирования (шифрование и дешифрование)
class Security
{
    private readonly Dictionary<int, char> indexToAlphabet;
    private readonly Dictionary<char, int> alphabetToIndex;
    private const int AlphabetSize = 256; // Для расширенной ASCII таблицы
    public Security()
    {
        alphabetToIndex = new Dictionary<char, int>();
        indexToAlphabet = new Dictionary<int, char>();

        // Заполнение таблицы символов (ASCII)
        for (int i = 0; i < AlphabetSize; i++)
        {
            char symbol = (char)i;
            indexToAlphabet[i] = symbol;
            alphabetToIndex[symbol] = i;
        }
    }

    public string Encrypt(string text, int A, int C, int seed)
    {
        PRNG prng = new PRNG(A, C, seed); // Создаем генератор ПСЧ
        char[] encrypted = new char[text.Length];

        Console.WriteLine("*Проверка шифрования*");
        for (int i = 0; i < text.Length; i++)
        {
            int textCharIndex = alphabetToIndex[text[i]];  // Индекс символа текста
            int gamma = prng.T(i);                         // Генерируем элемент гаммы
            int encryptedCharIndex = (textCharIndex + gamma) % AlphabetSize;  // Сложение по модулю 256
            encrypted[i] = indexToAlphabet[encryptedCharIndex];               // Преобразуем обратно в символ
            Console.WriteLine($"Gamma:{gamma} | {textCharIndex}({indexToAlphabet[textCharIndex]}) -> {encryptedCharIndex}({indexToAlphabet[encryptedCharIndex]})");
        }
        Console.WriteLine("*Проверка шифрования*");


        return new string(encrypted);
    }
    public string Decrypt(string encryptedText, int A, int C, int seed)
    {
        PRNG prng = new PRNG(A, C, seed); // Создаем генератор ПСЧ
        char[] decrypted = new char[encryptedText.Length];

        Console.WriteLine("*Проверка дешифрования*");
        for (int i = 0; i < encryptedText.Length; i++)
        {
            int encryptedCharIndex = alphabetToIndex[encryptedText[i]];  // Индекс символа шифртекста
            int gamma = prng.T(i);                                      // Генерируем элемент гаммы
            int decryptedCharIndex = (encryptedCharIndex - gamma + AlphabetSize) % AlphabetSize;  // Вычитание по модулю 256
            decrypted[i] = indexToAlphabet[decryptedCharIndex];         // Преобразуем обратно в символ
            Console.WriteLine($"Gamma:{gamma} | {encryptedCharIndex}({indexToAlphabet[encryptedCharIndex]}) -> {decryptedCharIndex}({indexToAlphabet[decryptedCharIndex]})");
        }
        Console.WriteLine("*Проверка дешифрования*");

        return new string(decrypted);
    }
}

// Класс генератора псевдослучайных чисел
class PRNG
{
    private int A;
    private int B; // Модуль (256 для таблицы ASCII)
    private int C;
    private int currentT; //Текущее состояние

    public PRNG(int a, int c, int seed)
    {
        A = a;
        B = 256;  // Для расширенной таблицы ASCII
        C = c;
        currentT = seed;
    }

    // Генерация псевдослучайного числа
    public int T(int i)
    {
        if (i == 0) 
            return currentT;
        currentT = (A * currentT + C) % B;
        return currentT;
    }
}
